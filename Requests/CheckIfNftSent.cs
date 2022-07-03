using Newtonsoft.Json.Linq;
using System.Globalization;

using ConfigFile;
using PostgreSQL;


namespace Requests
{
    public class CheckIfNftSent
    {
        private static string walletAddress = UserDB.GetTONWallet();
        public static bool Transaction(long chatId)
        {
            string getWalletInfoURl = $"https://api.ton.cat/v2/explorer/getWalletInformation?address={walletAddress}";

            JObject jObject = JObject.Parse(GetJson(getWalletInfoURl).Result);

            string lt = jObject["result"]["last_transaction_id"]["lt"].ToString();
            string hash = jObject["result"]["last_transaction_id"]["hash"].ToString().Replace("/", "%2F").Replace("=", "%3D").Replace("+", "%2B");
            int transactionsCount = 1000;
            GetLimitOfTransactions(transactionsCount, lt, hash, chatId);
            
            if(NewAuctionDB.GetIfNftSent(chatId) == "none")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static void GetLimitOfTransactions(int transactionsCount, string lt, string hash, long chatId)
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
                GetLimitOfTransactions(transactionsCount, lt, hash, chatId);
            }
            else
            {
                GetAllTransactionsInfo(transactionsCount, lt, hash, chatId);
            }
        }

        private static void GetAllTransactionsInfo(int transactionsCount, string lt, string hash, long chatId)
        {
            string inputNftAddress = NewAuctionDB.GetAuctionNftAddress(chatId);
            string requestURL = $"https://api.ton.cat/v2/explorer/getTransactions?address={walletAddress}&lt={lt}&hash={hash}&limit={transactionsCount}";
            JObject jObject = JObject.Parse(GetJson(requestURL).Result);
            for(int i = 0; i <= transactionsCount; i++)
            {
                try
                {
                    string nftAddress = jObject["result"][i]["in_msg"]["source"].ToString();

                    if(nftAddress == inputNftAddress)
                    {
                        NewAuctionDB.UpdateIfNftSent(chatId, "yes");
                        return;
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