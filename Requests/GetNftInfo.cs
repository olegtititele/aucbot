using Newtonsoft.Json.Linq;
using System.Globalization;
using HtmlAgilityPack;

using ConfigFile;
using PostgreSQL;


namespace Requests
{
    public static class GetNftInfo
    {
        public static List<string> GetNft(string nftAddress)
        {
            List<string> nftInfo = new List<string>();

            string requestUrl = $"https://api.ton.cat/v2/contracts/nft/{nftAddress}";
            JObject jObject = JObject.Parse(GetJson(requestUrl).Result);

            string nftDescription = jObject["content"]["description"].ToString();
            string nftImage = jObject["content"]["image"]["w960"].ToString();
            string nftName = jObject["content"]["name"].ToString();
            string collectionAddress = jObject["collectionAddress"].ToString();
            string attributes;
            try
            {
                attributes =  "{ \"attributes\":" + jObject["content"]["attributes"].ToString() + "}";
            }
            catch(Exception){ attributes = "none"; }

            requestUrl = $"https://api.ton.cat/v2/contracts/nft_collection/{collectionAddress}";
            jObject = JObject.Parse(GetJson(requestUrl).Result);

            string collectionName = jObject["info"]["collectionContent"]["name"].ToString();


            nftInfo.Add(collectionName);
            nftInfo.Add(nftName);
            nftInfo.Add(nftImage);
            nftInfo.Add(nftDescription);
            nftInfo.Add(attributes);

            return nftInfo;
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