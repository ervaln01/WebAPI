namespace IotManagerAPI
{
	using IotManagerBusiness.Enums;

	public class MailSender
	{
		public static void SendMail(ProductType type, string message)
		{
			try
			{
				var to = "example@mail.ru";
				using (var client_message = new EmailSender.SendMessageSoapClient())
					client_message.SendEmail_v2("no-reaply@mail.ru", $"IotManagerApi_{type}", to, "Upload to Azure failed", message, "", "", "", 0, "IotManagerApi");
			}
			catch { }
		}
	}
}