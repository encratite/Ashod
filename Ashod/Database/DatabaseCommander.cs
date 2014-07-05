using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Ashod.Database
{
	public class DatabaseCommander : IDisposable
	{
		private DbConnection _Connection;

		public DatabaseCommander(DbConnection connection)
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

		public void NonQuery(string commandText, params CommandParameter[] parameters)
		{
			using (var command = Command(commandText, parameters))
			{
				command.ExecuteNonQuery();
			}
		}

		public void NonQueryFunction(string function, params CommandParameter[] parameters)
		{
			using (var command = Function(function, true, parameters))
			{
				command.ExecuteNonQuery();
			}
		}

		public DatabaseReader Read(string commandText, params CommandParameter[] parameters)
		{
			var command = Command(commandText, parameters);
			var reader = new DatabaseReader(command);
			return reader;
		}

		public DatabaseReader ReadFunction(string function, params CommandParameter[] parameters)
		{
			var command = Function(function, false, parameters);
			var reader = new DatabaseReader(command);
			return reader;
		}

		public ScalarType Scalar<ScalarType>(string commandText, params CommandParameter[] parameters)
		{
			using (var command = Command(commandText, parameters))
			{
				var scalar = command.ExecuteScalar();
				if (scalar == DBNull.Value)
					return default(ScalarType);
				var output = (ScalarType)scalar;
				return output;
			}
		}

		public ScalarType ScalarFunction<ScalarType>(string function, params CommandParameter[] parameters)
		{
			using (var command = Function(function, false, parameters))
			{
				var scalar = command.ExecuteScalar();
				if (scalar == DBNull.Value)
					return default(ScalarType);
				var output = (ScalarType)scalar;
				return output;
			}
		}

		public DbTransaction Transaction()
		{
			return _Connection.BeginTransaction();
		}

		DbCommand Command(string commandText, params CommandParameter[] parameters)
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

		DbCommand Function(string function, bool isScalar, params CommandParameter[] parameters)
		{
			var newParameters = parameters.Select(x => new CommandParameter("@" + x.Name, x.Value)).ToArray();
			string parameterString = string.Join(", ", newParameters.Select(x => x.Name).ToArray());
			string commandText;
			if(isScalar)
				commandText = string.Format("select {0}({1})", function, parameterString);
			else
				commandText = string.Format("select * from {0}({1})", function, parameterString);
			var command = Command(commandText, newParameters);
			return command;
		}
	}
}
