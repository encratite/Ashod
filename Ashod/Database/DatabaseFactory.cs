using System.Data.Common;
using System.Data.SqlClient;

namespace Ashod.Database
{
	public class DatabaseFactory
	{
		private DbConnection _Connection;

		public DatabaseFactory(DbConnection connection)
		{
			_Connection = connection;
		}

		public DbCommand Command(string commandText, params CommandParameter[] parameters)
		{
			var command = _Connection.CreateCommand();
			command.CommandText = commandText;
			foreach (var parameter in parameters)
			{
				var commandParameter = command.CreateParameter();
				commandParameter.ParameterName = parameter.Name;
				commandParameter.Value = parameter.Value;
				command.Parameters.Add(commandParameter);
			}
			return command;
		}

		public DatabaseReader Reader(string commandText, params CommandParameter[] parameters)
		{
			var command = Command(commandText, parameters);
			var reader = new DatabaseReader(command);
			return reader;
		}

		public void NonQuery(string commandText, params CommandParameter[] parameters)
		{
			using (var command = Command(commandText, parameters))
			{
				command.ExecuteNonQuery();
			}
		}
	}
}
