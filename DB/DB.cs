using Npgsql;

using Requests;
using System.Globalization;
namespace PostgreSQL
{
    public static class UserDB
    {
        private static string user_db_connection = DBConfig.user_db;

        public static void TruncateTable()
        {
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            cmd.CommandText = $"TRUNCATE TABLE users_table";
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static int GetPopupSum(long userId)
        {
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();
            var sql = $"SELECT popup_sum FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar()!.ToString();

            return Int32.Parse(result!);
        }
        
        public static void UpdatePopupSum(long userId, string popupSum)
        {
            using (var conn = new NpgsqlConnection(user_db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET popup_sum = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", popupSum);
                    int nRows = command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public static string GetCurrentNftCollection(long userId)
        {
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();
            var sql = $"SELECT nft_collection FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();

            return result!.ToString()!;
        }
        public static void UpdateCurrentNftCollection(long userId, string nftCollection)
        {
            using (var conn = new NpgsqlConnection(user_db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET nft_collection = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", nftCollection);
                    int nRows = command.ExecuteNonQuery();
                    
                }
            }
        }

        public static string GetCurrentNft(long userId)
        {
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();
            var sql = $"SELECT current_nft FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            
            return result!.ToString()!;
        }
        public static void UpdateCurrentNft(long userId, string currentNft)
        {
            using (var conn = new NpgsqlConnection(user_db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET current_nft = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", currentNft);
                    int nRows = command.ExecuteNonQuery();
                    
                }
            }
        }

        // public static List<string> GetAllWallets()
        // {
        //     List<string> allWallets= new List<string>();
        //     using var con = new NpgsqlConnection(user_db_connection);
        //     con.Open();
        //     var sql = $"SELECT wallet FROM users_table";
        //     using var cmd = new NpgsqlCommand(sql, con);
        //     cmd.ExecuteNonQuery();
        //     using (NpgsqlDataReader reader = cmd.ExecuteReader())
        //     {
        //         while (reader.Read())
        //         {
        //             allWallets.add(reader.GetString(0));
        //         }
        //         con.Close();
        //         return allWallets;
        //     }    
        // }


        public static void AddNewWalletToVerificationTable(string wallet)
        {
            string verification = "none";
            
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            cmd.CommandText = $"INSERT INTO verification_of_wallets (wallet, verification) VALUES (@wallet, @verification)";
            cmd.Parameters.AddWithValue("@wallet", wallet);
            cmd.Parameters.AddWithValue("@verification", verification);


            cmd.ExecuteNonQuery();
        }

        public static bool CheckWalletInVerificationTable(string wallet)
        {
            try
            {
                using var con = new NpgsqlConnection(user_db_connection);
                con.Open();
                var sql = $"SELECT * FROM verification_of_wallets WHERE wallet = '{wallet}'";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar()!.ToString();
                con.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void VerificateWallet(string wallet)
        {
            using (var conn = new NpgsqlConnection(user_db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE verification_of_wallets SET verification = @q WHERE wallet = @n", conn))
                {
                    command.Parameters.AddWithValue("n", wallet);
                    command.Parameters.AddWithValue("q", "yes");
                    int nRows = command.ExecuteNonQuery();  
                }
            }
        }

        public static string GetVerificationOfWallet(string wallet)
        {
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();
            var sql = $"SELECT verification FROM verification_of_wallets WHERE wallet = '{wallet}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }

        // Получить все объявления
        public static List<List<string>> GetAllNfts()
        {
            List<List<string>> AllNfts = new List<List<string>>();
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();
            var sql = $"SELECT * FROM nft_collection";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    List<string> list= new List<string>();
                    list.Add(reader.GetString(0));
                    list.Add(reader.GetString(1));
                    list.Add(reader.GetString(2));
                    list.Add(reader.GetString(3));
                    list.Add(reader.GetString(4));
                    list.Add(reader.GetString(5));
                    list.Add(reader.GetString(6));
                    AllNfts.Add(list);
                }
                con.Close();

                return AllNfts;
            }
        }

        // Получить количество nft
        public static int NftCount()
        {
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();
            var sql = $"SELECT * FROM nft_collection";
            using var cmd = new NpgsqlCommand(sql, con);
            var reader = cmd.ExecuteReader();
            int length = 0;
            while (reader.Read())
            {
                length++;
            }
            return length;
        }





        // Получить ton кошелек
        public static string GetTONWallet()
        {
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();
            var sql = $"SELECT ton_wallet FROM nft_information";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }

        // Обновить ton кошелек
        public static void UpdateTONWallet(string TONWallet)
        {
            using (var conn = new NpgsqlConnection(user_db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE nft_information SET ton_wallet = @q", conn))
                {
                    command.Parameters.AddWithValue("q", TONWallet);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }

        // Получить кол-во NFT
        public static string GetTotalNftNumber()
        {
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();
            var sql = $"SELECT number_of_nft FROM nft_information";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }

        // Обновить кол-во NFT
        public static void UpdateTotalNftNumber(string nftNumber)
        {
            using (var conn = new NpgsqlConnection(user_db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE nft_information SET number_of_nft = @q", conn))
                {
                    command.Parameters.AddWithValue("q", nftNumber);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }

        // Добавить кол-во NFT
        public static void AddTotalNftNumber(string nftNumber)
        {
            int newTotalNftNumber = Int32.Parse(GetTotalNftNumber()) + Int32.Parse(nftNumber);
            using (var conn = new NpgsqlConnection(user_db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE nft_information SET number_of_nft = @q", conn))
                {
                    command.Parameters.AddWithValue("q", newTotalNftNumber);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }
        
        
        // Проверка юзера на наличие
        public static bool CheckUser(long user_id)
        {
            try
            {
                using var con = new NpgsqlConnection(user_db_connection);
                con.Open();
                var sql = $"SELECT * FROM users_table WHERE user_id = '{user_id.ToString()}'";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar()!.ToString();
                con.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static void CreateUser(long userId, string username)
        {
            string state = "MainMenu";
            string wallet = "Не указан";
            string nftCollection = "none";
            string currentNft = "none";
            int popupSum = 0;
            string auctionNftAddress = "none";
            string auctionStartNftPrice = "none";
            string auctionRedemptionPrice = "none";
            string auctionMinStep = "none";
            DateTime auctionValidUntil = DateTime.Now.AddDays(2);
            string auctionCheckIfNftSent = "none";
            
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            cmd.CommandText = $"INSERT INTO users_table (user_id, username, state, wallet, nft_collection, current_nft, popup_sum, auction_nft_address, auction_start_nft_price, auction_redemption_price, auction_min_step, auction_valid_until, auction_check_if_nft_sent) VALUES (@user_id, @username, @state, @wallet, @nft_collection, @current_nft, @popup_sum, @auction_nft_address, @auction_start_nft_price, @auction_redemption_price, @auction_min_step, @auction_valid_until, @auction_check_if_nft_sent)";

            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@state", state);
            cmd.Parameters.AddWithValue("@wallet", wallet);
            cmd.Parameters.AddWithValue("@nft_collection", nftCollection);
            cmd.Parameters.AddWithValue("@current_nft", currentNft);
            cmd.Parameters.AddWithValue("@popup_sum", popupSum);
            cmd.Parameters.AddWithValue("@auction_nft_address", auctionNftAddress);
            cmd.Parameters.AddWithValue("@auction_start_nft_price", auctionStartNftPrice);
            cmd.Parameters.AddWithValue("@auction_redemption_price", auctionRedemptionPrice);
            cmd.Parameters.AddWithValue("@auction_min_step", auctionMinStep);
            cmd.Parameters.AddWithValue("@auction_valid_until", auctionValidUntil);
            cmd.Parameters.AddWithValue("@auction_check_if_nft_sent", auctionCheckIfNftSent);


            cmd.ExecuteNonQuery();
        }


        // Получить STATE юзера
        public static string state(long user_id)
        {
            try
            {
                using var con = new NpgsqlConnection(user_db_connection);
                con.Open();
                var sql = $"SELECT state FROM users_table WHERE user_id = '{user_id.ToString()}'";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar();
                return result!.ToString()!;
            }
            catch(Exception)
            {
                return "MainMenu";
            }
        }


        // Обновить STATE
        public static void UpdateState(long userId, string state)
        {
            using (var conn = new NpgsqlConnection(user_db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET state = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", state);
                    int nRows = command.ExecuteNonQuery();
                    
                }
            }
        }


        // Проверка кошелька
        public static bool CheckWallet(string wallet)
        {
            try
            {
                using var con = new NpgsqlConnection(user_db_connection);
                con.Open();
                var sql = $"SELECT * FROM users_table WHERE wallet = '{wallet}'";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar()!.ToString();
                con.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Получить Wallet
        public static string GetWallet(long userId)
        {
            using var con = new NpgsqlConnection(user_db_connection);
            con.Open();
            var sql = $"SELECT wallet FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }

        // Обновить Wallet
        public static void UpdateWallet(long userId, string wallet)
        {
            using (var conn = new NpgsqlConnection(user_db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET wallet = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", wallet);
                    int nRows = command.ExecuteNonQuery();
                    
                }
            }
        }
    }
}
