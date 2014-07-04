using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Ashod.Database
{
	public class DatabaseFactory : IDisposable
	{
		private DbConnection _Connection;

		public DatabaseFactory(DbConnection connection)
		{
			_Connection = connection;
			if (_Connection.State != ConnectionState.Open)
			{
				try
				{
					_Connection.Open();
				}
				catch
				{
					_Connection.Dispose();
					throw;
				}
			}
		}

		public void Dispose()
		{
			if (_Connection != null)
			{
				_Connection.Dispose();
				_Connection = null;
			}
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

		public DbCommand Function(string function, params CommandParameter[] parameters)
		{
			var newParameters = parameters.Select(x => new CommandParameter("@" + x.Name, x.Value)).ToArray();
			string parameterString = string.Join(", ", newParameters.Select(x => x.Name).ToArray());
			string commandText = string.Format("select {0}({1})", function, parameterString);
			var command = Command(commandText, newParameters);
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

		public void NonQueryFunction(string function, params CommandParameter[] parameters)
		{
			using (var command = Function(function, parameters))
			{
				command.ExecuteNonQuery();
			}
		}

		public DbTransaction Transaction()
		{
			return _Connection.BeginTransaction();
		}
	}
}
