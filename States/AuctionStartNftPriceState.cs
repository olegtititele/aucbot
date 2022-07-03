using Telegram.Bot;
using Telegram.Bot.Types.Enums;


using Requests;
using PostgreSQL;
using Bot_Keyboards;


namespace States
{
    public class AuctionStartNftPriceState
    {
        public async static void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId)
        {
            string startPrice = messageText;
            if(int.TryParse(startPrice, out int number) && Int32.Parse(startPrice) > 1)
            {
                NewAuctionDB.UpdateAuctionStartNftPrice(chatId, startPrice);
                UserDB.UpdateState(chatId, "MainMenu");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>☑️ Успешно. Стартовая цена торга NFT обновлена.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToNewAuctionInfo
                );
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>☠️ Ошибка при вводе данных. Стартовая цена торга должна быть числом и должна быть больше 1.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToNewAuctionInfo
                );
            }
        }
    }
}