using PostgreSQL;
using Bot_Keyboards;
using Requests;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Modules
{
    public class CheckVerificationClass
    {
        public async static void CallDataCheckVerification(ITelegramBotClient botClient, long chatId, int messageId, string username)
        {
            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: "<b>☑️ Ваш кошелёк на проверке. Ожидайте.</b>",
                parseMode: ParseMode.Html
            );
            if(CheckVerification.Verification(UserDB.GetWallet(chatId)))
            {
                await botClient.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: messageId,
                    text: "<b>✅ Поздравляем. Ваш кошелёк верифицирован.</b>",
                    parseMode: ParseMode.Html
                );
                await botClient.SendTextMessageAsync(
                    chatId: Config.adminChannel,
                    text: $"<b>@{username} верифицировал кошлёк.</b>",
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
                    replyMarkup: Keyboards.VerifacationKb()
                );
            }
        }
    }
}