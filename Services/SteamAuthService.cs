using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace _123123.Services
{
    public class SteamAuthService
    {
        private const string STEAM_OPENID_URL = "https://steamcommunity.com/openid/login";
        private readonly string _returnUrl;
        private readonly HttpClient _httpClient;

        public SteamAuthService(string returnUrl = "http://127.0.0.1:8080/callback/")
        {
            _returnUrl = returnUrl;
            _httpClient = new HttpClient();
        }

        public string GetAuthUrl()
        {
            var queryParams = new NameValueCollection
            {
                {"openid.ns", "http://specs.openid.net/auth/2.0"},
                {"openid.mode", "checkid_setup"},
                {"openid.return_to", _returnUrl},
                {"openid.realm", _returnUrl},
                {"openid.identity", "http://specs.openid.net/auth/2.0/identifier_select"},
                {"openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select"}
            };

            var uriBuilder = new UriBuilder(STEAM_OPENID_URL);
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (string key in queryParams)
            {
                query[key] = queryParams[key];
            }
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }

        private Dictionary<string, string> NameValueCollectionToDictionary(NameValueCollection collection)
        {
            var dict = new Dictionary<string, string>();
            foreach (string key in collection.Keys)
            {
                if (key != null)
                {
                    dict[key] = collection[key];
                }
            }
            return dict;
        }

        public async Task<string> ValidateAndGetSteamIdAsync(string responseUrl)
        {
            var responseParams = HttpUtility.ParseQueryString(new Uri(responseUrl).Query);
            
            // Проверяем, что ответ действительно от Steam
            var validationParams = new NameValueCollection
            {
                {"openid.ns", responseParams["openid.ns"]},
                {"openid.mode", "check_authentication"},
                {"openid.op_endpoint", responseParams["openid.op_endpoint"]},
                {"openid.claimed_id", responseParams["openid.claimed_id"]},
                {"openid.identity", responseParams["openid.identity"]},
                {"openid.return_to", responseParams["openid.return_to"]},
                {"openid.response_nonce", responseParams["openid.response_nonce"]},
                {"openid.assoc_handle", responseParams["openid.assoc_handle"]},
                {"openid.signed", responseParams["openid.signed"]},
                {"openid.sig", responseParams["openid.sig"]}
            };

            var content = new FormUrlEncodedContent(NameValueCollectionToDictionary(validationParams));
            var response = await _httpClient.PostAsync(STEAM_OPENID_URL, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (responseContent.Contains("is_valid:true"))
            {
                var steamId = responseParams["openid.claimed_id"]
                    .Split('/')
                    .LastOrDefault();
                
                if (string.IsNullOrEmpty(steamId))
                {
                    throw new Exception("Failed to extract Steam ID from response");
                }

                return steamId;
            }

            throw new Exception("Steam authentication failed");
        }
    }
} 