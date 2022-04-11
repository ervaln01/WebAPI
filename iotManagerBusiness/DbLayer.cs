namespace IotManagerBusiness
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;

	class DbLayer
	{
		private readonly SqlConnection conn;
		private SqlTransaction trans;
		private SqlDataAdapter sda;
		private SqlCommand com;
		private SqlBulkCopy sbc;
		private DataSet ds;
		private DataTable dt;

		public DbLayer(string connStr)
		{
			conn = new SqlConnection(connStr);
			conn.Open();
		}

		public void BeginTransaction() => trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);

		public void CommitTransaction()
		{
			trans.Commit();
			CloseConnection();
		}

		public void RollbackTransaction()
		{
			trans.Rollback();
			CloseConnection();
		}

		public void CloseConnection() => conn.Close();

		public void ExecuteCommandWithinTransaction(string commandName, CommandType commandType, DbParam[] dbParamList = null)
		{
			com = new SqlCommand(commandName, conn, trans) { CommandType = commandType };

			dbParamList?.ToList().ForEach(param => com.Parameters.Add(new SqlParameter() { ParameterName = param.ParamName, Value = param.ParamValue, SqlDbType = param.ParamType }));

			try
			{
				com.ExecuteNonQuery();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public object ExecuteScalarWithinTransaction(string commandName, CommandType commandType, DbParam[] dbParamList = null)
		{
			com = new SqlCommand(commandName, conn, trans) { CommandType = commandType };

			dbParamList?.ToList().ForEach(param => com.Parameters.Add(new SqlParameter() { ParameterName = param.ParamName, Value = param.ParamValue, SqlDbType = param.ParamType }));

			sda = new SqlDataAdapter(com);
			dt = new DataTable();

			try
			{
				sda.Fill(dt);
			}
			catch
			{
				throw;
			}

			return dt.Rows[0][0];
		}

		public DataTable GetDataTable(string commandName, CommandType commandType, DbParam[] dbParamList = null)
		{
			com = new SqlCommand(commandName, conn) { CommandType = commandType };

			dbParamList?.ToList().ForEach(param => com.Parameters.Add(new SqlParameter()
			{
				ParameterName = param.ParamName,
				Value = param.ParamValue,
				SqlDbType = param.ParamType
			}));

			sda = new SqlDataAdapter(com);
			ds = new DataSet();

			try
			{
				sda.Fill(ds);
			}
			catch (Exception)
			{
				throw;
			}

			return ds.Tables[0];
		}

		public void BulkInsert(DataTable dataTableToInsert, string tableName)
		{
			try
			{
				sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, trans) { DestinationTableName = tableName };
				sbc.WriteToServer(dataTableToInsert);
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}