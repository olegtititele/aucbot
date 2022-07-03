using PostgreSQL;
using Bot_Keyboards;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Modules
{
    public static class ShowNft
    {
        public static async void NftInfo(ITelegramBotClient botClient, long chatId, int messageId, string nftCollection, int index)
        {
            List<string> nftNames = NewNftDB.GetAllNftNamesFromCollection(nftCollection);
            string nftName = nftNames[index];
            int lastItemIndex = NewNftDB.NftInCollectionCount(nftCollection);
            UserDB.UpdateCurrentNft(chatId, nftName);
            List<string> nftInfo = NewNftDB.GetNft(nftCollection, nftName);

            string nftAddress = nftInfo[2];
            string nftImage = nftInfo[3];
            string nftDescription = nftInfo[4];
            string firstBet = nftInfo[7];
            string lastBet = nftInfo[8];
            string validUntil = Convert.ToDateTime(nftInfo[13]).ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));

            string caption;

            if(NewNftDB.GetAttributes(nftCollection, nftName) == "none")
            {
                caption = $"<b>💎 {nftName}</b>\n\n<i>{nftDescription}</i>\n\n<b>🗂 Коллекция: </b><code>{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nftCollection.Replace("_"," "))}</code>";
            }
            else
            {
                string attributes = GetAttributesText(nftCollection, nftName);
                caption = $"<b>💎 {nftName}</b>\n\n<i>{nftDescription}</i>\n\n<b>🗂 Коллекция: </b><code>{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nftCollection.Replace("_"," "))}</code>\n\n{attributes}";
            }
            

            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: $"{caption}<a href=\"{nftImage}\">&#8203;</a>",
                parseMode: ParseMode.Html,
                replyMarkup: Keyboards.NftKb(index, lastItemIndex, nftCollection, validUntil)
            );
            return;
        }

        public static async void NftTradeKb(ITelegramBotClient botClient, long chatId, int messageId, string nftCollection, string nftName)
        {
            try
            {
                List<string> nftInfo = NewNftDB.GetNft(nftCollection, nftName);

                string nftAddress = nftInfo[2];
                string nftImage = nftInfo[3];
                string nftDescription = nftInfo[4];
                string startBet = nftInfo[7];
                string lastBet = nftInfo[8];
                int minStep = Int32.Parse(nftInfo[9]);
                int redemptionPrice = Int32.Parse(nftInfo[11]);
                string validUntil = Convert.ToDateTime(nftInfo[13]).ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));
                

                string caption;

                if(NewNftDB.GetAttributes(nftCollection, nftName) == "none")
                {
                    caption = $"<b>💎 {nftName}</b>\n\n<i>{nftDescription}</i>\n\n<b>🗂 Коллекция: </b><code>{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nftCollection.Replace("_"," "))}</code>";
                }
                else
                {
                    string attributes = GetAttributesText(nftCollection, nftName);
                    caption = $"<b>💎 {nftName}</b>\n\n<i>{nftDescription}</i>\n\n<b>🗂 Коллекция: </b><code>{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nftCollection.Replace("_"," "))}</code>\n\n{attributes}";
                }
                

                await botClient.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: messageId,
                    text: $"{caption}<a href=\"{nftImage}\">&#8203;</a>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.TradeKb(chatId, nftCollection, nftName, startBet, lastBet, redemptionPrice, minStep)
                );
            }
            catch(Exception){ return; }
            
            return;
        }

        static string GetAttributesText(string nftCollection, string nftName)
        {
            string attributesText = "<b>📜 Атрибуты: </b>\n\n";
            
            JObject jsonAttributes = JObject.Parse(NewNftDB.GetAttributes(nftCollection, nftName));
            var attributes = jsonAttributes["attributes"];

            foreach(JObject attribute in attributes)
            {
                string traitType = attribute["trait_type"].ToString().ToUpper();
                string value = attribute["value"].ToString();
                string line = $"<b>{traitType}: </b><code>{value}</code>\n";
                attributesText += line;
            }
            return attributesText;
        }
    }
}