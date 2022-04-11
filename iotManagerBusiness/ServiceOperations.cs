namespace IotManagerBusiness
{
	using Newtonsoft.Json;

	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Web;

	internal class ServiceOperations
	{
		public string BaseURL { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Token { get; set; }

		public ServiceOperations(string env)
		{
			if (env.Equals("PROD"))
			{
				BaseURL = "tempuri1";
				UserName = "user1";
				Password = "password1";
			}
			else
			{
				BaseURL = "tempuri2";
				UserName = "user2";
				Password = "password2";
			}

			Token = GetToken();
		}

		public Tuple<bool, string> DeAttachProductCardData(string productNumber, string cardbarcode, string productserial)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.BaseAddress = new Uri(BaseURL);
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

				var model = new
				{
					CardBarcode = cardbarcode,
					ApplianceSerialNumber = productNumber + productserial,
				};

				var json = JsonConvert.SerializeObject(model);
				var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
				var response = httpClient.PostAsync("api/CardData/DeAttachProductCardMatch", httpContent).Result.Content.ReadAsStringAsync().Result;

				var result = JsonConvert.DeserializeObject<ReturnModel<string>>(response);
				var success = result.Status == ReturnTypeStatus.Success;

				return new Tuple<bool, string>(success, success ? result.Message : $"Error during deattachment. Status code: {result.Status} - Message: {result.Message}");
			}
		}

		public Tuple<bool, string> AttachProductCardData(string productType, string productNumber, string cardbarcode, string productserial, string productBrand, string productConnectivity)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.BaseAddress = new Uri(BaseURL);
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

				var model = new
				{
					CardBarcode = cardbarcode,
					ApplianceSerialNumber = productNumber + productserial,
					MatchDateTime = DateTime.Now,
					ApplianceType = productType.Equals("WM") ? "Washer" : "Refrigerator",
					Connectivity = productConnectivity,
					Brand = productBrand
				};

				var json = JsonConvert.SerializeObject(model);
				var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
				var response = httpClient.PostAsync("api/CardData/AddProductCardDataMatch", httpContent).Result.Content.ReadAsStringAsync().Result;

				var result = JsonConvert.DeserializeObject<ReturnModel<string>>(response);
				var success = result.Status == ReturnTypeStatus.Success;

				return new Tuple<bool, string>(success, success ? result.Message : $"Error during attachment. Status code: {result.Status} - Message: {result.Message}");
			}
		}

		private string GetToken()
		{
			var client = new HttpClient { BaseAddress = new Uri(BaseURL) };
			var stringContent = new StringContent($"grant_type=password&username={HttpUtility.UrlEncode(UserName)}&password={HttpUtility.UrlEncode(Password)}", Encoding.UTF8, "application/x-www-form-urlencoded");
			var response = client.PostAsync("Token", stringContent).Result;

			var resultJson = response.Content.ReadAsStringAsync().Result;
			var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(resultJson);
			return dict["access_token"].ToString();
		}
	}
}