using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Sipernasa
{
    public class PerangkatDAO
    {
        ConnectionUtils cu = new ConnectionUtils();
        SQLiteCommand cmd;
        bool dBTablePerangkat;
        public bool CreateTablePerangkat()
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"CREATE TABLE [perangkat](
                    [nama_perangkat] TEXT(50) PRIMARY KEY NOT NULL,
                    [nip] INTEGER DEFAULT NULL,
                    [jabatan] TEXT(100) NOT NULL)";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    cmd.ExecuteNonQuery();
                    dBTablePerangkat = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    dBTablePerangkat = false;
                }
                return dBTablePerangkat;
            }
        }
        bool available;
        public bool CheckDBPerangkat()
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='perangkat';";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    SQLiteDataReader r = cmd.ExecuteReader();
                    int w = 0;
                    while (r.Read())
                    {
                        w = r.GetInt32(0);
                    }
                    if (w == 1)
                    {
                        available = true;
                    }
                    else
                    {
                        available = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    available = false;
                }
                return available;
            }
        }
        bool perangkatAvailable;
        public bool checkPerangkat()
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"select count(nama_perangkat) from perangkat;";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    SQLiteDataReader r = cmd.ExecuteReader();
                    int w = 0;
                    while (r.Read())
                    {
                        w = r.GetInt32(0);
                    }
                    if (w > 0)
                    {
                        perangkatAvailable = true;
                    }
                    else
                    {
                        perangkatAvailable = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    perangkatAvailable = false;
                }
                return perangkatAvailable;
            }
        }
        bool jabatanAvailable;
        public bool CheckJabatan(string jabatan)
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"SELECT * FROM perangkat WHERE jabatan = '" + jabatan + "'";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        jabatanAvailable = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    jabatanAvailable = false;
                }
                return jabatanAvailable;
            }
        }
        public bool insertData(string namaPerangkat, int Nip, string Jabatan)
        {
            bool success;
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"INSERT INTO perangkat(nama_perangkat, nip, jabatan) VALUES(@namaPerangkat, @Nip, @Jabatan)";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    cmd.Parameters.Add(new SQLiteParameter("@namaPerangkat", namaPerangkat));
                    cmd.Parameters.Add(new SQLiteParameter("@Nip", Nip));
                    cmd.Parameters.Add(new SQLiteParameter("@Jabatan", Jabatan));
                    cmd.ExecuteNonQuery();
                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    success = false;
                }
            }
            return success;
        }
        bool resetSuccess;
        public bool resetAutoIncrement()
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"DELETE FROM sqlite_sequence WHERE name = 'perangkat'";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    resetSuccess = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    resetSuccess = false;
                }
                return resetSuccess;
            }
        }
        bool removed;
        public bool removeData(string Jabatan)
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"DELETE FROM perangkat WHERE jabatan = '" + Jabatan + "'";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    cmd.ExecuteNonQuery();
                    removed = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    removed = false;
                }
                return removed;
            }
        }
        bool updated;
        public bool updateData(string namaPerangkat, int Nip, string Jabatan)
        {
            string tempJabatan = Properties.Settings.Default.tempJabatan;
            Properties.Settings.Default.Save();
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"UPDATE perangkat SET nama_perangkat = @namaPerangkat, nip = @Nip, jabatan = @Jabatan WHERE jabatan = '" + tempJabatan + "'";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    cmd.Parameters.Add(new SQLiteParameter("@namaPerangkat", namaPerangkat));
                    cmd.Parameters.Add(new SQLiteParameter("@Nip", Nip));
                    cmd.Parameters.Add(new SQLiteParameter("@Jabatan", Jabatan));
                    cmd.ExecuteNonQuery();
                    updated = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    updated = false;
                }
            }
            return updated;
        }
    }
}
