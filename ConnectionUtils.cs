using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;


namespace Sipernasa
{
    public class ConnectionUtils
    {
        public string getConnection()
        {
            return @"Data Source=sipernasa.db; version=3; FailIfMissing=True; Foreign Keys=True; Pooling=false; synchronous=0; temp_store=2; count_changes=0; journal_mode=WAL";
        }
        public bool IsDBFileAvailable(string db)
        {
            bool isExists;
            if (!File.Exists(db))
            {
                //File Telah Tersedia
                isExists = false;
            }
            else
            {
                //File Belum Tersedia
                isExists = true;
            }
            return isExists;
        }

        public bool CreateDBFile(string db)
        {
            File.Create(db);
            return true;
        }
        SQLiteCommand cmd = new SQLiteCommand();
        bool loginAvailable;
        public bool checkLoginTable()
        {
            using (SQLiteConnection cc = new SQLiteConnection(getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"SELECT * FROM pengguna WHERE nama_pengguna = 'admin'";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    SQLiteDataReader r = cmd.ExecuteReader();
                    int w = 0;
                    while (r.Read())
                    {
                        w = r.GetInt32(0);
                    }
                    try
                    {
                        if (w == 1)
                        {
                            loginAvailable = true;
                        }
                        else
                        {
                            loginAvailable = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    loginAvailable = false;
                }
                return loginAvailable;
            }
        }
        bool dBpenggunaCreated;
        public bool createLoginInfo()
        {
            using (SQLiteConnection cc = new SQLiteConnection(getConnection()))
            {
                try
                {
                    cc.Open();
                    string query = @"CREATE TABLE [pengguna](
                    [nama_pengguna] TEXT(50) PRIMARY KEY NOT NULL,
                    [kata_sandi] TEXT(100) NOT NULL);";
                    SQLiteCommand cmd = new SQLiteCommand(query, cc);
                    cmd.ExecuteNonQuery();
                    dBpenggunaCreated = true;
                    cc.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    dBpenggunaCreated = false;
                }
                return dBpenggunaCreated;
            }
        }
    }
}
