using PostgreSQL;
using Bot_Keyboards;
using Functions;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;




namespace Modules
{
    public class NextAuctionStep
    {
        public async static void CallDataNextAuctionStep(ITelegramBotClient botClient, CallbackQuery callbackQuery, long chatId, int messageId)
        {
            string errorText = "☠️ Ошибка при вводе данных!\n\n";
            if(!WalletFunction.CheckAuctionInfo(chatId))
            {
                if(NewAuctionDB.GetAuctionNftAddress(chatId) == "none")
                {
                    errorText += "У вас не указан адрес NFT.\n";
                }
                if(NewAuctionDB.GetAuctionStartNftPrice(chatId) == "none")
                {
                    errorText += "У вас не указана стартовая цена NFT.\n";
                }
                if(NewAuctionDB.GetAuctionRedemptionNftPrice(chatId) == "none")
                {
                    errorText += "У вас не указана цена выкупа NFT.\n";
                }
                if(NewAuctionDB.GetAuctionMinStep(chatId) == "none")
                {
                    errorText += "У вас не указан минимальный шаг повышения ставки.\n";
                }
                if(NewAuctionDB.GetAuctionValidUntil(chatId) <= DateTime.Now.AddDays(2))
                {
                    errorText += "Укажите правильное время действия аукциона.\n";
                }
                await botClient.AnswerCallbackQueryAsync(
                    callbackQueryId:callbackQuery.Id,
                    text: errorText,
                    showAlert: true
                );
                return;
            }
            else if(NewAuctionDB.GetAuctionValidUntil(chatId) <= DateTime.Now.AddDays(2))
            {
                errorText += "Укажите правильное время действия аукциона.\n";
                await botClient.AnswerCallbackQueryAsync(
                    callbackQueryId:callbackQuery.Id,
                    text: errorText,
                    showAlert: true
                );
                return;
            }
            else
            {
                await botClient.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: messageId,
                    text: $"<b>Для создания аукциона отправьте свою NFT на кошелёк аукциона.\nАдрес кошелька: </b><code>{UserDB.GetTONWallet()}</code>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.SentNftToHold
                );
            }
        }
    }
}