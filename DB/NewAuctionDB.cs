using Npgsql;

using Requests;


namespace PostgreSQL
{
    public class NewAuctionDB
    {
        private static string db_connection = DBConfig.user_db;

        public static List<string> GetNewAuctionInfo(long userId)
        {
            List<string> NewAuctionInfo= new List<string>();
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT auction_nft_address, auction_start_nft_price, auction_redemption_price, auction_min_step, auction_valid_until FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    NewAuctionInfo.Add(reader.GetString(0));
                    NewAuctionInfo.Add(reader.GetString(1));
                    NewAuctionInfo.Add(reader.GetString(2));
                    NewAuctionInfo.Add(reader.GetString(3));
                    NewAuctionInfo.Add(reader.GetString(4));
                }
                con.Close();
                return NewAuctionInfo;
            }
        }

        // Получить адрес nft для аукциона
        public static string GetAuctionNftAddress(long userId)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT auction_nft_address FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();

            return result!.ToString()!;
        }

        // Обновить адрес nft для аукциона
        public static void UpdateAuctionNftAddress(long userId, string auctionNftAddress)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET auction_nft_address = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", auctionNftAddress);
                    int nRows = command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        // Получить стартовую цену nft
        public static string GetAuctionStartNftPrice(long userId)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT auction_start_nft_price FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();

            return result!.ToString()!;
        }


        // Обновить стартовую цену nft
        public static void UpdateAuctionStartNftPrice(long userId, string auctionStartNftPrice)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET auction_start_nft_price = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", auctionStartNftPrice);
                    int nRows = command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        // Получить цену выкупа nft
        public static string GetAuctionRedemptionNftPrice(long userId)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT auction_redemption_price FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();

            return result!.ToString()!;
        }

        // Обновить цену выкупа nft
        public static void UpdateAuctionRedemptionNftPrice(long userId, string redemptionPrice)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET auction_redemption_price = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", redemptionPrice);
                    int nRows = command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        // Получить минимальный шаг nft
        public static string GetAuctionMinStep(long userId)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT auction_min_step FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();

            return result!.ToString()!;
        }

        // Обновить минимальный шаг nft
        public static void UpdateAuctionMinStep(long userId, string minStepPrice)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET auction_min_step = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", minStepPrice);
                    int nRows = command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        // Получить дату окончания аукциона
        public static DateTime GetAuctionValidUntil(long userId)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT auction_valid_until FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();

            return Convert.ToDateTime(result);
        }

        // Обновить дату окончания аукциона
        public static void UpdateAuctionValidUntil(long userId, DateTime validUntil)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET auction_valid_until = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", validUntil);
                    int nRows = command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        // Получить дату окончания аукциона
        public static string GetIfNftSent(long userId)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT auction_check_if_nft_sent FROM users_table WHERE user_id = '{userId.ToString()}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar()!;

            return result.ToString()!;
        }

        // Обновить дату окончания аукциона
        public static void UpdateIfNftSent(long userId, string ifSent)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET auction_check_if_nft_sent = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId.ToString());
                    command.Parameters.AddWithValue("q", ifSent);
                    int nRows = command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }
    }
}