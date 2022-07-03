using Newtonsoft.Json.Linq;
using System.Globalization;
using HtmlAgilityPack;

using ConfigFile;
using PostgreSQL;



namespace Requests
{
    public static class CheckVerification
    {
        private static string walletAddress = UserDB.GetTONWallet();
        // UserDB.GetTONWallet()
        public static bool Verification(string userWallet)
        {
            string getWalletInfoURl = $"https://api.ton.cat/v2/explorer/getWalletInformation?address={walletAddress}";

            JObject jObject = JObject.Parse(GetJson(getWalletInfoURl).Result);

            string lt = jObject["result"]["last_transaction_id"]["lt"].ToString();
            string hash = jObject["result"]["last_transaction_id"]["hash"].ToString().Replace("/", "%2F").Replace("=", "%3D").Replace("+", "%2B");
            int transactionsCount = 1000;
            GetLimitOfTransactions(transactionsCount, lt, hash, userWallet);
            
            if(UserDB.GetVerificationOfWallet(userWallet) == "none")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static void GetLimitOfTransactions(int transactionsCount, string lt, string hash, string userWallet)
        {
            string requestURL = $"https://api.ton.cat/v2/explorer/getTransactions?address={walletAddress}&lt={lt}&hash={hash}&limit={transactionsCount}";
            JObject jObject = JObject.Parse(GetJson(requestURL).Result);
            int totalTransactions = 0;
            for(int i = 0; i <= transactionsCount; i++)
            {
                try
                {
                    var transaction = jObject["result"][i];
                    totalTransactions++;
                }
                catch(System.ArgumentOutOfRangeException){ break; }
            }

            if(totalTransactions == transactionsCount)
            {
                transactionsCount += transactionsCount;
                GetLimitOfTransactions(transactionsCount, lt, hash, userWallet);
            }
            else
            {
                GetAllTransactionsInfo(transactionsCount, lt, hash, userWallet);
            }
        }

        private static void GetAllTransactionsInfo(int transactionsCount, string lt, string hash, string userWallet)
        {
            string requestURL = $"https://api.ton.cat/v2/explorer/getTransactions?address={walletAddress}&lt={lt}&hash={hash}&limit={transactionsCount}";
            JObject jObject = JObject.Parse(GetJson(requestURL).Result);
            for(int i = 0; i <= transactionsCount; i++)
            {
                try
                {
                    string walletSource = jObject["result"][i]["in_msg"]["source"].ToString();
                    double value = double.Parse(jObject["result"][i]["in_msg"]["value"].ToString(), CultureInfo.InvariantCulture) / 1000000000;
                    string comment = jObject["result"][i]["in_msg"]["message"].ToString();
                    if(walletSource == userWallet)
                    {
                        if(value >= 1 && comment == Config.verifComment)
                        {
                            UserDB.VerificateWallet(userWallet);
                            return;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }     
                }
                catch(System.ArgumentOutOfRangeException){ }
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