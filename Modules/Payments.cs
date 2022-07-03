using PostgreSQL;
using Bot_Keyboards;
using Requests;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Modules
{
    public class Payments
    {
        public static async void CheckPayment(ITelegramBotClient botClient, long chatId, string username, int messageId, string nftCollection, string nftName, int popupSum)
        {
            string payComment = $"{chatId}{NewNftDB.GetNftId(nftCollection, nftName)}";

            if(CheckTransaction.Transaction(UserDB.GetWallet(chatId), chatId, payComment))
            {
                NewNftDB.UpdateLastBet(nftCollection, nftName, chatId, popupSum);

                await botClient.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: messageId,
                    text: "<b>✅ Поздравляем. Оплата прошла успешно.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToAuction
                );
                
                await botClient.SendTextMessageAsync(
                    chatId: Config.adminChannel,
                    text: $"<b>@{username} перевел {UserDB.GetPopupSum(chatId)} TON.</b>",
                    parseMode: ParseMode.Html
                );
            }
            else
            {
                await botClient.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: messageId,
                    text: "<b>❌ Ошибка. Транзакция не найдена. Проверьте указанный комментарий и сумму. Для вопросов обратитесь в службу поддержки.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.PaymentKb(payComment, popupSum)
                );
            }
            return;
        }

        public async static void RaiseBetStatic(ITelegramBotClient botClient, long chatId, int messageId, int auctionMinStep)
        {
            string nftName = UserDB.GetCurrentNft(chatId);
            string nftCollection = UserDB.GetCurrentNftCollection(chatId);
            string payComment = $"{chatId}{NewNftDB.GetNftId(nftCollection, nftName)}";
            int bet = auctionMinStep;
            UserDB.UpdatePopupSum(chatId, bet.ToString());

            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: $"<b>❕ Нажмите на кнопку \"Оплата\" для перевода {bet} TON.</b>",
                parseMode: ParseMode.Html,
                replyMarkup: Keyboards.PaymentKb(payComment, bet)
            );
        }
    }
}