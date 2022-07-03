using Newtonsoft.Json.Linq;
using System.Globalization;
using HtmlAgilityPack;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Requests
{
    public static class GetWalletInfo
    {
        static HttpClient client = new HttpClient();
        public static List<double> GetWalletBalance(string walletAddress)
        {
            List<double> balance = new List<double>();
            string requestURL = $"https://api.ton.cat/v2/explorer/getWalletInformation?address={walletAddress}";

            JObject jObject = JObject.Parse(GetJson(requestURL).Result);

            double balanceTON = double.Parse(jObject["result"]["balance"].ToString(), CultureInfo.InvariantCulture) / 1000000000;
            double balanceUSD = balanceTON * GetUsdRate();
            
            balanceUSD = Math.Round(balanceUSD, 2, MidpointRounding.AwayFromZero);
            
            balance.Add(balanceTON);
            balance.Add(balanceUSD);
            return balance;
        }

        async static Task<string> GetJson(string requestURl)
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:97.0) Gecko/20100101 Firefox/97.0");
            HttpResponseMessage response = await client.GetAsync(requestURl);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return json.ToString();
        }

        private static double GetUsdRate()
        {
            HtmlWeb web = new HtmlWeb();
            web.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36";
            HtmlDocument document = web.Load("https://finance.yahoo.com/quote/TONCOIN-USD/?guccounter=1&guce_referrer=aHR0cHM6Ly93d3cuZ29vZ2xlLmNvbS8&guce_referrer_sig=AQAAACA9m3ZHhvc0j1-pNIffA9ZbBUGULZdnenEyIBDe11COMOlz9C9HvKslMI_usIIAe9ka2C0ab2jVyuFEus_gv-GYQ_kzf0gNq1yZSyGP1BBmcuCAZzbAx1Rw58yFZhT8xsoKvXbWqrsC7JwRLLg1-9M6ZeXBM9mvaDSa63XMA28e");
            string usdRateBlock = document.DocumentNode.SelectSingleNode("//fin-streamer[@data-symbol=\"TONCOIN-USD\"]").GetAttributeValue("value", "");
            double usdRate = double.Parse(usdRateBlock, CultureInfo.InvariantCulture);
            return usdRate;
        }
    }
}