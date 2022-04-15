namespace IotManagerBusiness
{
	using IotManagerBusiness.Enums;
	using IotManagerBusiness.Exceptions;

	using System;
	using System.Linq;

	public class BusinessOperations
	{
		public string Environment { get; set; }

		public BusinessOperations(string env) => Environment = env;

		public void DeAttachElectronicCard(ProductType productType, string productNumber, string cardBarcode, string productSerial, string operatingUser)
		{
			var dop = new DataOperations(Environment);

			try
			{
				var sop = new ServiceOperations(Environment);
				var res = sop.DeAttachProductCardData(productNumber, cardBarcode, productSerial);
				if (res.Status != StatusType.Success) throw new AzureException($"Error during deattachment. Status code: {res.Status} - Message: {res.Message}");

				dop.DeleteFromBarcodeTables(productType, cardBarcode);
				dop.LogOperation("DeAttach", "S", $"Deattach successful. Card: {cardBarcode}- Product: {productSerial}", operatingUser);
			}
			catch (Exception ex)
			{
				dop.LogOperation("DeAttach", "E", $"Deattach Error. Card: {cardBarcode}- Product: {productSerial} ERR:{ex.Message}", operatingUser);
				throw;
			}
		}

		public void AttachElectronicCard(ProductType productType, string newCardBarcode, string productNumber, string productSerial, string operatingUser, bool cardExistsinDB)
		{
			var dop = new DataOperations(Environment);

			try
			{
				var productData = GetProductDetails(productNumber);
				var sop = new ServiceOperations(Environment);
				var res = sop.AttachProductCardData(productType, productNumber, newCardBarcode, productSerial, productData[0], productData[1]);
				if (res.Status != StatusType.Success) throw new AzureException($"Error during attachment. Status code: {res.Status} - Message: {res.Message}");

				if (cardExistsinDB)
				{
					dop.InsertIntoQueueTable(productType, newCardBarcode, productNumber, productSerial, true);
				}
				else
				{
					dop.InsertIntoBarcodeTables(productType, newCardBarcode, productNumber, productSerial);
				}

				dop.LogOperation("Attach", "S", $"Attach successful. Card: {newCardBarcode}- Product: {productSerial}", operatingUser);
			}
			catch (Exception ex)
			{
				dop.LogOperation("Attach", "E", $"Attach Error. Card: {newCardBarcode}- Product: {productSerial} ERR:{ex.Message}", operatingUser);
				throw;
			}
		}

		public void AttachElectronicCardFromProduction(ProductType productType, string productNumber, string productSerial, IntegrationType integrationType, string operatingUser)
		{
			var dop = new DataOperations(Environment);

			try
			{
				var cardData = GetCardDetails(productType, productNumber, productSerial);
				var productData = GetProductDetails(cardData[0]);

				switch (integrationType)
				{
					case IntegrationType.Instant:
						var sop = new ServiceOperations(Environment);
						var res = sop.AttachProductCardData(productType, cardData[0], cardData[2], productSerial, productData[0], productData[1]);
						if (res.Status != StatusType.Success) throw new AzureException($"Error during attachment. Status code: {res.Status} - Message: {res.Message}");

						dop.InsertIntoQueueTable(productType, cardData[2], cardData[0], productSerial, true);
						break;
					case IntegrationType.Queue:
						dop.InsertIntoQueueTable(productType, cardData[2], cardData[0], productSerial, false);
						break;
					default:
						throw new Exception("Undefined integration type.");
				}

				dop.LogOperation("AttachFromProduction", "S", $"Attach successful. Card: {cardData[2]}- Product: {productSerial}", operatingUser);
			}
			catch (Exception ex)
			{
				dop.LogOperation("AttachFromProduction", "E", $"Attach Error. Product: {productSerial} ERR:{ex.Message}", operatingUser);
				throw;
			}
		}

		public void UpdateErrorDisplayCard(ProductType productType, string product, string serial, string operatingUser, string card = "")
		{
			var dop = new DataOperations(Environment);
			if (string.IsNullOrEmpty(card)) 
				card = GetCardDetails(productType, product, serial)[2];

			try
			{
				dop.UpdateStateDisplayCard(product, serial, card, 2);
				dop.LogOperation("UpdateErrorDisplayCard", "S", $"Update successful. Product: {product}", operatingUser);
			}
			catch (Exception ex)
			{
				dop.LogOperation("UpdateErrorDisplayCard", "E", "UpdateErrorDisplayCard Error.", ex.Message);
			}
		}

		public string[] GetProductDetails(string productNumber)
		{
			var dop = new DataOperations(Environment);

			try
			{
				var productDT = dop.GetProductData(productNumber);
				if (productDT.Rows.Count != 2)
				{
					throw new Exception("Product connectivitiy or brand info not found.");
				}
				else
				{
					var productData = new string[]
					{
						ConvertToCamelCase(productDT.Select("ATR_CODE = 'F481_605'").FirstOrDefault()?["ATR_VALUE_MDM"].ToString()),
						productDT.Select("ATR_CODE = 'F2119_4334977'").FirstOrDefault()?["ATR_VALUE_MDM"].ToString()
					};

					return new string[] { string.Empty, "NA", "No" }.Contains(productData[1]) ?
						throw new Exception("Product is not a connected model.") :
						productData;
				}
			}
			catch (Exception ex)
			{
				dop.LogOperation("GetProductDetails", "E", "Product data not found.", ex.Message);
				throw;
			}
		}
		public string[] GetCardDetails(ProductType productType, string productNumber, string productSerial)
		{
			var dop = new DataOperations(Environment);

			try
			{
				var cardDT = dop.GetElectronicCard($"{productType}#{productSerial}#{productNumber}");

				return cardDT.Rows.Count != 1 ?
					throw new Exception(cardDT.Rows.Count == 0 ? "Card info not found." : "Multiple card info found.") :
					new string[]
					{
						cardDT.Rows[0]["ProductNumber"].ToString(),
						cardDT.Rows[0].IsNull("ProductCardMaterial") ? string.Empty : cardDT.Rows[0]["ProductCardMaterial"].ToString(),
						cardDT.Rows[0]["ProductCardBarcode"].ToString(),
						cardDT.Rows[0].IsNull("ProductCardModel") ? string.Empty : cardDT.Rows[0]["ProductCardModel"].ToString()
					};
			}
			catch (Exception ex)
			{
				dop.LogOperation("GetCardDetails", "E", "Card data not found.", ex.Message);
				throw;
			}
		}

		public bool IsProductConnected(string productNumber)
		{
			GetProductDetails(productNumber);
			return true;
		}

		private string ConvertToCamelCase(string inputString) =>
			string.IsNullOrEmpty(inputString) || inputString.Length < 2 ? inputString.ToUpper() : char.ToUpperInvariant(inputString[0]) + inputString.Substring(1).ToLowerInvariant();
	}
}