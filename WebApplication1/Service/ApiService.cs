using System.Net.Http.Headers;
using WebApplication2.Model;
using System.Linq;
namespace WebApplication1.Service
{
    public class ApiService
    {

        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5000"); // Replace with your API URL
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<SuperStore>> GetAllDataAsync(int page = 1, int pageSize = 100)
        {
            List<SuperStore> records = null;
            HttpResponseMessage response = await _client.GetAsync($"api/csvdata?page={page}&pageSize={pageSize}");

            if (response.IsSuccessStatusCode)
            {
                records = await response.Content.ReadFromJsonAsync<List<SuperStore>>();
            }
            return records ?? new List<SuperStore>();
        }
    }
}
