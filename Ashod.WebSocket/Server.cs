using Newtonsoft.Json;
using SuperSocket.SocketBase;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ashod.WebSocket
{
	public class Server
	{
		int _Port;
		WebSocketServer _Server;

		Dictionary<string, MethodInfo> _ServerMethods = new Dictionary<string, MethodInfo>();

		public Server(int port)
		{
			_Port = port;
			_Server = new WebSocketServer();
			_Server.NewSessionConnected += new SessionHandler<WebSocketSession>(OnConnect);
			_Server.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(OnDisconnect);
			_Server.NewMessageReceived += new SessionHandler<WebSocketSession, string>(OnMessage);
		}

		public void Run()
		{
			InitialiseServerMethods();
			_Server.Setup(_Port);
			_Server.Start();
		}

		bool IsServerMethod(MethodInfo methodInfo)
		{
			return methodInfo.GetCustomAttributes(typeof(ServerMethod), true).Length > 0; ;
		}

		void InitialiseServerMethods()
		{
			var type = GetType();
			var methods = type.GetMethods();
			var serverMethods = methods.Where(x => IsServerMethod(x));
			foreach (var method in serverMethods)
				_ServerMethods[method.Name] = method;
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
				var result = ResultMessage.Exception(null, exception.Message);
				SendMessage(session, result);
			}
		}

		void ProcessCall(WebSocketSession session, CallMessage message)
		{
			try
			{
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
				for (int i = 0; i < expectedLength; i++)
				{
					var methodArgumentType = parameters[i].ParameterType;
					var inputArgumentType = message.Arguments[i].GetType();
					if (methodArgumentType != inputArgumentType)
					{
						string error = string.Format("Encountered an invalid type in argument {0}, expected {1} but received {2}", i + 1, methodArgumentType, inputArgumentType);
						throw new ArgumentException(error);
					}
				}
				var output = method.Invoke(this, message.Arguments);
				var outputMessage = ResultMessage.Success(message.Id, output);
				SendMessage(session, outputMessage);
			}
			catch (Exception exception)
			{
				var result = ResultMessage.Exception(message.Id, exception.Message);
				SendMessage(session, result);
			}
		}

		void SendMessage(WebSocketSession session, ResultMessage message)
		{
			string messageText = JsonConvert.SerializeObject(message);
			session.Send(messageText);
		}
	}
}
