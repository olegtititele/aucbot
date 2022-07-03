using PostgreSQL;
using ConfigFile;
using Bot_Keyboards;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;


namespace CommandsSpace
{
    public static class Commands
    {
        async public static void AdminCommnads(ITelegramBotClient botClient, string messageText, long chatId, int messageId)
        {
            if(messageText == "/help")
            {
                string commandsText = "";
                foreach(var command in Config.AdminCommnads)
                {
                    commandsText += $"\n\n<code>{command.Key}</code> - {command.Value}";
                }
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "<b>🚀 Команды управления проектом: </b>" + commandsText,
                    parseMode: ParseMode.Html
                );
            }
            else if(messageText!.Contains("/get_total_nft"))
            {
                try
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>Общее кол-во NFT: </b><code>{UserDB.GetTotalNftNumber()}</code>",
                        parseMode: ParseMode.Html
                    );
                }
                catch(Exception)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"❗️ Введите команду корректно.",
                        parseMode: ParseMode.Html
                    );
                }
            }
            else if(messageText!.Contains("/updade_total_nft "))
            {
                try
                {
                    string newTotalNft = messageText.Split(" ")[1];
                    UserDB.UpdateTotalNftNumber(newTotalNft);
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>Общее кол-во NFT обновлено на: </b><code>{newTotalNft}</code>",
                        parseMode: ParseMode.Html
                    );
                }
                catch(Exception)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"❗️ Введите команду корректно.",
                        parseMode: ParseMode.Html
                    );
                }
            }
            else if(messageText!.Contains("/add_to_total_nft "))
            {
                try
                {
                    string newTotalNft = messageText.Split(" ")[1];
                    UserDB.AddTotalNftNumber(newTotalNft);
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>Общее кол-во NFT обновлено на: </b><code>{UserDB.GetTotalNftNumber()}</code>",
                        parseMode: ParseMode.Html
                    );
                }
                catch(Exception)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"❗️ Введите команду корректно.",
                        parseMode: ParseMode.Html
                    );
                }
            }
            else if(messageText!.Contains("/update_ton_wallet "))
            {
                try
                {
                    string newTONWallet = messageText.Split(" ")[1];
                    UserDB.UpdateTONWallet(newTONWallet);
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>TON кошелёк обновлен на: </b><code>{UserDB.GetTONWallet()}</code>",
                        parseMode: ParseMode.Html
                    );
                }
                catch(Exception)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"❗️ Введите команду корректно.",
                        parseMode: ParseMode.Html
                    );
                }
            }
            else if(messageText!.Contains("/get_ton_wallet"))
            {
                try
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>TON кошелёк: </b><code>{UserDB.GetTONWallet()}</code>",
                        parseMode: ParseMode.Html
                    );
                }
                catch(Exception)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"❗️ Введите команду корректно.",
                        parseMode: ParseMode.Html
                    );
                }
            }
            else if(messageText!.Contains("/add_new_nft "))
            {
                try
                {
                    string nftAddress = messageText.Split(" ")[1].Split("::")[0];
                    int firstBet = Int32.Parse(messageText.Split(" ")[1].Split("::")[1]);
                    int minStep = Int32.Parse(messageText.Split(" ")[1].Split("::")[2]);
                    int redemptionPrice = Int32.Parse(messageText.Split(" ")[1].Split("::")[3]);
                    DateTime validUntil = Convert.ToDateTime(messageText.Split(" ")[1].Split("::")[4]);
                    if(NewNftDB.AddNewNft(chatId, nftAddress, firstBet.ToString(), minStep.ToString(), redemptionPrice.ToString(), validUntil))
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"<b>NFT добавлена</b>",
                            parseMode: ParseMode.Html
                        );
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"<b>Данная NFT уже добавлена.</b>",
                            parseMode: ParseMode.Html
                        );
                    }
                    
                }
                catch(Exception)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"❗️ Введите команду корректно.",
                        parseMode: ParseMode.Html
                    );
                }
            }
        }

        async public static void StartCommand(ITelegramBotClient botClient, string messageText, long chatId, int messageId, string username)
        {
            if(chatId.ToString()[0]=='-'){ return; }
            else if(UserDB.CheckUser(chatId))
            {
                if(UserDB.GetWallet(chatId)=="Не указан")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: Config.auctionRulesText,
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.knowledge
                    );
                }
                else
                {
                    UserDB.UpdateState(chatId, "MainMenu");
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>💎Канал TON DOODLES PRESALE: @TonDoodlesNFT</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.MenuKb
                    );
//                     await botClient.SendTextMessageAsync(
//                         chatId: chatId,
//                         text: Config.startText,
//                         parseMode: ParseMode.Html,
//                         replyMarkup: Keyboards.MenuKb
//                     );
                }
                return;
            }
            else
            {
                try
                {
                    UserDB.CreateUser(chatId, username!);
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: Config.auctionRulesText,
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.knowledge
                    );

                }
                catch(Exception)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>💎 Для работы с ботом нужно указать @username.</b>",
                        parseMode: ParseMode.Html
                    );
                }
                return;
            }
        }
    }
}
