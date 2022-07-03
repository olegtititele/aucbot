using Telegram.Bot;
using Telegram.Bot.Types.Enums;


using Requests;
using PostgreSQL;
using Bot_Keyboards;


namespace States
{
    public static class AuctionNftAddressState
    {
        public async static void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId)
        {
            string nftAddress = messageText;
            if(nftAddress.Length != 48)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>☠️ Ошибка при вводе данных. Введите действительный адрес NFT.</b>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToNewAuctionInfo
                );
            }
            else
            {
                if(CheckNftRight.Nft(nftAddress))
                {
                    NewAuctionDB.UpdateAuctionNftAddress(chatId, nftAddress);
                    UserDB.UpdateState(chatId, "MainMenu");
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>☑️ Успешно. Информация об NFT получена.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.BackToNewAuctionInfo
                    );
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>☠️ Ошибка при вводе данных. Введите действительный адрес NFT.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.BackToNewAuctionInfo
                    );
                }
            }
        }
    }
}