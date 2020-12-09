using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using HealthHerb.Data;

namespace HealthHerb.Payment
{
    public class PayPal
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppDbContext context;
        private readonly HttpClient httpClient;
        private readonly string baseUrl;

        public PayPal(IHttpContextAccessor httpContextAccessor, AppDbContext context, bool isLive)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
            httpClient = new HttpClient();
            baseUrl = isLive ? "https://api.paypal.com" : "https://api.sandbox.paypal.com";
        }

        public async Task<bool> Capture(string orderId)
        {
            string accessToken = await GenerateAccessToken();

            if (accessToken == null)
            {
                return false;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.PostAsync($"{baseUrl}/v2/checkout/orders/{orderId}/capture", new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                if (json.status == "COMPLETED")
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<string> CreateOrder(decimal amount, string currency)
        {
            string accessToken = await GenerateAccessToken();

            if (accessToken == null)
            {
                return null;
            }

            var data = new
            {
                intent = "CAPTURE",
                purchase_units = new object[]
                {
                    new
                    {
                        amount = new
                        {
                            value = amount.ToString(),
                            currency_code = currency
                        }
                    }
                }
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.PostAsync($"{baseUrl}/v2/checkout/orders", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                return json.id;
            }

            return null;
        }

        public async Task<bool> IsPayed(string transactionId)
        {
            string accessToken = await GenerateAccessToken();

            if (accessToken == null)
            {
                return false;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync($"{baseUrl}/v2/checkout/orders/{transactionId}");

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                if (json.intent == "CAPTURE" && json.status == "COMPLETED")
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<string> GenerateClientToken()
        {
            string accessToken = await GenerateAccessToken();

            if (accessToken == null)
            {
                return null;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.PostAsync($"{baseUrl}/v1/identity/generate-token", new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                string token = json.client_token;
                int expire = json.expires_in;

                httpContextAccessor.HttpContext.Response.Cookies.Append("client_token", token, new CookieOptions { Expires = DateTime.Now.AddSeconds(expire) });

                return json.client_token;
            }

            return null;
        }

        private async Task<string> GenerateAccessToken()
        {
            string token = null;
            var credentials = await context.PaymentManages.FirstOrDefaultAsync();

            if (credentials == null)
            {
                return null;
            }

            if (credentials.AccessToken != null)
            {
                if ((DateTime.Now - credentials.CreatedAt) < TimeSpan.FromSeconds(credentials.TokenExpireAt))
                {
                    return credentials.AccessToken;
                }
            }

            var creditsBytes = Encoding.UTF8.GetBytes($"{credentials.ClientId}:{credentials.ClientSecret}");
            var creditsBase64 = Convert.ToBase64String(creditsBytes);
            var creditsData = new StringContent("grant_type=client_credentials");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", creditsBase64);

            var response = await httpClient.PostAsync(baseUrl + "/v1/oauth2/token", creditsData);

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                credentials.AccessToken = json.access_token;
                credentials.TokenExpireAt = json.expires_in;
                credentials.CreatedAt = DateTime.Now;

                context.PaymentManages.Update(credentials);

                await context.SaveChangesAsync();

                token = json.access_token;
            }

            return token;
        }

    }
}
