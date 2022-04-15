namespace IotManagerBusiness
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	class DbLayer
	{
		private readonly SqlConnection connection;
		private SqlTransaction transaction;

		public DbLayer(string connStr)
		{
			connection = new SqlConnection(connStr);
			connection.Open();
		}

		public void BeginTransaction() => transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

		public void CommitTransaction()
		{
			transaction.Commit();
			connection.Close();
		}

		public void RollbackTransaction()
		{
			transaction.Rollback();
			connection.Close();
		}

		public void ExecuteCommandWithinTransaction(string commandName, CommandType commandType)
		{
			using (var command = new SqlCommand(commandName, connection, transaction) { CommandType = commandType })
			{
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception)
				{
					throw;
				}
			}
		}

		public DataTable GetDataTable(string commandName, CommandType commandType)
		{
			using (var command = new SqlCommand(commandName, connection) { CommandType = commandType })
			{
				var adapter = new SqlDataAdapter(command);
				var dataSet = new DataSet();

				try
				{
					adapter.Fill(dataSet);
				}
				catch (Exception)
				{
					throw;
				}

				return dataSet.Tables[0];
			}
		}
	}
}