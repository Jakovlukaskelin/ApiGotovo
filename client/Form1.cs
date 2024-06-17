using Newtonsoft.Json;
using System.Windows.Forms;

namespace client
{
    public partial class Form1 : Form
    {
        private const string BaseUrl = "https://localhost:5000/api/CsvData";

        private const string AuthEndpoint = "https://localhost:5000/api/Auth/login";

        private const string RefreshTokenEndpoint = "https://localhost:5000/api/Auth/refresh";
        private string currentRefreshToken;

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            try
            {
                // autoriziraj daj token
                var (accessToken, refreshToken) = await AuthenticateUser(username, password);

                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    currentRefreshToken = refreshToken; // spremi  refresh token

                    List<SuperStore> records = await GetCsvData(accessToken);

                    dataGridView1.DataSource = records;
                }
                else
                {
                    MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<(string accessToken, string refreshToken)> AuthenticateUser(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var loginModel = new { Username = username, Password = password };
                var content = new StringContent(JsonConvert.SerializeObject(loginModel), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(AuthEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(apiResponse);
                    return (data.accessToken, data.refreshToken);
                }
                else
                {
                    return (null, null);
                }
            }
        }

        private async Task<List<SuperStore>> GetCsvData(string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                string requestUrl = $"{BaseUrl}?page=1&pageSize=200";

                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    List<SuperStore> records = JsonConvert.DeserializeObject<List<SuperStore>>(apiResponse);
                    return records;
                }
                else
                {
                    throw new Exception($"Failed to retrieve data. Status code: {response.StatusCode}");
                }
            }
        }

        private async Task<string> RefreshAccessToken()
        {
            using (HttpClient client = new HttpClient())
            {
                
                var refreshTokenContent = new { refreshToken = currentRefreshToken };
                var content = new StringContent(JsonConvert.SerializeObject(refreshTokenContent), System.Text.Encoding.UTF8, "application/json");

                // zovi token endpoint
                HttpResponseMessage response = await client.PostAsync(RefreshTokenEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(apiResponse);
                   
                    currentRefreshToken = data.refreshToken;
                    return data.accessToken;
                }
                else
                {
                    
                    return null;
                }
            }
        }
    }



}


