using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using PostgreSQL;
using Bot_Keyboards;


namespace States
{
    public class AuctionValidUntilState
    {
        public async static void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId)
        {
            string validUntilDate = messageText;
            if(CheckDate(validUntilDate))
            {
                DateTime inputDate = Convert.ToDateTime(validUntilDate);
                DateTime maxAuctionTime = DateTime.Now.AddDays(7);

                if(inputDate <= maxAuctionTime && inputDate > DateTime.Now.AddDays(2))
                {
                    NewAuctionDB.UpdateAuctionValidUntil(chatId, inputDate);
                    UserDB.UpdateState(chatId, "MainMenu");
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>☑️ Успешно. Дата окончания аукциона обновлена.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.BackToNewAuctionInfo
                    );
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>☠️ Ошибка при вводе данных. Дата окончания аукциона не должна превышать 1 неделю от текущего времени и должна быть меньше 2 дней.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.BackToNewAuctionInfo
                    );
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>☠️ Ошибка при вводе данных. Дата должна быть строго формата \"ДД.ММ.ГГГГ чч:мм\". Максимальная длительность аукциона ровно 1 неделя от текущего времени.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToNewAuctionInfo
                );
            }
        }

        private static bool CheckDate(string date)
        {
            DateTime dt;
            return DateTime.TryParse(date, out dt);
        }
    }
}