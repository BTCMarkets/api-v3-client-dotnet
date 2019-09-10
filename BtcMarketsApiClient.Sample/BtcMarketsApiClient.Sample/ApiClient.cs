using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BtcMarketsApiClient.Sample
{
    public class ApiClient
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly string _privateKey;

        public ApiClient()
        {
            _baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            _apiKey = $"{ConfigurationManager.AppSettings["ApiKey"]}:{ConfigurationManager.AppSettings["443"]}";
            _privateKey = ConfigurationManager.AppSettings["PrivateKey"];
        }

        public async Task Get(string path, string queryString)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                GenerateHeaders(client, "GET", null, path);

                var fullPath = !string.IsNullOrEmpty(queryString) ? path + "?" + queryString : path;

                var response = await client.GetAsync(fullPath);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Success: " + responseString);
                }
                else
                    Console.WriteLine("Error: " + response.StatusCode.ToString());
            }
        }

        public async Task Post(string path, string queryString, object data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                var stringifiedData = data != null ? JsonConvert.SerializeObject(data) : null;
                GenerateHeaders(client, "POST", stringifiedData, path);

                var fullPath = !string.IsNullOrEmpty(queryString) ? path + "?" + queryString : path;
                var content = new StringContent(stringifiedData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(path, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Success: " + responseString);
                }
                else
                    Console.WriteLine("Error: " + response.StatusCode.ToString());
            }
        }

        public async Task Put(string path, string queryString, object data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                var stringifiedData = data != null ? JsonConvert.SerializeObject(data) : null;
                GenerateHeaders(client, "PUT", stringifiedData, path);

                var fullPath = !string.IsNullOrEmpty(queryString) ? path + "?" + queryString : path;
                var content = new StringContent(stringifiedData, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(path, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Success: " + responseString);
                }
                else
                    Console.WriteLine("Error: " + response.StatusCode.ToString());
            }
        }

        public async Task Delete(string path, string queryString)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                GenerateHeaders(client, "DELETE", null, path);

                var fullPath = !string.IsNullOrEmpty(queryString) ? path + "?" + queryString : path;

                var response = await client.DeleteAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Success: " + responseString);
                }
                else
                    Console.WriteLine("Error: " + response.StatusCode.ToString());
            }
        }


        private void GenerateHeaders(HttpClient client, string method, string data, string path)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var message = method + path + now.ToString();
            if (!string.IsNullOrEmpty(data))
            {
                message += data;
                client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            }

            string signature = SignMessage(message);
            //client.DefaultRequestHeaders.Remove("Content-Type");
            client.DefaultRequestHeaders.Add("Accept", "application /json");
            client.DefaultRequestHeaders.Add("Accept-Charset", "UTF-8");
            client.DefaultRequestHeaders.Add("BM-AUTH-APIKEY", _apiKey);
            client.DefaultRequestHeaders.Add("BM-AUTH-TIMESTAMP", now.ToString());
            client.DefaultRequestHeaders.Add("BM-AUTH-SIGNATURE", signature);
        }

        private string SignMessage(string message)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_apiKey);
            var bytes = Encoding.UTF8.GetBytes(message);
            using (var hash = new HMACSHA512(keyBytes))//SHA512.Create(keyBytes))
            {
                var hashedInputeBytes = hash.ComputeHash(bytes);
                return Convert.ToBase64String(hashedInputeBytes);
            }
        }
    }
}
