namespace IotManagerAPI
{
	using IotManagerBusiness;

	using System;
	using System.Configuration;
	using System.Threading.Tasks;
	using System.Web.Services;

	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class IotManagerService : WebService
	{

		[WebMethod]
		public IotManagerRespose AttachElectronicCard(ProductType productType, string productNumber, string productSerial, string cardMaterial, string cardBarcode, string cardModel, string operatingUser)
		{
			var bop = new BusinessOperations(ConfigurationManager.AppSettings["ENV"].ToString());

			try
			{
				bop.AttachElectronicCard(Enum.GetName(typeof(ProductType), productType), cardMaterial, cardBarcode, cardModel, productNumber, productSerial, operatingUser, false);
				return new IotManagerRespose() { Result = true, Message = string.Empty };
			}
			catch (Exception ex)
			{
				if (ex is AzureException)
				{
					bop.UpdateErrorDisplayCard(Enum.GetName(typeof(ProductType), productType), productNumber, productSerial, operatingUser, cardBarcode);
					var message = $"Не удалось отправить в Azure данные по продукту product {productNumber}, serial {productSerial}.<br>" +
						$"Дата записи {DateTime.Now:dd.MM.yyyy HH:mm:ss}.<br>" +
						$"Требуется отправить данные вручную.<br><br>" +
						$"-----------------------------------<br><br>" +
						$"Информация для разработчиков:<br>" +
						$"Method AttachElectronicCard, productType {productType}, cardMaterial {cardMaterial}, cardBarcode {cardBarcode}, cardModel {cardModel}, operatingUser {operatingUser}<br>" +
						$"Exception message:<br>{ex.Message}<br>" +
						$"StackTrace:<br>{ex.StackTrace}"; 
					Task.Run(() => SendMail(productType, message));
				}
				return new IotManagerRespose() { Result = false, Message = ex.Message };
			}
		}

		[WebMethod]
		public IotManagerRespose DeAttachElectronicCard(ProductType productType, string productNumber, string cardBarcode, string productSerial, string operatingUser)
		{
			var bop = new BusinessOperations(ConfigurationManager.AppSettings["ENV"].ToString());

			try
			{
				bop.DeAttachElectronicCard(Enum.GetName(typeof(ProductType), productType), productNumber, cardBarcode, productSerial, operatingUser);
				return new IotManagerRespose() { Result = true, Message = string.Empty };
			}
			catch (Exception ex)
			{
				if (ex is AzureException)
				{
					var message = $"Не удалось отправить в Azure данные по продукту product {productNumber}, serial {cardBarcode}.<br>" +
						$"Дата записи {DateTime.Now:dd.MM.yyyy HH:mm:ss}.<br>" +
						$"Требуется отправить данные вручную.<br><br>" +
						$"-----------------------------------<br><br>" +
						$"Информация для разработчиков:<br>" +
						$"Method DeAttachElectronicCard, productType {productType}, cardBarcode {cardBarcode}, operatingUser {operatingUser}<br>" +
						$"Exception message:<br>{ex.Message}<br>" +
						$"StackTrace:<br>{ex.StackTrace}";
					Task.Run(() => SendMail(productType, message));
				}
				return new IotManagerRespose() { Result = false, Message = ex.Message };
			}
		}

		[WebMethod]
		public IotManagerRespose AttachElectronicCardFromProduction(ProductType productType, string productNumber, string productSerial, IntegrationType integrationType, string operatingUser)
		{
			var bop = new BusinessOperations(ConfigurationManager.AppSettings["ENV"].ToString());

			try
			{
				bop.AttachElectronicCardFromProduction(Enum.GetName(typeof(ProductType), productType), productNumber, productSerial, Enum.GetName(typeof(IntegrationType), integrationType), operatingUser);
				return new IotManagerRespose() { Result = true, Message = string.Empty };
			}
			catch (Exception ex)
			{
				if (ex is AzureException)
				{
					bop.UpdateErrorDisplayCard(Enum.GetName(typeof(ProductType), productType), productNumber, productSerial, operatingUser);
					var message = $"Не удалось отправить в Azure данные по продукту product {productNumber}, serial {productSerial}.<br>" +
						$"Дата записи {DateTime.Now:dd.MM.yyyy HH:mm:ss}.<br>" +
						$"Требуется отправить данные вручную.<br><br>" +
						$"-----------------------------------<br><br>" +
						$"Информация для разработчиков:<br>" +
						$"Method AttachElectronicCardFromProduction, productType {productType}, integrationType {integrationType}, operatingUser {operatingUser}<br>" +
						$"Exception message:<br>{ex.Message}<br>" +
						$"StackTrace:<br>{ex.StackTrace}";
					Task.Run(() => SendMail(productType, message));
				}

				return new IotManagerRespose() { Result = false, Message = ex.Message };
			}
		}

		[WebMethod]
		public IotManagerRespose IsProductConnected(string productNumber)
		{
			var bop = new BusinessOperations(ConfigurationManager.AppSettings["ENV"].ToString());

			try
			{
				return new IotManagerRespose() { Result = bop.IsProductConnected(productNumber), Message = string.Empty };
			}
			catch (Exception ex)
			{
				return new IotManagerRespose() { Result = false, Message = ex.Message };
			}
		}

		private void SendMail(ProductType type, string message)
		{
			try
			{
				var to = "example@mail.ru";
				using (var client_message = new EmailSender.SendMessageSoapClient())
					client_message.SendEmail_v2("no-reaply@mail.ru", $"IotManagerApi_{type}", to, "Upload to Azure failed", message, "", "", "", 0, "IotManagerApi");
			}
			catch { }
		}

		public class IotManagerRespose
		{
			public bool Result { get; set; }
			public string Message { get; set; }
		}

		public enum ProductType
		{
			WM,
			REF
		}

		public enum IntegrationType
		{
			Instant,
			Queue
		}
	}
}