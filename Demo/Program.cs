using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace Demo
{
	class Program
	{
		private static readonly AuthenticationService AuthenticationService = new AuthenticationService("API-Key");
		private const String InstanceName = "Instance-Name";

		static void Main(string[] args)
		{
			var gateway = new PayrexxGateway
			{
				Purpose = "Test xyz",
				Amount = 28115,
				Currency = "CHF",
				ReferenceId = "27",
				SuccessRedirectUrl = "https://test.ch/success/[TRANSACTION_UUID]",
				FailedRedirectUrl = "https://test.ch/failed/[TRANSACTION_UUID]",
				Fields = new Dictionary<string, PayrexxField>
				{
					{"title",
						new PayrexxField {
							Value = "mister"
						}
					},
					{"forename",
						new PayrexxField {
							Value = "Hans"
						}
					},
					{"surname",
						new PayrexxField {
							Value = "Muster"
						}
					},
					{"email",
						new PayrexxField {
							Value = "hans@muster.ch"
						}
					}
				}
			};

			var gatewayResponse = CreateGateway(gateway, InstanceName);
			Console.WriteLine(gatewayResponse.Link);
		}

		public static PayrexxGateway CreateGateway(PayrexxGateway gateway, string instance)
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			var data = new Dictionary<string, string>
			{
				{ "purpose", gateway.Purpose },
				{ "amount", gateway.Amount.ToString() },
				{ "currency", gateway.Currency },
				{ "referenceId", gateway.ReferenceId },
				{ "successRedirectUrl", gateway.SuccessRedirectUrl },
				{ "failedRedirectUrl", gateway.FailedRedirectUrl }
			};
			foreach (var field in gateway.Fields)
			{
				data.Add("fields[" + field.Key + "][value]", field.Value.Value);
			}

			var payload = GetPayloadRfc1738(data);
			var apiSignature = AuthenticationService.GetApiSignature(payload);
			data.Add("ApiSignature", apiSignature);
			payload = GetPayloadRfc3986(data);

			var requestUri = new Uri("https://api.payrexx.com/v1.0/Gateway/?instance=" + instance);

			var client = new RestClient(requestUri);
			var request = new RestRequest(Method.POST);
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", payload, ParameterType.RequestBody);

			var response = client.Execute(request);
			if (response.IsSuccessful)
			{
				var payrexxResponseGateway = JsonConvert.DeserializeObject<PayrexxResponseGateway>(response.Content);
				if (payrexxResponseGateway.Status == "success")
					return payrexxResponseGateway.Data.OrderByDescending(x => x.CreatedAt).FirstOrDefault(x => x.ReferenceId == gateway.ReferenceId);
			}
			return new PayrexxGateway();
		}
		private static string GetPayloadRfc1738(Dictionary<string, string> postData)
		{
			var urlEncoded = "";
			foreach (var data in postData)
			{
				var concat = string.IsNullOrEmpty(urlEncoded) ? "" : "&";
				urlEncoded += concat + WebUtility.UrlEncode(data.Key) + "=" + WebUtility.UrlEncode(data.Value);
			}

			return urlEncoded;
		}

		private static string GetPayloadRfc3986(Dictionary<string, string> postData)
		{
			var urlEncoded = "";
			foreach (var data in postData)
			{
				var concat = string.IsNullOrEmpty(urlEncoded) ? "" : "&";
				urlEncoded += concat + Uri.EscapeDataString(data.Key) + "=" + Uri.EscapeDataString(data.Value);
			}

			return urlEncoded;
		}
	}
}
