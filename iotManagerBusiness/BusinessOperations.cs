namespace IotManagerBusiness
{
	using IotManagerBusiness.Enums;
	using IotManagerBusiness.Exceptions;

	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class BusinessOperations
	{
		private readonly string Environment;

		public BusinessOperations(string env) => Environment = env;

		public void DeAttachElectronicCard(ProductType productType, string product, string card, string serial, string user) => SafeAttaching(operation =>
		{
			var sop = new ServiceOperations(Environment);
			var res = sop.DeAttachProductCardData(product, card, serial);
			if (res.Status != StatusType.Success)
				throw new AzureException($"Error during deattachment. Status code: {res.Status} - Message: {res.Message}");

			operation.DeleteFromBarcodeTables(productType, card);
		}, "DeAttach", card, serial, user);

		public void AttachElectronicCard(ProductType productType, string newCardBarcode, string productNumber, string productSerial, string operatingUser, bool cardExistsinDB) => SafeAttaching(operation =>
		{
			var productData = GetProductDetails(productNumber);
			var sop = new ServiceOperations(Environment);
			var res = sop.AttachProductCardData(productType, productNumber, newCardBarcode, productSerial, productData[0], productData[1]);
			if (res.Status != StatusType.Success)
				throw new AzureException($"Error during attachment. Status code: {res.Status} - Message: {res.Message}");

			if (cardExistsinDB)
			{
				operation.InsertIntoQueueTable(productType, newCardBarcode, productNumber, productSerial, true);
				return;
			}
			
			operation.InsertIntoBarcodeTables(productType, newCardBarcode, productNumber, productSerial);
		}, "Attach", newCardBarcode, productSerial, operatingUser);

		public void AttachElectronicCardFromProduction(ProductType productType, string productNumber, string productSerial, IntegrationType integrationType, string operatingUser) => SafeAttaching(operation =>
		{
			var cardData = GetCardDetails(productType, productNumber, productSerial);
			var productData = GetProductDetails(cardData["ProductNumber"]);

			switch (integrationType)
			{
				case IntegrationType.Instant:
					var sop = new ServiceOperations(Environment);
					var res = sop.AttachProductCardData(productType, cardData["ProductNumber"], cardData["ProductCardBarcode"], productSerial, productData[0], productData[1]);
					if (res.Status != StatusType.Success) 
						throw new AzureException($"Error during attachment. Status code: {res.Status} - Message: {res.Message}");

					operation.InsertIntoQueueTable(productType, cardData["ProductCardBarcode"], cardData["ProductNumber"], productSerial, true);
					break;
				case IntegrationType.Queue:
					operation.InsertIntoQueueTable(productType, cardData["ProductCardBarcode"], cardData["ProductNumber"], productSerial, false);
					break;
				default:
					throw new Exception("Undefined integration type.");
			}
		}, "AttachFromProduction", string.Empty, productNumber, operatingUser);

		public void UpdateErrorDisplayCard(ProductType productType, string product, string serial, string operatingUser, string card = "") => SafeAttaching(operation =>
		{
			if (string.IsNullOrEmpty(card))
				card = GetCardDetails(productType, product, serial)["ProductCardBarcode"];

			operation.UpdateStateDisplayCard(product, serial, card, 2);
		}, "UpdateErrorDisplayCard", card, product, operatingUser);

		public string[] GetProductDetails(string productNumber)
		{
			var dop = new DataOperations(Environment);

			try
			{
				var productDT = dop.GetProductData(productNumber);
				if (productDT.Count != 2)
					throw new Exception("Product connectivitiy or brand info not found.");


				var productData = new string[]
				{
					ConvertToCamelCase(productDT.FirstOrDefault(x => x.AtrCode.Equals("F481_605"))?.AtrValueMdm),
					productDT.FirstOrDefault(x => x.AtrCode.Equals("F2119_4334977"))?.AtrValueMdm
				};

				return new string[] { string.Empty, "NA", "No" }.Contains(productData[1]) ?
					throw new Exception("Product is not a connected model.") :
					productData;
			}
			catch (Exception ex)
			{
				dop.LogOperation("GetProductDetails", "E", "Product data not found.", ex.Message);
				throw;
			}
		}
		public bool IsProductConnected(string productNumber)
		{
			GetProductDetails(productNumber);
			return true;
		}

		private void SafeAttaching(Action<DataOperations> operation, string method, string card, string product, string user)
		{
			var dop = new DataOperations(Environment);
			try
			{
				operation(dop);
				dop.LogOperation(method, "S", $"{method} successful. Card: {card} - Product: {product}", user);
			}
			catch (Exception ex)
			{
				dop.LogOperation(method, "E", $"{method} error. Card: {card} - Product: {product} - Error:{ex.Message}", user);
				throw;
			}
		}

		private Dictionary<string, string> GetCardDetails(ProductType productType, string productNumber, string productSerial)
		{
			var dop = new DataOperations(Environment);

			try
			{
				var cardDT = dop.GetElectronicCard(productType, productSerial,productNumber);
				return cardDT;
			}
			catch (Exception ex)
			{
				dop.LogOperation("GetCardDetails", "E", "Card data not found.", ex.Message);
				throw;
			}
		}

		private string ConvertToCamelCase(string inputString) =>
			string.IsNullOrEmpty(inputString) || inputString.Length < 2 ? inputString.ToUpper() : char.ToUpperInvariant(inputString[0]) + inputString.Substring(1).ToLowerInvariant();
	}
}