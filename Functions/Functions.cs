using ConfigFile;
using PostgreSQL;

namespace Functions
{
    public class WalletFunction
    {
        public static string GetLineCase(int nftCollectionsNumber)
        {
            string selectCase;

            if(nftCollectionsNumber == 1 || nftCollectionsNumber % 10 == 1)
            {
                selectCase = "коллекция";
            }
            else if(nftCollectionsNumber == 2 || nftCollectionsNumber % 10 == 2)
            {
                selectCase = "коллекции";
            }
            else if(nftCollectionsNumber == 3 || nftCollectionsNumber % 10 == 3)
            {
                selectCase = "коллекции";
            }
            else if(nftCollectionsNumber == 4 || nftCollectionsNumber % 10 == 4)
            {
                selectCase = "коллекции";
            }
            else if(nftCollectionsNumber == 5 || nftCollectionsNumber % 10 == 5)
            {
                selectCase = "коллекций";
            }
            else if(nftCollectionsNumber == 6 || nftCollectionsNumber % 10 == 6)
            {
                selectCase = "коллекций";
            }
            else if(nftCollectionsNumber == 7 || nftCollectionsNumber % 10 == 7)
            {
                selectCase = "коллекций";
            }
            else if(nftCollectionsNumber == 8 || nftCollectionsNumber % 10 == 8)
            {
                selectCase = "коллекций";
            }
            else if(nftCollectionsNumber == 9 || nftCollectionsNumber % 10 == 9)
            {
                selectCase = "коллекций";
            }
            else
            {
                selectCase = "коллекций";
            }
            return selectCase;
        }
        public static bool CheckWalletRight(string wallet)
        {
            if(wallet.Length==48)
            {
                foreach(var letter in wallet)
                {
                    if(Config.walletSymbols.Contains(letter)){  }
                    else{return false;}
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckSubChannel(string chatMember)
        {
            if(chatMember != "Left")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckAuctionInfo(long chatId)
        {
            List <string> auctionInfo = NewAuctionDB.GetNewAuctionInfo(chatId);
            foreach(string e in auctionInfo)
            {
                if(e == "none")
                {
                    return false;
                }
            }
            return true;
        }
    }
}