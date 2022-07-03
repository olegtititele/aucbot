using Telegram.Bot.Types.ReplyMarkups;
using PostgreSQL;
using ConfigFile;

using System.Globalization;

namespace Bot_Keyboards
{
    class Keyboards
    {
        public static InlineKeyboardMarkup knowledge = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "✅ Ознакомлен", callbackData: "knowledge"),
            },
        });


        public static InlineKeyboardMarkup ProfileNotVerifKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "✔️ Верификация", callbackData: "verification"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Создать аукцион", callbackData: "create_an_auction"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "↪️ Изменить TON кошелёк", callbackData: "change_toncoin_wallet"),
            },
        });

        public static InlineKeyboardMarkup ProfileVerifKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Создать аукцион", callbackData: "create_an_auction"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "↪️ Изменить TON кошелёк", callbackData: "change_toncoin_wallet"),
            },
        });

        public static InlineKeyboardMarkup InputBetKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "⤵️ Введите количество TON:", callbackData: "---"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "↩️ Назад к торгам", callbackData: "back_to_auction"),
            },
        });

        public static InlineKeyboardMarkup BackToAuction = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "↩️ Назад к торгам", callbackData: "back_from_input_bet"),
            },
        });

        public static InlineKeyboardMarkup BackToProfile = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "↩️ Назад", callbackData: "back_to_profile"),
            },
        });

        public static InlineKeyboardMarkup NftCollectionsKb()
        {
            var rows = new List<InlineKeyboardButton[]>();
            List<string> nftCollections = NewNftDB.GetAllCollections();
            
            foreach (string nftCollection in nftCollections)
            {
                int nftInCollectionCount = NewNftDB.NftInCollectionCount(nftCollection);
                if(nftInCollectionCount == 0)
                {
                    continue;
                }
                else
                {
                    rows.Add(
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData($"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nftCollection.Replace("_"," "))} ({nftInCollectionCount})", $"{nftCollection}"),
                        }
                    );
                }
                
            }
            
            return rows.ToArray();
        }

        public static InlineKeyboardMarkup NftKb(int currentItemIndex, int lastItemIndex, string nftCollection, string validUntil)
        {
            

            int previousItemIndex;
            int nextItemIndex;
            if(currentItemIndex == 0)
            {
                previousItemIndex = lastItemIndex;
            }
            else
            {
                previousItemIndex = currentItemIndex;
            }

            if(currentItemIndex + 1 == lastItemIndex)
            {
                nextItemIndex = 1;
            }
            else
            {
                nextItemIndex = currentItemIndex + 2;
            }

            InlineKeyboardMarkup NftKb;

            if(NewNftDB.NftInCollectionCount(nftCollection) > 1)
            {
                NftKb = new(new []
                {
                    // new []
                    // {
                    //     InlineKeyboardButton.WithCallbackData(text: $"Начальная ставка: {startBet} TON", callbackData: "2"),
                    // },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"Дата окончания торгов ⤵️", callbackData: "---"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"{validUntil}", callbackData: "---"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "🛒 Торговать", callbackData: "trade"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"{previousItemIndex} | {lastItemIndex}", callbackData: "previous"),
                        InlineKeyboardButton.WithCallbackData(text: "↩️", callbackData: "back_to_collections"),
                        InlineKeyboardButton.WithCallbackData(text: $"{nextItemIndex} | {lastItemIndex}", callbackData: "next"),
                    },
                });
            }
            else
            {
                NftKb = new(new []
                {
                    // new []
                    // {
                    //     InlineKeyboardButton.WithCallbackData(text: $"Начальная ставка: {startBet} TON", callbackData: "2"),
                    // },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"Дата окончания торгов ⤵️", callbackData: "---"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"{validUntil}", callbackData: "---"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "🛒 Торговать", callbackData: "trade"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "↩️ Назад", callbackData: "back_to_collections"),
                    },
                });
            }
            
            return NftKb;
        }

        public static InlineKeyboardMarkup TradeKb(long chatId, string nftCollection, string nftName, string startBet, string lastBet, int redemptionPrice, int minStep)
        {

            InlineKeyboardMarkup TradeKb;

            if(NewNftDB.GetLastBetFromUser(nftCollection, nftName) == chatId)
            {
                TradeKb = new(new []
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"Начальная ставка: {startBet} TON", callbackData: "2"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"🏆 Ваша ставка лидирует: {lastBet} TON", callbackData: "2"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"Выкупить NFT: {redemptionPrice} TON", callbackData: "redeem_nft"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"+{minStep} TON", callbackData: $"+{minStep}_TON"),
                        InlineKeyboardButton.WithCallbackData(text: $"+{minStep*2} TON", callbackData: $"+{minStep*2}_TON"),
                        InlineKeyboardButton.WithCallbackData(text: $"+{minStep*4} TON", callbackData: $"+{minStep*4}_TON"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Обновить", callbackData: "update_rate"),
                        InlineKeyboardButton.WithCallbackData(text: "Повысить ставку", callbackData: "make_a_bet"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "↩️ Назад к NFT", callbackData: "back_to_nft"),
                    },
                });
            }
            else
            {
                TradeKb = new(new []
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"Начальная ставка: {startBet} TON", callbackData: "2"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"Текущая ставка: {lastBet} TON", callbackData: "2"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"Выкупить NFT: {redemptionPrice} TON", callbackData: "redeem_nft"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: $"+{minStep} TON", callbackData: $"+{minStep}_TON"),
                        InlineKeyboardButton.WithCallbackData(text: $"+{minStep*2} TON", callbackData: $"+{minStep*2}_TON"),
                        InlineKeyboardButton.WithCallbackData(text: $"+{minStep*4} TON", callbackData: $"+{minStep*4}_TON"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Обновить", callbackData: "update_rate"),
                        InlineKeyboardButton.WithCallbackData(text: "Повысить ставку", callbackData: "make_a_bet"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "↩️ Назад к NFT", callbackData: "back_to_nft"),
                    },
                });
            }
            
            return TradeKb;
        }

        
        public static ReplyKeyboardMarkup MenuKb = new(new []
        {
            new KeyboardButton[] {"💎 Участвовать"},
            new KeyboardButton[] {"🙊 Профиль", "❓ Информация"},
        })
        {
            ResizeKeyboard = true,
        };

        public static InlineKeyboardMarkup InfoKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithUrl(text: "💭 Чат пресейла", url: "https://t.me/+zT2Fgh0M_hc4YmNk"),
            }
        });

        public static InlineKeyboardMarkup VerifacationKb()
        {
            string paymentLink = $"ton://transfer/{UserDB.GetTONWallet()}?amount=1000000000&text={Config.verifComment}";

            InlineKeyboardMarkup verifacationKb = new(new []
            {
                new[]
                {
                    InlineKeyboardButton.WithUrl(text: "Верификация", url: $"{paymentLink}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Проверить верификацию", callbackData: "check_verification"),
                },
            });
            return verifacationKb;
        }

        public static InlineKeyboardMarkup PaymentKb(string payComment, int popupSum)
        {
            
            string paymentLink = $"ton://transfer/{UserDB.GetTONWallet()}?amount={popupSum}000000000&text={payComment}";

            InlineKeyboardMarkup paymentKb = new(new []
            {
                new[]
                {
                    InlineKeyboardButton.WithUrl(text: "Оплата", url: $"{paymentLink}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Проверить оплату", callbackData: "check_payment"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "↩️ Назад к торгам", callbackData: "back_from_input_bet"),
                },
            });
            return paymentKb;
        }

        public static InlineKeyboardMarkup NewAuctionKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Изменить адресс NFT", callbackData: "specify_the_address_of_nft"),
            },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "Тип торгов \"Продажа\"", callbackData: "---"),
            // },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Изменить стартовую цену NFT", callbackData: "specify_the_start_price_of_nft"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Изменить цену моментального выкупа", callbackData: "specify_the_redemption_price"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Изменить минимальный шаг", callbackData: "specify_the_min_step"),
            },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "Укажите комментарий продавца", callbackData: "specify_seller_comment"),
            // },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Изменить время действия аукциона", callbackData: "specify_valid_until"),
            },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "Отправить NFT на кошелёк", callbackData: "send_nft"),
            // },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "↩️ Назад", callbackData: "back_to_profile"),
                InlineKeyboardButton.WithCallbackData(text: "Следующий шаг ↪️", callbackData: "next_auction_step"),
                
            },
        });

        public static InlineKeyboardMarkup BackToNewAuctionInfo = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "↩️ Назад", callbackData: "back_to_new_auction"),
            },
        });

        public static InlineKeyboardMarkup SentNftToHold = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Проверить отправку", callbackData: "check_if_nft_sent"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "↩️ Назад", callbackData: "back_to_new_auction"),
            },
        });
    }
}
