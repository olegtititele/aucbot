using Newtonsoft.Json.Linq;



namespace Requests
{
    public class CheckNftRight
    {
        public static bool Nft(string nftAddress)
        {
            try
            {
                string requestUrl = $"https://api.ton.cat/v2/contracts/nft/{nftAddress}";
                JObject jObject = JObject.Parse(GetJson(requestUrl).Result);
                string nftDescription = jObject["content"].ToString();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            
        }
        private async static Task<string> GetJson(string requestURl)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.61 Safari/537.36");
            HttpResponseMessage response = await client.GetAsync(requestURl);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return json.ToString();
        }
    }
}