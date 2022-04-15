namespace IotManagerBusiness
{
	using IotManagerBusiness.Enums;

	using System;
	using System.Configuration;
	using System.Data;

	internal class DataOperations
	{
		public string ConnectionString { get; set; }
		public string RF_WIFI_COMPONENTCODE { get; set; }
		public string WM_ELCARD_STATION { get; set; }
		public string CONNECTIVITY_CARD_LENGTH { get; set; }

		public string FilterSQL { get; set; }

		public DataOperations(string env)
		{
			if (env.Equals("PROD"))
			{
				FilterSQL = "AND PRODUCT IN (select distinct SKUNUMBER COLLATE Latin1_General_CI_AS from[ETIKET].[dbo].[T_ATR_VALUE_tr_TR] WITH(NOLOCK) where ATR_CODE = 'F2119_4334977' AND ATR_VALUE_MDM NOT IN('NA', '', 'No')) ";
				ConnectionString = "DBConnectionString";
			}
			else
			{
				FilterSQL = "";
				ConnectionString = "testDBConnectionString";
			}

			RF_WIFI_COMPONENTCODE = ConfigurationManager.AppSettings["RF_WIFI_COMPONENTCODE"];
			WM_ELCARD_STATION = ConfigurationManager.AppSettings["WM_ELCARD_STATION"];
			CONNECTIVITY_CARD_LENGTH = ConfigurationManager.AppSettings["CONNECTIVITY_CARD_LENGTH"];
		}

		public void DeleteFromBarcodeTables(ProductType productType, string cardBarcode)
		{
			var DbL = new DbLayer(ConnectionString);

			var sqlQuery = $"DELETE FROM [dbo].[T_KKTS_CONNECTED_DISPLAY_CARD] WHERE LINE = {(productType.Equals("WM") ? 2 : 1)} AND DISPLAY_CARD = '{cardBarcode}'";
	
			try
			{
				DbL.BeginTransaction();
				DbL.ExecuteCommandWithinTransaction(sqlQuery, CommandType.Text);
				DbL.CommitTransaction();
			}
			catch
			{
				DbL.RollbackTransaction();
				throw;
			}
		}

		public void InsertIntoBarcodeTables(ProductType productType, string newCardBarcode, string productNumber, string productSerial)
		{
			var DbL = new DbLayer(ConnectionString);

			var sqlQuery = "INSERT INTO [dbo].[T_KKTS_CONNECTED_DISPLAY_CARD] (PRODUCT, SERIAL, LINE, DISPLAY_CARD, REGISTER_STATE, REGISTER_TIME) " +
					$"VALUES ('{productNumber}', '{productSerial}',{(productType.Equals("WM") ? "2" : "1")},'{newCardBarcode}',1,GETDATE())";

			try
			{
				DbL.BeginTransaction();
				DbL.ExecuteCommandWithinTransaction(sqlQuery, CommandType.Text);
				DbL.CommitTransaction();
			}
			catch
			{
				DbL.RollbackTransaction();
				throw;
			}
		}

		public void InsertIntoQueueTable(ProductType productType, string cardBarcode, string productNumber, string productSerial, bool isInstant)
		{
			var DbL = new DbLayer(ConnectionString);

			var sqlQuery = "INSERT INTO [dbo].[T_KKTS_CONNECTED_DISPLAY_CARD] (PRODUCT, SERIAL, LINE, DISPLAY_CARD, REGISTER_STATE, REGISTER_TIME) " +
				$"VALUES ('{productNumber}','{productSerial}',{(productType.Equals("WM") ? "2" : "1")},'{cardBarcode}',{(isInstant ? "1" : "0")},GETDATE())";

			try
			{
				DbL.BeginTransaction();
				DbL.ExecuteCommandWithinTransaction(sqlQuery, CommandType.Text);
				DbL.CommitTransaction();
			}
			catch (Exception)
			{
				DbL.RollbackTransaction();
				throw;
			}
		}

		public void UpdateStateDisplayCard(string product, string serial, string card, int state)
		{
			var DbL = new DbLayer(ConnectionString);
			var sqlQuery1 = $"UPDATE [BekoLLCSQL].[dbo].[T_KKTS_CONNECTED_DISPLAY_CARD] SET [REGISTER_STATE] = {state} WHERE [PRODUCT] = '{product}' and [SERIAL] = '{serial}' and [DISPLAY_CARD] = '{card}'";

			try
			{
				DbL.BeginTransaction();
				DbL.ExecuteCommandWithinTransaction(sqlQuery1, CommandType.Text);
				DbL.CommitTransaction();
			}
			catch
			{
				DbL.RollbackTransaction();
			}
		}

		public void LogOperation(string logType, string logStatus, string logText, string logOperator)
		{
			var DbL = new DbLayer(ConnectionString);
			var sqlQuery1 = $" INSERT INTO [dbo].[T_KKTS_IOT_LOG] (LogType, LogStatus, LogText, LogOperator) VALUES ('{logType}', '{logStatus}','{logText}','{logOperator}')";

			try
			{
				DbL.BeginTransaction();
				DbL.ExecuteCommandWithinTransaction(sqlQuery1, CommandType.Text);
				DbL.CommitTransaction();
			}
			catch
			{
				DbL.RollbackTransaction();
			}
		}
		public DataTable GetElectronicCard(string productSerial)
		{
			var productDet = productSerial.Split('#');
			var sqlQuery = string.Empty;

			if (productDet[0].Equals("WM"))
			{
				sqlQuery = "SELECT 'WM' AS [ProductType], PRODUCT AS [ProductNumber], SERIAL AS [ProductSerial]" +
				", NULL AS [ProductCardMaterial], [DISPLAY_CARD] AS [ProductCardBarcode], NULL AS [ProductCardModel] " +
				" FROM [BekoLLCSQL].[dbo].[T_KKTS_CONNECTED_DISPLAY_CARD] WITH (NOLOCK) " +
				$" WHERE [LINE] = 2 AND SERIAL = '{productDet[1]}' AND PRODUCT = '{productDet[2]}'";
			}
			else if (productDet[0].Equals("REF"))
			{
				sqlQuery = "SELECT 'REF' AS [ProductType], [PRODUCT] AS [ProductNumber], [SERIAL] AS [ProductSerial], [MATERIAL] AS [ProductCardMaterial]" +
				", [BARCODE] AS [ProductCardBarcode], [MODEL] AS [ProductCardModel] " +
				" FROM [BekoLLCSQL].[dbo].[KKI_MATCH] WITH (NOLOCK) " +
				$" WHERE COMPONENTCODE = '{RF_WIFI_COMPONENTCODE}' AND LEN(BARCODE) = {CONNECTIVITY_CARD_LENGTH} AND SERIAL = '{productDet[1]}' AND PRODUCT = '{productDet[2]}'";
			}

			var DbL = new DbLayer(ConnectionString);
			return DbL.GetDataTable(sqlQuery, CommandType.Text);
		}

		public DataTable GetProductData(string productNumber)
		{
			var DbL = new DbLayer(ConnectionString);
			return DbL.GetDataTable($"SELECT [SKUNUMBER],[RECORDID],[ATR_TYPE],[ATR_CODE],[ATR_VALUE_MDM] FROM [ETIKET].[dbo].[T_ATR_VALUE_tr_TR] WITH (NOLOCK) " +
				$"WHERE SKUNUMBER = '{productNumber}' AND ATR_CODE in ('F481_605','F2119_4334977')", CommandType.Text);
		}
	}
}