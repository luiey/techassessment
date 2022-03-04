using Microsoft.Extensions.Configuration;

namespace techassessment
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Test assessment start by Wan Mohd Fadhli bin Wan Mohd Azmin");
            if (ReadAppSettings())
            {
                var baseurl = "http://test-demo.aemenersol.com";
                Uri baseUri = new Uri(baseurl);
                if (await Ping(baseurl))
                {
                    LoginModel login = new LoginModel() { username = "user@aemenersol.com", password = "Test@123" };
                    string? jsonObject = Newtonsoft.Json.JsonConvert.SerializeObject(login);
                    StringContent? jsonContent = new StringContent(jsonObject, System.Text.Encoding.UTF8, "application/json");
                    /// Account
                    string targetEndpoint = "/api/Account/Login";
                    Uri myUri = new Uri(baseUri, targetEndpoint);
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.PostAsync(myUri.ToString(), jsonContent);
                    string? result = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string token = result.Replace("\"", string.Empty);
                        Console.WriteLine("Login:" + token);
                        /// PlatformWell
                        targetEndpoint = "/api/PlatformWell/GetPlatformWellActual";
                        myUri = new Uri(baseUri, targetEndpoint);
                        client = new HttpClient();
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        response = await client.GetAsync(myUri.ToString());
                        result = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("GetPlatformWell:" + result);
                            List<Platform> platform = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Platform>>(result);
                            PWDb objDb = new PWDb(Global.localDBConn);
                            objDb.HandlePlatformWellData(platform);
                            objDb.Disconnect();
                            Console.WriteLine("Platform well has been performed.");
                        }
                        else
                        {
                            Console.WriteLine(string.Format("GetPlatformWell failed. Status: ({0}) {1}", (int)response.StatusCode, response.StatusCode));
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Login failed. Status: ({0}) {1}", (int)response.StatusCode, response.StatusCode));
                    }
                }
            }

        }
        static bool ReadAppSettings()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
                IConfiguration config = builder.Build();
                var ConnectionStrings = config.GetSection("ConnectionStrings").Get<ConnectionStrings>();
                Console.WriteLine($"Connection string: {ConnectionStrings.PlatformWell}");
                Global.localDBConn = ConnectionStrings.PlatformWell;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error read app settings. " + ex.Message);
            }
            return false;
        }
        static void ManagePlatformWell()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while manage platform well data. " + ex.Message);
            }
        }
        static async Task<bool> Ping(string url)
        {
            try
            {
                System.Net.Http.HttpClient client = new HttpClient();
                Uri myUri = new Uri(url);
                string domainname = "http://" + myUri.Host;
                using HttpResponseMessage response = await client.GetAsync(url);
                using HttpContent content = response.Content;
                var json = await content.ReadAsStringAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Web API not available. " + ex.Message);
                return false;
            }
        }
    }

}