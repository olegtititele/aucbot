using PostgreSQL;
using Bot_Keyboards;
using Requests;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Modules
{
    public class CheckIfNftSentClass
    {
        public async static void CallDataCheckIfNftSent(ITelegramBotClient botClient, CallbackQuery callbackQuery, long chatId, int messageId, string username)
        {
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId:callbackQuery.Id,
                text: "☑️ Проверяем отправку NFT.",
                showAlert: false
            );
            
            if(CheckIfNftSent.Transaction(chatId))
            {
                string nftAddress = NewAuctionDB.GetAuctionNftAddress(chatId);
                string firstBet = NewAuctionDB.GetAuctionStartNftPrice(chatId);
                int minStep = Int32.Parse(NewAuctionDB.GetAuctionMinStep(chatId));
                string auctionRedemptionPrice = NewAuctionDB.GetAuctionRedemptionNftPrice(chatId);
                DateTime auctionValidUntil = NewAuctionDB.GetAuctionValidUntil(chatId);

                if(NewNftDB.AddNewNft(chatId, nftAddress, firstBet.ToString(), minStep.ToString(), auctionRedemptionPrice, auctionValidUntil))
                {
                    await botClient.EditMessageTextAsync(
                        chatId: chatId,
                        messageId: messageId,
                        text: "<b>✅ Поздравляем. Вашa NFT Выставлена на аукцион.</b>",
                        parseMode: ParseMode.Html
                    );
                    await botClient.SendTextMessageAsync(
                        chatId: Config.adminChannel,
                        text: $"<b>@{username} выставил NFT на аукцион.</b>",
                        parseMode: ParseMode.Html
                    );
                }
                else
                {
                    await botClient.EditMessageTextAsync(
                        chatId: chatId,
                        messageId: messageId,
                        text: $"<b>Данная NFT уже добавлена.</b>",
                        parseMode: ParseMode.Html
                    );
                }
            }
            else
            {
                await botClient.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: messageId,
                    text: $"<b>❌ Ошибка. NFT не найдена.\nДля создания аукциона отправьте свою NFT на кошелёк аукциона.\nАдрес кошелька: </b><code>{UserDB.GetTONWallet()}</code>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.SentNftToHold
                );
            }
        }
    }
}