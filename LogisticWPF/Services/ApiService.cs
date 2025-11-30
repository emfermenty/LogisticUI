using LogisticWPF.DTO;
using LogisticWPF.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogisticWPF.Services
{
    public class ApiService
    {
        private const string BaseUrl = "http://localhost:5080";
        private readonly HttpClient _httpClient;
        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }
        public async Task<List<Shipping>> GetActiveShippings()
        {
            try
            {
                string url = "/api/Shipping/GetAllActiveShippings";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();

                var shippings = System.Text.Json.JsonSerializer.Deserialize<List<Shipping>>(json,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters =
                        {
                            new System.Text.Json.Serialization.JsonStringEnumConverter()
                        }
                    });

                Console.WriteLine(shippings.ToString());
                Console.WriteLine(json);
                return shippings ?? new List<Shipping>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка запроса: {ex.Message}");
                return new List<Shipping>();
            }
        }
        public async Task ChangeStatus(string TrackNumber, ShippingStatus status)
        {
            string url = "/api/Shipping/ShippingChangeStatus";
            var dto = new ChangeStatusDTO
            {
                TrackingNumber = TrackNumber,
                ShippingStatus = status
            };
            string json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
        public async Task<List<ShippingQuote>> GetShippingQuotes(double distance, double weight, double volume)
        {
            string url = "/api/Shipping/quotes";
            var dto = new ShippingRequest{ Distance = distance, Weight = weight, Volume = volume };
            string json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            var quotes = JsonSerializer.Deserialize<List<ShippingQuote>>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return quotes ?? new List<ShippingQuote>();
        }

        public async Task<ShippingQuote> GetOptimizedQuote(double distance, double weight, double volume)
        {
            string url = "/api/Shipping/optimize";
            var dto = new ShippingRequest{ Distance = distance, Weight = weight, Volume = volume };
            string json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            var quote = JsonSerializer.Deserialize<ShippingQuote>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return quote;
        }
    }
}
