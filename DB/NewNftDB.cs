using Npgsql;

using Requests;

using System.Text.RegularExpressions;

namespace PostgreSQL
{
    public class NewNftDB
    {
        private static string db_connection = DBConfig.nft_db;

        static string GenerateIdentifier()
        {
            char[] letters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_".ToCharArray();

            Random rand = new Random();

            string identifier = "";
            for (int i = 1; i <= 15; i++)
            {
                int letterNum = rand.Next(0, letters.Length - 1);
                identifier += letters[letterNum];
            }
            return identifier;
        }
        public static bool CheckNft(string nftCollection, string nftName)
        {
            try
            {
                using var con = new NpgsqlConnection(db_connection);
                con.Open();
                var sql = $"SELECT * FROM {nftCollection} WHERE nft_name = '{nftName}'";
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
        public static bool AddNewNft(long chatId, string nftAddress, string firstBet, string minStep, string redemptionPrice, DateTime validUntil)
        {
            List<string> nftInfo = GetNftInfo.GetNft(nftAddress);
            string collectionName = nftInfo[0];
            string nftName = nftInfo[1];
            string nftImage = nftInfo[2];
            string nftDescription = nftInfo[3];
            string nftAttributes = nftInfo[4];
            string identifier = GenerateIdentifier();
            
            Regex rgx = new Regex("[^a-zA-Z0-9 _]");
            string tableName = rgx.Replace(collectionName.Replace(" ", "_").ToLower(), "");

            if(CheckNftCollection(tableName))
            {

            }
            else
            {
                CreateNftCollection(tableName);
            }

            if(CheckNft(tableName, nftName))
            {
                return false;
            }
            else
            {
                using var con = new NpgsqlConnection(db_connection);
                con.Open();

                using var cmd = new NpgsqlCommand();
                cmd.Connection = con;

                
                cmd.CommandText = $"INSERT INTO {tableName} (nft_from_user_id, nft_name, nft_address, nft_img, nft_description, nft_collection, nft_id, first_bet, last_bet, min_step, last_bet_from_user, redemption_price, attributes, valid_until) VALUES (@nft_from_user_id, @nft_name, @nft_address, @nft_img, @nft_description, @nft_collection, @nft_id, @first_bet, @last_bet, @min_step, @last_bet_from_user, @redemption_price, @attributes, @valid_until)";
                
                cmd.Parameters.AddWithValue("@nft_from_user_id", chatId);
                cmd.Parameters.AddWithValue("@nft_name", nftName);
                cmd.Parameters.AddWithValue("@nft_address", nftAddress);
                cmd.Parameters.AddWithValue("@nft_img", nftImage);
                cmd.Parameters.AddWithValue("@nft_description", nftDescription);
                cmd.Parameters.AddWithValue("@nft_collection", collectionName);
                cmd.Parameters.AddWithValue("@nft_id", identifier);
                cmd.Parameters.AddWithValue("@first_bet", firstBet);
                cmd.Parameters.AddWithValue("@last_bet", firstBet);
                cmd.Parameters.AddWithValue("@min_step", minStep);
                cmd.Parameters.AddWithValue("@last_bet_from_user", "none");
                cmd.Parameters.AddWithValue("@redemption_price", redemptionPrice);
                cmd.Parameters.AddWithValue("@attributes", nftAttributes);
                cmd.Parameters.AddWithValue("@valid_until", validUntil);

                cmd.ExecuteNonQuery();
                return true;
            }
        }


        public static bool CheckNftCollection(string collectionName)
        {
            try
            {
                using var con = new NpgsqlConnection(db_connection);
                con.Open();
                
                var sql = $"SELECT * FROM {collectionName.Replace(" ", "_").ToLower()}";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar()!.ToString();
                con.Close();
                return true;
            }
            catch (System.NullReferenceException)
            {
                return true;
            }
            catch (Npgsql.PostgresException)
            {
                return false;
            }
        }

        public static void CreateNftCollection(string collectionName)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            cmd.CommandText = $"CREATE TABLE {collectionName} (nft_from_user_id VARCHAR(255), nft_name VARCHAR(255), nft_address VARCHAR(255), nft_img VARCHAR(255), nft_description VARCHAR(2000), nft_collection VARCHAR(255), nft_id VARCHAR(255), first_bet VARCHAR(255), last_bet VARCHAR(255), min_step VARCHAR(255), last_bet_from_user VARCHAR(255), redemption_price VARCHAR(255), attributes VARCHAR(10000), valid_until VARCHAR(255))";
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static List<string> GetAllCollections()
        {
            List<string> nftCollections = new List<string>();
            
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT table_name FROM information_schema.tables  WHERE table_schema='public' ORDER BY table_name;";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    nftCollections.Add(reader.GetString(0));
                }
                con.Close();
            }

            return nftCollections;
        }

        public static List<string> GetAllNftNamesFromCollection(string nftCollection)
        {
            List<string> nftNames = new List<string>();
            
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT nft_name FROM {nftCollection}";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    nftNames.Add(reader.GetString(0));
                }
                con.Close();
            }

            return nftNames;
        }

        public static List<string> GetNft(string nftCollection, string nftName)
        {
            List<string> nftInfo = new List<string>();
            
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT * FROM {nftCollection} WHERE nft_name = '{nftName}'";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    nftInfo.Add(reader.GetString(0));
                    nftInfo.Add(reader.GetString(1));
                    nftInfo.Add(reader.GetString(2));
                    nftInfo.Add(reader.GetString(3));
                    nftInfo.Add(reader.GetString(4));
                    nftInfo.Add(reader.GetString(5));
                    nftInfo.Add(reader.GetString(6));
                    nftInfo.Add(reader.GetString(7));
                    nftInfo.Add(reader.GetString(8));
                    nftInfo.Add(reader.GetString(9));
                    nftInfo.Add(reader.GetString(10));
                    nftInfo.Add(reader.GetString(11));
                    nftInfo.Add(reader.GetString(12));
                    nftInfo.Add(reader.GetString(13));
                }
                con.Close();
            }

            return nftInfo;
        }

        public static int GetCollectionsNumber()
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT table_name FROM information_schema.tables  WHERE table_schema='public' ORDER BY table_name;";
            using var cmd = new NpgsqlCommand(sql, con);
            var reader = cmd.ExecuteReader();
            int length = 0;
            while (reader.Read())
            {
                length++;
            }
            return length;
        }

        public static int NftInCollectionCount(string nftCollection)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT * FROM {nftCollection}";
            using var cmd = new NpgsqlCommand(sql, con);
            var reader = cmd.ExecuteReader();
            int length = 0;
            while (reader.Read())
            {
                length++;
            }
            return length;
        }

        public static DateTime GetValidUntil(string nftCollection, string nftName)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT valid_until FROM {nftCollection} WHERE nft_name = '{nftName}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            
            return Convert.ToDateTime(result);
        }

        public static long GetLastBetFromUser(string nftCollection, string nftName)
        {
            try
            {
                using var con = new NpgsqlConnection(db_connection);
                con.Open();
                var sql = $"SELECT last_bet_from_user FROM {nftCollection} WHERE nft_name = '{nftName}'";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar()!.ToString();
                
                return Int64.Parse(result!);
            }
            catch(Exception){ return 228; }
            
        }

        public static string GetNftId(string nftCollection, string nftName)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT nft_id FROM {nftCollection} WHERE nft_name = '{nftName}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar()!.ToString();
            
            return result!;
        }

        public static int GetLastBet(string nftCollection, string nftName)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT last_bet FROM {nftCollection} WHERE nft_name = '{nftName}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar()!.ToString();
            
            return Int32.Parse(result!);
        }

        public static int GetMinStep(string nftCollection, string nftName)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT min_step FROM {nftCollection} WHERE nft_name = '{nftName}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar()!.ToString();
            
            return Int32.Parse(result!);
        }

        public static string GetAttributes(string nftCollection, string nftName)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT attributes FROM {nftCollection} WHERE nft_name = '{nftName}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar()!.ToString();
            
            return result!;
        }

        public static int GetRedemptionPrice(string nftCollection, string nftName)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT redemption_price FROM {nftCollection} WHERE nft_name = '{nftName}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar()!.ToString();
            
            return Int32.Parse(result!);
        }

        public static void UpdateRedemptionPrice(string nftCollection, string nftName, int newPrice)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand($"UPDATE {nftCollection} SET redemption_price = @q WHERE nft_name = @n", conn))
                {
                    command.Parameters.AddWithValue("n", nftName);
                    command.Parameters.AddWithValue("q", newPrice);
                    int nRows = command.ExecuteNonQuery();
                }
                conn.Close();
            }
            
        }

        public static void UpdateLastBet(string nftCollection, string nftName, long chatId, int bet)
        {
            int newBet = GetLastBet(nftCollection, nftName) + bet;
            int redemtionPrice = GetRedemptionPrice(nftCollection, nftName);
            if(newBet >= redemtionPrice)
            {
                int newPrice = newBet + (newBet / 2);
                UpdateRedemptionPrice(nftCollection, nftName, newPrice);
            }
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand($"UPDATE {nftCollection} SET last_bet = @q WHERE nft_name = @n", conn))
                {
                    command.Parameters.AddWithValue("n", nftName);
                    command.Parameters.AddWithValue("q", newBet);
                    int nRows = command.ExecuteNonQuery();
                }

                using (var command = new NpgsqlCommand($"UPDATE {nftCollection} SET last_bet_from_user = @q WHERE nft_name = @n", conn))
                {
                    command.Parameters.AddWithValue("n", nftName);
                    command.Parameters.AddWithValue("q", chatId);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }
    }
}