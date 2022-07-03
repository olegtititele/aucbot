using Newtonsoft.Json.Linq;
using System.Globalization;

using ConfigFile;
using PostgreSQL;



namespace Requests
{
    public static class CheckTransaction
    {
        private static string walletAddress = UserDB.GetTONWallet();
        // UserDB.GetTONWallet()
        public static bool Transaction(string userWallet, long chatId, string payComment)
        {
            string getWalletInfoURl = $"https://api.ton.cat/v2/explorer/getWalletInformation?address={walletAddress}";

            JObject jObject = JObject.Parse(GetJson(getWalletInfoURl).Result);

            string lt = jObject["result"]["last_transaction_id"]["lt"].ToString();
            string hash = jObject["result"]["last_transaction_id"]["hash"].ToString().Replace("/", "%2F").Replace("=", "%3D").Replace("+", "%2B");
            int transactionsCount = 1000;
            transactionsCount = GetLimitOfTransactions(transactionsCount, lt, hash, userWallet);
            
            if(GetAllTransactionsInfo(transactionsCount, lt, hash, userWallet, chatId, payComment))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static int GetLimitOfTransactions(int transactionsCount, string lt, string hash, string userWallet)
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
                return totalTransactions;
            }
            return totalTransactions;
        }

        private static bool GetAllTransactionsInfo(int transactionsCount, string lt, string hash, string userWallet, long chatId, string payComment)
        {
            int popupSum = UserDB.GetPopupSum(chatId);
            string requestURL = $"https://api.ton.cat/v2/explorer/getTransactions?address={walletAddress}&lt={lt}&hash={hash}&limit={transactionsCount}";
            JObject jObject = JObject.Parse(GetJson(requestURL).Result);
            for(int i = 0; i <= transactionsCount; i++)
            {
                try
                {
                    string walletSource = jObject["result"][i]["in_msg"]["source"].ToString();
                    long value = Int64.Parse(jObject["result"][i]["in_msg"]["value"].ToString()) / 1000000000;
                    string comment = jObject["result"][i]["in_msg"]["message"].ToString();
                    if(walletSource == userWallet)
                    {
                        if(value == popupSum && comment == payComment)
                        {
                            return true;
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
            return false;
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