using System;
using System.Data.Common;

namespace Ashod.Database
{
	public class DatabaseReader: IDisposable
	{
		private DbCommand _Command;
		private DbDataReader _Reader;

		public DatabaseReader(DbCommand command)
		{
			_Command = command;
			_Reader = command.ExecuteReader();
		}

		void IDisposable.Dispose()
		{
			_Reader.Dispose();
			_Command.Dispose();
		}

		public bool Read()
		{
			return _Reader.Read();
		}

		public bool GetBoolean(string column)
		{
			int ordinal = _Reader.GetOrdinal(column);
			return _Reader.GetBoolean(ordinal);
		}

		public int GetInt32(string column)
		{
			int ordinal = _Reader.GetOrdinal(column);
			return _Reader.GetInt32(ordinal);
		}

		public string GetString(string column)
		{
			int ordinal = _Reader.GetOrdinal(column);
			return _Reader.GetString(ordinal);
		}
	}
}
