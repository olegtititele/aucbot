using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using PostgreSQL;
using Bot_Keyboards;

namespace States
{
    public class AuctionRedemptionNftPriceState
    {
        public async static void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId)
        {
            string redemptionPrice = messageText;
            if(int.TryParse(redemptionPrice, out int number) && Int32.Parse(redemptionPrice) > 1)
            {
                NewAuctionDB.UpdateAuctionRedemptionNftPrice(chatId, redemptionPrice);
                UserDB.UpdateState(chatId, "MainMenu");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>☑️ Успешно. Цена выкупа NFT обновлена.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToNewAuctionInfo
                );
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>☠️ Ошибка при вводе данных. Цена выкупа NFT должна быть числом и должна быть больше 1.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToNewAuctionInfo
                );
            }
        }
    }
}