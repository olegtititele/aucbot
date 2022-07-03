using PostgreSQL;
using Bot_Keyboards;


using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Globalization;

namespace Modules
{
    public class NewAuction
    {
        public async static void GetNewAuctionInfo(ITelegramBotClient botClient, long chatId, int messageId)
        {
            UserDB.UpdateState(chatId, "MainMenu");

            string auctionNftAddress = NewAuctionDB.GetAuctionNftAddress(chatId);
            string auctionStartNftPrice = NewAuctionDB.GetAuctionStartNftPrice(chatId);
            string auctionRedemptionNftPrice = NewAuctionDB.GetAuctionRedemptionNftPrice(chatId);
            string auctionMinStep = NewAuctionDB.GetAuctionMinStep(chatId);
            string auctionValidUntil = NewAuctionDB.GetAuctionValidUntil(chatId).ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));

            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: $"<b>Адрес NFT: </b><code>{auctionNftAddress}</code>\n<b>Стартовая цена NFT: </b><code>{auctionStartNftPrice}</code>\n<b>Цена моментального выкупа NFT: </b><code>{auctionRedemptionNftPrice}</code>\n<b>Минимальный шаг повышения ставки: </b><code>{auctionMinStep}</code>\n<b>Время действия аукциона: </b><code>{auctionValidUntil}</code>",
                parseMode: ParseMode.Html,
                replyMarkup: Keyboards.NewAuctionKb
            );
        }
    }
}