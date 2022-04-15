namespace IotManagerBusiness.Configuration
{
	using System.Configuration;

	public static class Settings
	{
		public static string RFWifiCode => ConfigurationManager.AppSettings["RF_WIFI_COMPONENTCODE"];
		public static string WMCardstation => ConfigurationManager.AppSettings["WM_ELCARD_STATION"];
		public static string CardLength => ConfigurationManager.AppSettings["CONNECTIVITY_CARD_LENGTH"];
	}
}