using System;

namespace Ashod
{
	public class ConsoleLogger : ILogger
	{
		void ILogger.Write(string message, LogLevel level)
		{
			ConsoleColor? colour = null;
			switch(level)
			{
				case LogLevel.Warning:
					colour = ConsoleColor.Yellow;
					break;

				case LogLevel.Error:
					colour = ConsoleColor.Red;
					break;
			}
			var originalColour = Console.ForegroundColor;
			if (colour != null)
				Console.ForegroundColor = colour.Value;
			Console.WriteLine(message);
			Console.ForegroundColor = originalColour;
		}
	}
}
