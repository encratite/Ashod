namespace Ashod.WebSocket
{
	class ResultMessage
	{
		public readonly int? Id;
		public readonly object Result;
		public readonly string Error;

		ResultMessage(int? id, object result, string error)
		{
			Id = id;
			Result = result;
			Error = error;
		}

		public static ResultMessage Success(int? id, object result)
		{
			return new ResultMessage(id, result, null);
		}

		public static ResultMessage Exception(int? id, string error)
		{
			return new ResultMessage(id, null, error);
		}
	}
}
