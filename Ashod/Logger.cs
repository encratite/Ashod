using System;

namespace Ashod
{
	public enum LogLevel
	{
		Information,
		Warning,
		Error,
	}

	public static class Logger
	{
		public static ILogger Handler = new ConsoleLogger();

		private static object _OutputLock = new object();

		public static void LogWithLevel(string formatString, LogLevel level, params object[] arguments)
		{
			lock (_OutputLock)
			{
				string message = string.Format(formatString, arguments);
                string finalMessage = string.Format("{0} {1}", DateTime.Now, message);
				Handler.Write(finalMessage, level);
			}
		}

		public static void Log(string formatString, params object[] arguments)
		{
			LogWithLevel(formatString, LogLevel.Information, arguments);
		}

		public static void Warning(string formatString, params object[] arguments)
		{
			LogWithLevel(formatString, LogLevel.Warning, arguments);
		}

		public static void Error(string formatString, params object[] arguments)
		{
			LogWithLevel(formatString, LogLevel.Error, arguments);
		}

		public static void Exception(string formatString, Exception exception, params object[] arguments)
		{
			string formattedMessage = string.Format(formatString, arguments);
			Error("{0}: {1} ({2})", formattedMessage, exception.Message, exception.GetType());
		}
	}
}
