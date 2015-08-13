﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

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
            return _Reader[column] as string;
		}

		public List<RowType> ReadAll<RowType>()
			where RowType : new()
		{
			var output = new List<RowType>();
			var propertyMap = new Dictionary<string, PropertyInfo>();
			foreach (var propertyInfo in typeof(RowType).GetProperties())
			{
				string internalName = GetInternalName(propertyInfo.Name);
				propertyMap[internalName] = propertyInfo;
			}
			while (_Reader.Read())
			{
				var row = new RowType();
				for (int i = 0; i < _Reader.FieldCount; i++)
				{
					string fieldName = _Reader.GetName(i);
					string internalName = GetInternalName(fieldName);
					PropertyInfo propertyInfo;
					if (!propertyMap.TryGetValue(internalName, out propertyInfo))
					{
						string error = string.Format("Type \"{0}\" lacks a corresponding property for column \"{1}\"", typeof(RowType), fieldName);
						throw new ApplicationException(error);
					}
					var value = _Reader[i];
					if(value == DBNull.Value)
						value = null;
					propertyInfo.SetValue(row, value);
				}
				output.Add(row);
			}
			return output;
		}

		string GetInternalName(string name)
		{
			string output = name.Replace("_", "");
			output = output.ToLower();
			return output;
		}
	}
}
