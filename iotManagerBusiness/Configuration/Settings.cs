namespace IotManagerBusiness.Configuration
{
	using System.Configuration;

	public static class Settings
	{
		public static string RFWifiCode => ConfigurationManager.AppSettings["RF_WIFI_COMPONENTCODE"];
		public static int WMCardstation => int.Parse(ConfigurationManager.AppSettings["WM_ELCARD_STATION"]);
		public static int CardLength => int.Parse(ConfigurationManager.AppSettings["CONNECTIVITY_CARD_LENGTH"]);
	}
}