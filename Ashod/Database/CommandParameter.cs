using System;
namespace Ashod.Database
{
	public class CommandParameter
	{
		public readonly string Name;
		public readonly object Value;

		public CommandParameter(string name, object value)
		{
			Name = name;
			if (value == null)
				Value = DBNull.Value;
			else
				Value = value;
		}
	}
}
