using PostgreSQL;
using System;
namespace ConfigFile
{
    public class Config
    {
        public static string botToken = "5428664214:AAHzHQAdvgTLWVCfbivB8e6c2Tb_H5evhjE";
        public static long adminChannel = -1001643852844;
        public static string walletSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";
        public static string verifComment = "Verification for TONDOT";
        public static string auctionRulesText = "Сюда правила аукциона";
        public static string startText = "Сюда текст при команде /start, когда юзер уже добавлен в бд и указал кошелек";

        public static DateTime EndDate()
        {
            var thTH = new System.Globalization.CultureInfo("de-De");
            var calendar = thTH.DateTimeFormat.Calendar;
            DateTime endDate = new DateTime(2022, 7, 20, 00, 00, 00, calendar);
            return endDate;
        }

        
        // Админ команды
        public static Dictionary<string, string> AdminCommnads = new Dictionary<string, string>()
        {
            {"/get_total_nft", "посмотреть общее кол-во NFT"},
            {"/updade_total_nft кол-во", "обновить общее кол-во NFT"},
            {"/add_to_total_nft кол-во", "добавить к общему кол-во NFT"},
            {"/update_ton_wallet кошелёк", "обновить TON кошелёк"},
            {"/get_ton_wallet", "посмотреть TON кошелёк"},
            {"/add_new_nft адресс NFT::начальная ставка::минимальный шаг::цена выкупа::время действия", "добавить новое NFT"}
        };
    }
}
