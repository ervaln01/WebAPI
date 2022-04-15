namespace IotManagerBusiness
{
	using IotManagerBusiness.Configuration;
	using IotManagerBusiness.Entities.BekoLLC;
	using IotManagerBusiness.Entities.Etiket;
	using IotManagerBusiness.Enums;

	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class DataOperations
	{
		private readonly List<string> AtrCodes = new List<string> { "F481_605", "F2119_4334977" };

		private readonly string ConnectionString;

		public DataOperations(string env) => ConnectionString = env.Equals("PROD") ? "DBConnectionString" : "testDBConnectionString";

		public void DeleteFromBarcodeTables(ProductType productType, string cardBarcode) => SafeExecuteSqlRequest(context =>
			context.TKktsConnectedDisplayCards.Where(x => x.Line == (int)productType && x.DisplayCard.Equals(cardBarcode)).AsParallel().ForAll(x => context.TKktsConnectedDisplayCards.Remove(x)));

		public void InsertIntoBarcodeTables(ProductType productType, string card, string product, string serial) => SafeExecuteSqlRequest(context =>
			context.TKktsConnectedDisplayCards.Add(new TKktsConnectedDisplayCard(product, serial, (int)productType, card, 1)));

		public void InsertIntoQueueTable(ProductType productType, string card, string product, string serial, bool isInstant) => SafeExecuteSqlRequest(context =>
			context.TKktsConnectedDisplayCards.Add(new TKktsConnectedDisplayCard(product, serial, (int)productType, card, isInstant ? 1 : 0)));

		public void UpdateStateDisplayCard(string product, string serial, string card, int state) => SafeExecuteSqlRequest(context =>
		{
			var displayCard = context.TKktsConnectedDisplayCards.FirstOrDefault(x => x.Product.Equals(product) && x.Serial.Equals(serial) && x.DisplayCard.Equals(card));
			if (displayCard != null)
				displayCard.RegisterState = state;
		});

		public void LogOperation(string logType, string logStatus, string logText, string logOperator) => SafeExecuteSqlRequest(context =>
			context.Database.ExecuteSqlCommand($"INSERT INTO [dbo].[T_KKTS_IOT_LOG] (LogType, LogStatus, LogText, LogOperator) VALUES ('{logType}', '{logStatus}','{logText}','{logOperator}')"));

		public Dictionary<string, string> GetElectronicCard(ProductType type, string product, string serial)
		{
			var dict = new Dictionary<string, string>()
			{
				{"ProductNumber", string.Empty},
				{"ProductCardMaterial", string.Empty},
				{"ProductCardBarcode", string.Empty},
				{"ProductCardModel", string.Empty}
			};

			using (var context = new BekoLLCSQLContext(ConnectionString))
			{
				switch (type)
				{
					case ProductType.REF:
						var rf = context.KkiMatches.Where(x => x.Componentcode.Equals(Settings.RFWifiCode) && x.Barcode.Length == Settings.CardLength && x.Serial.Equals(serial) && x.Product.Equals(product));
						var rfCount = rf.Count();
						if (rfCount != 1)
							throw new Exception(rfCount == 0 ? "Card info not found." : "Multiple card info found.");

						var match = rf.First();
						if (match.Product != null) dict["ProductNumber"] = match.Product;
						if (match.Material != null) dict["ProductCardMaterial"] = match.Material;
						if (match.Barcode != null) dict["ProductCardBarcode"] = match.Barcode;
						if (match.Model != null) dict["ProductCardModel"] = match.Model;
						return dict;
					case ProductType.WM:
						var wm = context.TKktsConnectedDisplayCards.Where(x => x.Serial.Equals(serial) && x.Product.Equals(product));
						var wmCount = wm.Count();
						if (wmCount != 1)
							throw new Exception(wmCount == 0 ? "Card info not found." : "Multiple card info found.");

						var card = wm.First();
						if (card.Product != null) dict["ProductNumber"] = card.Product;
						if (card.DisplayCard != null) dict["ProductCardBarcode"] = card.DisplayCard;
						return dict;

					default:
						return dict;
				}
			}
		}

		public List<TAtrValueTrTr> GetProductData(string productNumber)
		{
			using (var context = new EtiketContext(ConnectionString))
			{
				var value = context.TAtrValueTrTrs.Where(x => x.SkuNumber.Equals(productNumber) && AtrCodes.Contains(x.AtrCode));
				return value.ToList();
			}
		}

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
	}
}