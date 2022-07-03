using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using PostgreSQL;
using Bot_Keyboards;

namespace States
{
    public class InputNewBetState
    {
        public async static void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId)
        {
            string nftName = UserDB.GetCurrentNft(chatId);
            string nftCollection = UserDB.GetCurrentNftCollection(chatId);
            int minStep = NewNftDB.GetMinStep(nftCollection, nftName);

            if(int.TryParse(messageText, out int number) && Int32.Parse(messageText) >= minStep)
            {
                string payComment = $"{chatId}{NewNftDB.GetNftId(nftCollection, nftName)}";
                string bet = messageText;
                UserDB.UpdatePopupSum(chatId, bet);
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>❕ Нажмите на кнопку \"Оплата\" для перевода {bet} TON.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.PaymentKb(payComment, Int32.Parse(bet))
                );
                UserDB.UpdateState(chatId, "MainMenu");
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>☠️ Ошибка при вводе данных. Ставка должна быть числом и должна быть больше {minStep} TON.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToAuction
                );
            }
        }
    }
}