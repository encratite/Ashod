using Newtonsoft.Json;
using SuperSocket.SocketBase;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ashod.WebSocket
{
	public class WebSocketServer : IDisposable
	{
		int _Port;
		SuperWebSocket.WebSocketServer _Server;
		Dictionary<string, MethodInfo> _ServerMethods = new Dictionary<string, MethodInfo>();

		public WebSocketServer(int port)
		{
			_Port = port;
			_Server = new SuperWebSocket.WebSocketServer();
			_Server.NewSessionConnected += new SessionHandler<WebSocketSession>(OnConnect);
			_Server.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(OnDisconnect);
			_Server.NewMessageReceived += new SessionHandler<WebSocketSession, string>(OnMessage);
		}

		public virtual void Run()
		{
			InitialiseServerMethods();
			_Server.Setup(_Port);
			_Server.Start();
		}

		public virtual void Dispose()
		{
			if (_Server != null)
			{
				_Server.Dispose();
				_Server = null;
			}
		}

		bool IsServerMethod(MethodInfo methodInfo)
		{
			var attributes = methodInfo.GetCustomAttributes(typeof(WebSocketServerMethod), true);
			bool isServerMethod = attributes.Length > 0;
			return isServerMethod;
		}

		void InitialiseServerMethods()
		 {
			var type = GetType();
			var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
			var serverMethods = methods.Where(x => IsServerMethod(x));
			foreach (var method in serverMethods)
			{
				string name = JavaScriptContractResolver.GetJavaScriptName(method.Name);
				_ServerMethods[name] = method;
			}
		}

		void OnConnect(WebSocketSession session)
		{
		}

		void OnDisconnect(WebSocketSession session, CloseReason reason)
		{
		}

		void OnMessage(WebSocketSession session, string messageText)
		{
			try
			{
				var message = JsonConvert.DeserializeObject<CallMessage>(messageText);
				ProcessCall(session, message);
			}
			catch (Exception exception)
			{
				SendErrorResult(session, null, exception);
			}
		}

		void ProcessCall(WebSocketSession session, CallMessage message)
		{
			try
			{
				if (message.Method == null)
					throw new ArgumentException("Method must not be null");
				else if (message.Arguments == null)
					throw new ArgumentException("Arguments must not be null");
				MethodInfo method;
				if (!_ServerMethods.TryGetValue(message.Method, out method))
					throw new ArgumentException("No such method");
				var parameters = method.GetParameters();
				int expectedLength = parameters.Length;
				int inputLength = message.Arguments.Length;
				if (expectedLength != inputLength)
				{
					string error = string.Format("Invalid argument count, expected {0} but received {1}", expectedLength, inputLength);
					throw new ArgumentException(error);
				}
				var convertedArguments = new List<object>();
				for (int i = 0; i < expectedLength; i++)
				{
					var methodArgumentType = parameters[i].ParameterType;
					var argument = message.Arguments[i];
					var inputArgumentType = argument.GetType();
					if (methodArgumentType == typeof(Int32) && inputArgumentType == typeof(Int64))
					{
						long input = (long)argument;
						int convertedValue = (int)input;
						convertedArguments.Add(convertedValue);
					}
					else if (methodArgumentType != inputArgumentType)
					{
						string error = string.Format("Encountered an invalid type in argument {0}, expected {1} but received {2}", i + 1, methodArgumentType, inputArgumentType);
						throw new ArgumentException(error);
					}
					else
						convertedArguments.Add(argument);
				}
				var output = method.Invoke(this, convertedArguments.ToArray());
				var outputMessage = ResultMessage.Success(message.Id, output);
				SendMessage(session, outputMessage);
			}
			catch (Exception exception)
			{
				SendErrorResult(session, message.Id, exception);
			}
		}

		string GetFullExceptionMessage(Exception exception)
		{
			var messages = new List<string>();
			while (exception != null)
			{
				messages.Add(exception.Message);
				exception = exception.InnerException;
			}
			string output = exception.GetType() + ": " + string.Join(" ", messages.ToArray());
			output = output.Replace("\r", "");
			output = output.Replace("\n", " ");
			return output;
		}

		void SendErrorResult(WebSocketSession session, int? id, Exception exception)
		{
			string message = GetFullExceptionMessage(exception);
			var result = ResultMessage.Exception(id, message);
			SendMessage(session, result);
		}

		void SendMessage(WebSocketSession session, ResultMessage message)
		{
			JsonSerializerSettings serialiserSettings = new JsonSerializerSettings();
			serialiserSettings.ContractResolver = new JavaScriptContractResolver();
			string messageText = JsonConvert.SerializeObject(message, serialiserSettings);
			session.Send(messageText);
		}
	}
}
