namespace Ashod
{
	public interface ILogger
	{
		void Write(string message, LogLevel level);
	}
}
