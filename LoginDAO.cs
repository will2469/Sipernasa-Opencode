using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Sipernasa
{
    public class LoginDAO
    {
        ConnectionUtils c;
        SQLiteCommand cmd;
        bool tableCreated;
        public bool createTableLogin(string nama_pengguna, string kata_sandi)
        {
            c = new ConnectionUtils();
            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                try
                {
                    try
                    {
                        cc.Open();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        using (cmd = new SQLiteCommand())
                        {
                            string query = @"INSERT INTO pengguna(nama_pengguna, kata_sandi) VALUES(@nama_pengguna, @kata_sandi)";
                            cmd.Parameters.Add(new SQLiteParameter("@nama_pengguna", nama_pengguna));
                            cmd.Parameters.Add(new SQLiteParameter("@kata_sandi", kata_sandi));
                            cmd.CommandText = query;
                            cmd.Connection = cc;
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                tableCreated = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    tableCreated = false;
                }
            }
            return tableCreated;
        }
        bool loginSuccess;
        public bool Login(string nama_pengguna, string kata_sandi)
        {
            c = new ConnectionUtils();

            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                try
                {
                    try
                    {
                        cc.Open();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        SQLiteCommand cmd = new SQLiteCommand();
                        string query = "SELECT nama_pengguna, kata_sandi FROM pengguna WHERE nama_pengguna = '" + nama_pengguna + "' AND kata_sandi = '" + kata_sandi + "';";
                        cmd.CommandText = query;
                        cmd.Connection = cc;
                        try
                        {
                            cmd.ExecuteNonQuery();
                            SQLiteDataReader r = cmd.ExecuteReader();
                            if (r.Read())
                            {
                                if ((r["nama_pengguna"].ToString().Equals(nama_pengguna.ToString(), StringComparison.InvariantCulture)) && (r["kata_sandi"].ToString().Equals(kata_sandi.ToString(), StringComparison.InvariantCulture)))
                                {
                                    loginSuccess = true;
                                }
                                r.Close();
                                r.Dispose();
                                cmd.Dispose();
                                cc.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    loginSuccess = false;
                }
                return loginSuccess;
            }
        }
    }
}
