namespace IotManagerAPI
{
	using IotManagerBusiness;
	using IotManagerBusiness.Enums;
	using IotManagerBusiness.Exceptions;
	using IotManagerBusiness.Models;

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
		public Response<bool> AttachElectronicCard(ProductType productType, string productNumber, string productSerial, string cardMaterial, string cardBarcode, string cardModel, string operatingUser)
		{
			var bop = new BusinessOperations(ConfigurationManager.AppSettings["ENV"].ToString());

			try
			{
				bop.AttachElectronicCard(productType, cardBarcode, productNumber, productSerial, operatingUser, false);
				return new Response<bool>(true, StatusType.Success, string.Empty);
			}
			catch (AzureException ex)
			{
				bop.UpdateErrorDisplayCard(productType, productNumber, productSerial, operatingUser, cardBarcode);
				var message = $"Не удалось отправить в Azure данные по продукту product {productNumber}, serial {productSerial}.<br>" +
					$"Дата записи {DateTime.Now:dd.MM.yyyy HH:mm:ss}.<br>" +
					$"Требуется отправить данные вручную.<br><br>" +
					$"-----------------------------------<br><br>" +
					$"Информация для разработчиков:<br>" +
					$"Method AttachElectronicCard, productType {productType}, cardMaterial {cardMaterial}, cardBarcode {cardBarcode}, cardModel {cardModel}, operatingUser {operatingUser}<br>" +
					$"Exception message:<br>{ex.Message}<br>" +
					$"StackTrace:<br>{ex.StackTrace}";
				Task.Run(() => MailSender.SendMail(productType, message));
				return new Response<bool>(false, StatusType.Error, ex.Message);
			}
			catch (Exception ex)
			{
				return new Response<bool>(false, StatusType.Error, ex.Message);
			}
		}

		[WebMethod]
		public Response<bool> DeAttachElectronicCard(ProductType productType, string productNumber, string cardBarcode, string productSerial, string operatingUser)
		{
			var bop = new BusinessOperations(ConfigurationManager.AppSettings["ENV"].ToString());

			try
			{
				bop.DeAttachElectronicCard(productType, productNumber, cardBarcode, productSerial, operatingUser);
				return new Response<bool>(true, StatusType.Success, string.Empty);
			}
			catch (AzureException ex)
			{
				var message = $"Не удалось отправить в Azure данные по продукту product {productNumber}, serial {cardBarcode}.<br>" +
					$"Дата записи {DateTime.Now:dd.MM.yyyy HH:mm:ss}.<br>" +
					$"Требуется отправить данные вручную.<br><br>" +
					$"-----------------------------------<br><br>" +
					$"Информация для разработчиков:<br>" +
					$"Method DeAttachElectronicCard, productType {productType}, cardBarcode {cardBarcode}, operatingUser {operatingUser}<br>" +
					$"Exception message:<br>{ex.Message}<br>" +
					$"StackTrace:<br>{ex.StackTrace}";
				Task.Run(() => MailSender.SendMail(productType, message));
				return new Response<bool>(false, StatusType.Error, ex.Message);
			}
			catch (Exception ex)
			{
				return new Response<bool>(false, StatusType.Error, ex.Message);
			}
		}

		[WebMethod]
		public Response<bool> AttachElectronicCardFromProduction(ProductType productType, string productNumber, string productSerial, IntegrationType integrationType, string operatingUser)
		{
			var bop = new BusinessOperations(ConfigurationManager.AppSettings["ENV"].ToString());

			try
			{
				bop.AttachElectronicCardFromProduction(productType, productNumber, productSerial, integrationType, operatingUser);
				return new Response<bool>(true, StatusType.Success, string.Empty);
			}
			catch (AzureException ex)
			{
				bop.UpdateErrorDisplayCard(productType, productNumber, productSerial, operatingUser);
				var message = $"Не удалось отправить в Azure данные по продукту product {productNumber}, serial {productSerial}.<br>" +
					$"Дата записи {DateTime.Now:dd.MM.yyyy HH:mm:ss}.<br>" +
					$"Требуется отправить данные вручную.<br><br>" +
					$"-----------------------------------<br><br>" +
					$"Информация для разработчиков:<br>" +
					$"Method AttachElectronicCardFromProduction, productType {productType}, integrationType {integrationType}, operatingUser {operatingUser}<br>" +
					$"Exception message:<br>{ex.Message}<br>" +
					$"StackTrace:<br>{ex.StackTrace}";
				Task.Run(() => MailSender.SendMail(productType, message));
				return new Response<bool>(false, StatusType.Error, ex.Message);
			}
			catch (Exception ex)
			{
				return new Response<bool>(false, StatusType.Error, ex.Message);
			}
		}

		[WebMethod]
		public Response<bool> IsProductConnected(string productNumber)
		{
			var bop = new BusinessOperations(ConfigurationManager.AppSettings["ENV"].ToString());

			try
			{
				var isProductConnected = bop.IsProductConnected(productNumber);
				return new Response<bool>(isProductConnected, StatusType.Success, string.Empty);
			}
			catch (Exception ex)
			{
				return new Response<bool>(false, StatusType.Error, ex.Message);
			}
		}
	}
}