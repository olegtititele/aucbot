using PostgreSQL;
using Bot_Keyboards;
using Requests;
using ConfigFile;
using Functions;
using Modules;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace CallbackHandler
{
    public static class CallHandler
    {
        public static int index = 0;
        public static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            
            try
            {
                long chatId = callbackQuery.Message!.Chat.Id;
                int messageId = callbackQuery.Message.MessageId;
                string? firstName = callbackQuery.Message.Chat.FirstName;
                string? username = callbackQuery.Message.Chat.Username;
                string nftName;
                List<string> nftCollections = NewNftDB.GetAllCollections();
                
                try
                {
                    int auctionMinStep = NewNftDB.GetMinStep(UserDB.GetCurrentNftCollection(chatId), UserDB.GetCurrentNft(chatId));
                    if(callbackQuery.Data == $"+{auctionMinStep}_TON")
                    {
                        Payments.RaiseBetStatic(botClient, chatId, messageId, auctionMinStep);
                        return;
                    }
                    else if(callbackQuery.Data == $"+{auctionMinStep*2}_TON")
                    {
                        Payments.RaiseBetStatic(botClient, chatId, messageId, auctionMinStep*2);
                        return;
                    }
                    else if(callbackQuery.Data == $"+{auctionMinStep*4}_TON")
                    {
                        Payments.RaiseBetStatic(botClient, chatId, messageId, auctionMinStep*4);
                        return;
                    }
                }
                catch(Exception){  }

                foreach (string nftCollection in nftCollections)
                {
                    if(callbackQuery.Data == nftCollection)
                    {
                        index = 0;
                        UserDB.UpdateCurrentNftCollection(chatId, nftCollection);

                        ShowNft.NftInfo(botClient, chatId, messageId, nftCollection, index);
                        return;
                    }
                }

                switch(callbackQuery.Data)
                {
                    case "change_toncoin_wallet":
                    // ИЗМЕНИТЬ TON КОШЕЛЕК
                        UserDB.UpdateState(chatId, "Wallet");
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: "<b>⤵️ Введите адрес вашего TON кошелька: </b>",
                            replyMarkup: Keyboards.BackToProfile,
                            parseMode: ParseMode.Html
                        );
                        return;
                    case "knowledge":
                    try
                    {
                        UserDB.UpdateState(chatId, "Wallet");
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: "<b>⤵️ Введите адрес вашего TON кошелька: </b>",
                            parseMode: ParseMode.Html
                        );
                    }
                    catch(Exception e){Console.WriteLine(e);}
                    // НАЖАТИЕ НА КНОПКУ "ОЗНАКОМЛЕН"
                        
                        return;
                    case "verification":
                    // ПРОЙТИ ВЕРИФИКАЦИЮ
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: "<b>Для верификации переведите содочка лав. После перевода нажмите проверить вериф </b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.VerifacationKb()
                        );
                        return;
                    case "create_an_auction":
                        NewAuction.GetNewAuctionInfo(botClient, chatId, messageId);
                        // string wallet = UserDB.GetWallet(chatId);
                        
                        // if(UserDB.GetVerificationOfWallet(wallet) == "none")
                        // {
                        //     await botClient.EditMessageTextAsync(
                        //         chatId: chatId,
                        //         messageId: messageId,
                        //         text: "<b>❕ Для того чтобы создать аукцион, вы должны верифицировать свой кошелёк.</b>",
                        //         parseMode: ParseMode.Html,
                        //         replyMarkup: Keyboards.BackToProfile
                        //     );
                        // }
                        // else
                        // {
                        //     NewAuction.GetNewAuctionInfo(botClient, chatId, messageId);
                        // }
                        return;
                    case "specify_the_address_of_nft":
                        UserDB.UpdateState(chatId, "AuctionNftAddress");
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: "<b>⤵️ Для того чтобы получить информацию о NFT укажите ее адрес.\n\n✔️ Пример: </b><code>EQB24QOD027_BYFcZJd5BXZkzWEBdeg5beIisgeRA3xAanK7</code>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToNewAuctionInfo
                        );
                        return;
                    case "specify_the_start_price_of_nft":
                        UserDB.UpdateState(chatId, "AuctionStartNftPrice");
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: "<b>⤵️ Укажите начальную ставку торга NFT. Минимальная цена 1 TON.\n\n✔️ Пример: </b><code>5</code>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToNewAuctionInfo
                        );
                        return;
                    case "specify_the_redemption_price":
                        UserDB.UpdateState(chatId, "AuctionRedemptionNftPrice");
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: "<b>⤵️ Укажите цену моментального выкупа NFT. Минимальная цена 1 TON.\n\n✔️ Пример: </b><code>1000</code>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToNewAuctionInfo
                        );
                        return;
                    case "specify_the_min_step":
                        UserDB.UpdateState(chatId, "AuctionMinStep");
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: "<b>⤵️ Укажите минимальный шаг повышения ставки торгов. Минимальная цена 1 TON.\n\n✔️ Пример: </b><code>10</code>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToNewAuctionInfo
                        );
                        return;
                    case "specify_valid_until":
                        UserDB.UpdateState(chatId, "AuctionValidUntil");
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: "<b>⤵️ Укажите время действия аукциона. Максимальная длительность аукциона ровно 1 неделя от текущего времени.\n\n✔️ Пример: </b><code>31.07.2022 17:25</code>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToNewAuctionInfo
                        );
                        return;
                    case "next_auction_step":
                        NextAuctionStep.CallDataNextAuctionStep(botClient, callbackQuery, chatId, messageId);
                        return;
                    case "check_if_nft_sent":
                        CheckIfNftSentClass.CallDataCheckIfNftSent(botClient, callbackQuery, chatId, messageId, username!);
                        return;
                    case "check_verification":
                    // ПРОВЕРИТЬ ВЕРИФИКАЦИЮ
                        CheckVerificationClass.CallDataCheckVerification(botClient, chatId, messageId, username!);
                        return;
                    case "next":
                    // СЛЕДУЮЩАЯ NFT
                        string nftCollection = UserDB.GetCurrentNftCollection(chatId);

                        if(index + 1 >= NewNftDB.NftInCollectionCount(nftCollection))
                        { 
                            index = 0;
                        }
                        else
                        {
                            index = index + 1;
                        }

                        ShowNft.NftInfo(botClient, chatId, messageId, nftCollection, index);
                        System.Threading.Thread.Sleep(1000);  
                        return;
                    case "previous":
                    // ПРЕДЫДУЩАЯ NFT
                        nftCollection = UserDB.GetCurrentNftCollection(chatId);

                        if(index - 1  >= NewNftDB.NftInCollectionCount(nftCollection))
                        { 
                            index = NewNftDB.GetAllNftNamesFromCollection(nftCollection).Count - 1;
                        }
                        else if(index - 1 < 0)
                        {
                            index = NewNftDB.GetAllNftNamesFromCollection(nftCollection).Count - 1;
                        }
                        else
                        {
                            index = index - 1;
                        }
                        
                        ShowNft.NftInfo(botClient, chatId, messageId, nftCollection, index);
                        System.Threading.Thread.Sleep(1000); 
                        return;
                    case "trade":
                    // ТОРГОВАТЬ 
                        nftName = UserDB.GetCurrentNft(chatId);
                        nftCollection = UserDB.GetCurrentNftCollection(chatId);

                        ShowNft.NftTradeKb(botClient, chatId, messageId, nftCollection, nftName);
                        return;
                    case "update_rate":
                    // ОБНОВИТЬ NFT
                        nftName = UserDB.GetCurrentNft(chatId);
                        nftCollection = UserDB.GetCurrentNftCollection(chatId);

                        try
                        {
                            ShowNft.NftTradeKb(botClient, chatId, messageId, nftCollection, nftName);
                        }
                        catch(Exception){ return; }
                        
                        return;
                    case "redeem_nft":
                    // ВЫКУПИТЬ NFT
                        nftName = UserDB.GetCurrentNft(chatId);
                        nftCollection = UserDB.GetCurrentNftCollection(chatId);
                        int redemptionPrice = NewNftDB.GetRedemptionPrice(nftCollection, nftName);
                        string payComment = $"{chatId}{NewNftDB.GetNftId(nftCollection, nftName)}";

                        UserDB.UpdatePopupSum(chatId, redemptionPrice.ToString());

                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: $"<b>❕ Нажмите на кнопку \"Оплата\" для перевода {redemptionPrice} TON.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.PaymentKb(payComment, redemptionPrice)
                        );

                        return;
                    case "make_a_bet":
                    // СДЕЛАТЬ СТАВКУ
                        UserDB.UpdateState(chatId, "InputNewBet");
                        await botClient.EditMessageReplyMarkupAsync(
                            chatId: chatId,
                            messageId: messageId,
                            replyMarkup: Keyboards.InputBetKb
                        );
                        return;
                    case "check_payment":
                    // ПРОВЕРИТЬ ОПЛАТУ
                        nftName = UserDB.GetCurrentNft(chatId);
                        nftCollection = UserDB.GetCurrentNftCollection(chatId);
                        int popupSum = UserDB.GetPopupSum(chatId);
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: "<b>☑️ Оплата на проверке. Ожидайте.</b>",
                            parseMode: ParseMode.Html
                        );

                        Payments.CheckPayment(botClient, chatId, username!, messageId, nftCollection, nftName, popupSum);
                        return;


                    // КНОПКИ НАЗАД //
                    case "back_to_collections":
                    // ВЕРНУТЬСЯ К КОЛЛЕКЦИЯМ NFT
                        int nftCollectionsNumber = NewNftDB.GetCollectionsNumber();
                        
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: $"<b>💎 Доступно на аукционе: {NewNftDB.GetCollectionsNumber()} {WalletFunction.GetLineCase(nftCollectionsNumber)}.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.NftCollectionsKb()
                        );
                        return;
                    
                    case "back_to_nft":
                    // ВЕРНУТЬСЯ К NFT
                        nftName = UserDB.GetCurrentNft(chatId);
                        nftCollection = UserDB.GetCurrentNftCollection(chatId);
                        int lastItemIndex = NewNftDB.NftInCollectionCount(nftCollection);
                        UserDB.UpdateCurrentNft(chatId, nftName);
                        string validUntil = NewNftDB.GetValidUntil(nftCollection, nftName).ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));

                        await botClient.EditMessageReplyMarkupAsync(
                            chatId: chatId,
                            messageId: messageId,
                            replyMarkup: Keyboards.NftKb(index, lastItemIndex, nftCollection, validUntil)
                        );
                        return;
                    case "back_to_auction":
                        UserDB.UpdateState(chatId, "MainMenu");
                        nftName = UserDB.GetCurrentNft(chatId);
                        nftCollection = UserDB.GetCurrentNftCollection(chatId);

                        ShowNft.NftTradeKb(botClient, chatId, messageId, nftCollection, nftName);
                        return;
                    case "back_from_input_bet":
                        UserDB.UpdateState(chatId, "MainMenu");
                        nftName = UserDB.GetCurrentNft(chatId);
                        nftCollection = UserDB.GetCurrentNftCollection(chatId);
                        ShowNft.NftTradeKb(botClient, chatId, messageId, nftCollection, nftName);
                        return;
                    case "back_to_profile":
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

                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: messageId,
                            text: $"<b>Пользователь: </b>@{username}\n<b>ID: </b><code>{chatId}</code>\n<b>Верификация: </b><code>{verifText}</code>\n\n<b>💼 Адрес кошелька: </b><code>{wallet}</code>\n<b>💲 Баланс: </b><code>{balance[0]}</code> TON | <code>{balance[1]}</code> USD",
                            parseMode: ParseMode.Html,
                            replyMarkup: kb
                        );
                        return;
                    case "back_to_new_auction":
                        NewAuction.GetNewAuctionInfo(botClient, chatId, messageId);
                        return;
                }
            }
            catch(Exception e){ Console.WriteLine(e); return; }
        }
    }
}