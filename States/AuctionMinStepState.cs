using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using PostgreSQL;
using Bot_Keyboards;

namespace States
{
    public class AuctionMinStepState
    {
        public async static void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId)
        {
            string minStepPrice = messageText;
            string nftName = UserDB.GetCurrentNft(chatId);
            string nftCollection = UserDB.GetCurrentNftCollection(chatId);
            int minStep = NewNftDB.GetMinStep(nftCollection, nftName);
            if(int.TryParse(minStepPrice, out int number) && Int32.Parse(minStepPrice) >= minStep)
            {
                NewAuctionDB.UpdateAuctionMinStep(chatId, minStepPrice);
                UserDB.UpdateState(chatId, "MainMenu");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>☑️ Успешно. Минимальный шаг повышения ставки NFT обновлен.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToNewAuctionInfo
                );
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>☠️ Ошибка при вводе данных. Минимальный шаг должен быть числом и должен быть больше {minStep}.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToNewAuctionInfo
                );
            }
        }
    }
}