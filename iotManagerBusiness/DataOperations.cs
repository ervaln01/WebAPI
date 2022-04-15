namespace IotManagerBusiness
{
	using IotManagerBusiness.Configuration;
	using IotManagerBusiness.Entities.BekoLLC;
	using IotManagerBusiness.Enums;

	using System;
	using System.Data;
	using System.Linq;

	internal class DataOperations
	{
		public string ConnectionString { get; set; }

		public DataOperations(string env) => ConnectionString = env.Equals("PROD") ? "DBConnectionString" : "testDBConnectionString";

		private void SafeExecuteSqlRequest(Action<BekoLLCSQLContext> action)
		{
			try
			{
				using (var context = new BekoLLCSQLContext(ConnectionString))
				{
					action(context);
					context.SaveChanges();
				}
			}
			catch 
			{
				// TODO: Add logging
			}
		}

		public void DeleteFromBarcodeTables(ProductType productType, string cardBarcode) => SafeExecuteSqlRequest(context =>
		{
			foreach (var row in context.TKktsConnectedDisplayCards.Where(x => x.Line == (int)productType && x.DisplayCard.Equals(cardBarcode)))
			{
				context.TKktsConnectedDisplayCards.Remove(row);
			}
		});

		public void InsertIntoBarcodeTables(ProductType productType, string card, string product, string serial) => SafeExecuteSqlRequest(context =>
		{
			var displayCard = new TKktsConnectedDisplayCard(product, serial, (int)productType, card, 1);
			context.TKktsConnectedDisplayCards.Add(displayCard);
		});

		public void InsertIntoQueueTable(ProductType productType, string card, string product, string serial, bool isInstant) => SafeExecuteSqlRequest(context =>
		{
			var displayCard = new TKktsConnectedDisplayCard(product, serial, (int)productType, card, isInstant ? 1 : 0);
			context.TKktsConnectedDisplayCards.Add(displayCard);
		});

		public void UpdateStateDisplayCard(string product, string serial, string card, int state) => SafeExecuteSqlRequest(context =>
		{
			var displayCard = context.TKktsConnectedDisplayCards.FirstOrDefault(x => x.Product.Equals(product) && x.Serial.Equals(serial) && x.DisplayCard.Equals(card));
			if (displayCard != null)
			{
				displayCard.RegisterState = state;
				context.SaveChanges();
			}
		});

		public void LogOperation(string logType, string logStatus, string logText, string logOperator) => SafeExecuteSqlRequest(context =>
			context.Database.ExecuteSqlCommand($"INSERT INTO [dbo].[T_KKTS_IOT_LOG] (LogType, LogStatus, LogText, LogOperator) VALUES ('{logType}', '{logStatus}','{logText}','{logOperator}')"));

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
				$" WHERE COMPONENTCODE = '{Settings.RFWifiCode}' AND LEN(BARCODE) = {Settings.CardLength} AND SERIAL = '{productDet[1]}' AND PRODUCT = '{productDet[2]}'";
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