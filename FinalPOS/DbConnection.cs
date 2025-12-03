using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace FinalPOS
{
    public class DBConnection
    {
        private MySqlConnection cn;
        private readonly string _connectionString;

        public DBConnection()
        {
            _connectionString = DatabaseInitializer.ConnectionString;
            cn = new MySqlConnection(_connectionString);
        }

        public MySqlConnection Connection => cn;

        public void Open()
        {
            if (cn.State == ConnectionState.Closed)
                cn.Open();
        }

        public void Close()
        {
            if (cn.State == ConnectionState.Open)
                cn.Close();
        }

        public string MyConnection()
        {
            return _connectionString;
        }

        public MySqlConnection OpenConnection()
        {
            if (cn.State == ConnectionState.Closed)
            {
                cn.ConnectionString = MyConnection();
                cn.Open();
            }
            return cn;
        }

        public void CloseConnection()
        {
            if (cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        public MySqlDataReader ExecuteQuery(string query, MySqlParameter[] parameters = null)
        {
            try
            {
                OpenConnection();
                MySqlCommand cm = new MySqlCommand(query, cn);
                if (parameters != null)
                {
                    cm.Parameters.AddRange(parameters);
                }
                MySqlDataReader dr = cm.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ExecuteNonQuery(string query, MySqlParameter[] parameters = null)
        {
            try
            {
                OpenConnection();
                MySqlCommand cm = new MySqlCommand(query, cn);
                if (parameters != null)
                {
                    cm.Parameters.AddRange(parameters);
                }
                int result = cm.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public object ExecuteScalar(string query, MySqlParameter[] parameters = null)
        {
            try
            {
                OpenConnection();
                MySqlCommand cm = new MySqlCommand(query, cn);
                if (parameters != null)
                {
                    cm.Parameters.AddRange(parameters);
                }
                object result = cm.ExecuteScalar();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public double GetVal()
        {
            double vat = 0;
            try
            {
                cn.ConnectionString = MyConnection();
                cn.Open();
                MySqlCommand cm = new MySqlCommand("SELECT * FROM tbl_store LIMIT 1", cn);
                MySqlDataReader dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    // Note: Adjust field name if VAT is stored in tbl_store
                    // If VAT table exists, use: "SELECT * FROM tbl_vat LIMIT 1"
                    // vat = Double.Parse(dr["vat"].ToString());
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            return vat;
        }

        public double DailySales()
        {
            try
            {
                using (MySqlConnection tempCn = new MySqlConnection(MyConnection()))
                {
                    tempCn.Open();
                    using (MySqlCommand cm = new MySqlCommand("SELECT IFNULL(SUM(total), 0) AS total FROM tbl_cart WHERE DATE(sdate) = @sdate AND status = 'Sold'", tempCn))
                    {
                        cm.Parameters.Add("@sdate", MySqlDbType.Date).Value = DateTime.Today;
                        object result = cm.ExecuteScalar();
                        double dailysales = Convert.ToDouble(result);
                        return dailysales;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double ProductLine()
        {
            try
            {
                MySqlConnection tempCn = new MySqlConnection();
                tempCn.ConnectionString = MyConnection();
                tempCn.Open();
                MySqlCommand cm = new MySqlCommand("SELECT COUNT(*) FROM tbl_products", tempCn);
                int productline = int.Parse(cm.ExecuteScalar().ToString());
                tempCn.Close();
                return productline;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double StockOnHand()
        {
            try
            {
                MySqlConnection tempCn = new MySqlConnection();
                tempCn.ConnectionString = MyConnection();
                tempCn.Open();
                MySqlCommand cm = new MySqlCommand("SELECT IFNULL(SUM(qty), 0) AS qty FROM tbl_products", tempCn);
                int stockonhand = int.Parse(cm.ExecuteScalar().ToString());
                tempCn.Close();
                return stockonhand;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double Critical()
        {
            try
            {
                MySqlConnection tempCn = new MySqlConnection();
                tempCn.ConnectionString = MyConnection();
                tempCn.Open();
                MySqlCommand cm = new MySqlCommand("SELECT COUNT(*) FROM viewcriticalitems", tempCn);
                int critical = int.Parse(cm.ExecuteScalar().ToString());
                tempCn.Close();
                return critical;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetPassword(string user)
        {
            string password = "";
            try
            {
                cn.ConnectionString = MyConnection();
                cn.Open();
                MySqlCommand cm = new MySqlCommand("SELECT * FROM tbl_users WHERE username = @username", cn);
                cm.Parameters.AddWithValue("@username", user);
                MySqlDataReader dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    password = dr["password"].ToString();
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            return password;
        }
    }
}
