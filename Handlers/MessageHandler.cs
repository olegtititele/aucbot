using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;
using CommandsSpace;
using Requests;
using Functions;
using States;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

using System;
using System.Text.RegularExpressions;
using System.Globalization;



namespace MessageHandler
{
    public static class MessHandler
    {
        public static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            try
            {
                long chatId = message.Chat.Id;
                string? messageText = message.Text;
                int messageId = message.MessageId;
                string? username = message.Chat.Username;
                string? firstName = message.Chat.FirstName;

                
                
                if(messageText![0]=='/')
                {
                    if(messageText!.Contains("/start"))
                    {   
                        Commands.StartCommand(botClient, messageText, chatId, messageId, username!);
                    }
                    else
                    {
                        // Админ команды
                        if(chatId==Config.adminChannel)
                        {
                            Commands.AdminCommnads(botClient, messageText!, chatId, messageId);
                        }
                        else if(WalletFunction.CheckSubChannel(botClient.GetChatMemberAsync(Config.adminChannel, chatId).Result.Status.ToString()))
                        {
                            Commands.AdminCommnads(botClient, messageText!, chatId, messageId);
                        }
                        return;
                    }
                }

    
                switch(messageText)
                {
                    case "💎 Участвовать":
                        UserDB.UpdateState(chatId, "MainMenu");
                        int nftCollectionsNumber = NewNftDB.GetCollectionsNumber();
                        
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"<b>💎 Доступно на аукционе: {NewNftDB.GetCollectionsNumber()} {WalletFunction.GetLineCase(nftCollectionsNumber)}.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.NftCollectionsKb()
                        );
                        return;
                    case "❓ Информация":
                        UserDB.UpdateState(chatId, "MainMenu");
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"<b>💎Канал TON DOODLES PRESALE: @TonDoodlesNFT</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.InfoKb
                        );
                        return;
                    case "🙊 Профиль":
                        UserDB.UpdateState(chatId, "MainMenu");
                        string wallet = UserDB.GetWallet(chatId);
                        List<double> balance = GetWalletInfo.GetWalletBalance(wallet);
                        string verifText;
                        InlineKeyboardMarkup kb;

                        if(UserDB.GetVerificationOfWallet(wallet) == "none")
                        {
                            verifText = "❌";
                            kb = Keyboards.ProfileNotVerifKb;
                        }
                        else
                        {
                            verifText = "✅";
                            kb = Keyboards.ProfileVerifKb;
                        }

                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"<b>Пользователь: </b>@{username}\n<b>ID: </b><code>{chatId}</code>\n<b>Верификация: </b><code>{verifText}</code>\n\n<b>💼 Адрес кошелька: </b><code>{wallet}</code>\n<b>💲 Баланс: </b><code>{balance[0]}</code> TON | <code>{balance[1]}</code> USD",
                            parseMode: ParseMode.Html,
                            replyMarkup: kb
                        );
                        
                        return;
                }

                switch (UserDB.state(chatId))
                {
                    case "MainMenu":
                        return;
                    case "Wallet":
                        WalletState.MessageHandler(botClient, messageText, chatId, username!);
                        return;
                    case "InputNewBet":
                        InputNewBetState.MessageHandler(botClient, messageText, chatId);
                        return;
                    case "AuctionNftAddress":
                        AuctionNftAddressState.MessageHandler(botClient, messageText, chatId);
                        return;
                    case "AuctionStartNftPrice":
                        AuctionStartNftPriceState.MessageHandler(botClient, messageText, chatId);
                        return;
                    case "AuctionRedemptionNftPrice":
                        AuctionRedemptionNftPriceState.MessageHandler(botClient, messageText, chatId);
                        return;
                    case "AuctionMinStep":
                        AuctionMinStepState.MessageHandler(botClient, messageText, chatId);
                        return;
                    case "AuctionValidUntil":
                        AuctionValidUntilState.MessageHandler(botClient, messageText, chatId);
                        return;
                }
            }
            catch(Exception){ return; }
        }
    }
}
