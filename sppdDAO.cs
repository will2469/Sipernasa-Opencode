using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace Sipernasa
{
    public class sppdDAO
    {
        ConnectionUtils cu = new ConnectionUtils();
        SQLiteCommand cmd;
        bool dBTableSPPD;
        public bool CreateTableSPPD()
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    string query = @"CREATE TABLE
                    [sppd](
                    [no_agenda] TEXT(50) PRIMARY KEY NOT NULL,
                    [dasar_surat] TEXT(100) DEFAULT NULL,
                    [tanggal_surat] TEXT(100) DEFAULT NULL,
                    [nama_perangkat] TEXT(150) DEFAULT NULL,
                    [nip] INTEGER DEFAULT NULL,
                    [jabatan] TEXT(100) DEFAULT NULL,
                    [maksud_sppd] TEXT(150) DEFAULT NULL,
                    [alat_transportasi] TEXT(150) DEFAULT NULL,
                    [tempat_berangkat] TEXT(100) DEFAULT NULL,
                    [tempat_tujuan] TEXT(150) DEFAULT NULL,
                    [tanggal_berangkat] TEXT(150) DEFAULT NULL,
                    [tanggal_kembali] TEXT(150) DEFAULT NULL,
                    [lama_keberangkatan] TEXT(50) DEFAULT NULL,
                    [pengikut1] TEXT(100) DEFAULT NULL,
                    [jabatan_pengikut1] TEXT(150) DEFAULT NULL,
                    [pengikut2] TEXT(100) DEFAULT NULL,
                    [jabatan_pengikut2] TEXT(150) DEFAULT NULL,
                    [sumber_anggaran] TEXT(150) DEFAULT NULL,
                    [keterangan] TEXT(500) DEFAULT NULL)";
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.CommandText = query;
                        cmd.Connection = cc;
                        cmd.ExecuteNonQuery();
                        dBTableSPPD = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    dBTableSPPD = false;
                }
                return dBTableSPPD;
            }
        }
        bool available;
        public bool CheckDBsppd()
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='sppd';";
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
        public int getLastInput()
        {
            int count = 0;
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                cc.Open();
                cmd = new SQLiteCommand();
                string query1 = @"SELECT count(dasar_surat) FROM sppd";
                cmd.CommandText = query1;
                cmd.Connection = cc;
                count = Convert.ToInt32(cmd.ExecuteScalar());
                string query2 = @"SELECT dasar_surat FROM sppd";
                cmd.CommandText = query2;
                cmd.Connection = cc;
                SQLiteDataReader r = cmd.ExecuteReader();
            }
            return count;
        }
        bool agendaAvailable;
        public bool CheckAgenda(string no_agenda)
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"SELECT * FROM sppd WHERE no_agenda = '" + no_agenda + "'";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        agendaAvailable = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    agendaAvailable = false;
                }
                return agendaAvailable;
            }
        }
        public void insertDataSPPD(sppdModel sm, SQLiteTransaction trn)
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    try
                    {
                        cc.Open();
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        using (cmd = new SQLiteCommand())
                        {
                            string query = @"INSERT INTO [sppd] VALUES( @no_agenda, @dasar_surat, @tanggal_surat, @nama_perangkat, @nip, @jabatan, @maksud_sppd, @alat_transportasi, @tempat_berangkat, @tempat_tujuan, @tanggal_berangkat, @tanggal_kembali, @lama_keberangkatan, @pengikut1, @jabatan_pengikut1, @pengikut2, @jabatan_pengikut2, @sumber_anggaran, @keterangan)";
                            cmd.Parameters.AddWithValue("@no_agenda", sm.no_agenda);
                            cmd.Parameters.AddWithValue("@dasar_surat", sm.dasar_surat);
                            cmd.Parameters.AddWithValue("@tanggal_surat", sm.tanggal_surat);
                            cmd.Parameters.AddWithValue("@nama_perangkat", sm.nama_perangkat);
                            cmd.Parameters.AddWithValue("@nip", sm.nip);
                            cmd.Parameters.AddWithValue("@jabatan", sm.jabatan);
                            cmd.Parameters.AddWithValue("@maksud_sppd", sm.maksud_sppd);
                            cmd.Parameters.AddWithValue("@alat_transportasi", sm.alat_transportasi);
                            cmd.Parameters.AddWithValue("@tempat_berangkat", sm.tempat_berangkat);
                            cmd.Parameters.AddWithValue("@tempat_tujuan", sm.tempat_tujuan);
                            cmd.Parameters.AddWithValue("@tanggal_berangkat", sm.tanggal_berangkat);
                            cmd.Parameters.AddWithValue("@tanggal_kembali", sm.tanggal_kembali);
                            cmd.Parameters.AddWithValue("@lama_keberangkatan", sm.lama_keberangkatan);
                            cmd.Parameters.AddWithValue("@pengikut1", sm.pengikut1);
                            cmd.Parameters.AddWithValue("@jabatan_pengikut1", sm.jabatan_pengikut1);
                            cmd.Parameters.AddWithValue("@pengikut2", sm.pengikut2);
                            cmd.Parameters.AddWithValue("@jabatan_pengikut2", sm.jabatan_pengikut2);
                            cmd.Parameters.AddWithValue("@sumber_anggaran", sm.sumber_anggaran);
                            cmd.Parameters.AddWithValue("@keterangan", sm.keterangan);
                            cmd.CommandText = query;
                            cmd.Connection = cc;
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Data SPPD Baru Berhasil Disimpan Bosku!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                cc.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
                    string query = @"DELETE FROM sqlite_sequence WHERE name = 'sppd'";
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
        public bool removeData(string no_agenda)
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"DELETE FROM sppd WHERE no_agenda = '" + no_agenda + "'";
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
        public bool updateDataSPPD(string no_agenda, string dasar_surat, string tanggal_surat, string nama_perangkat, int nip, string jabatan, string maksud_sppd, string alat_transportasi, string tempat_berangkat, string tempat_tujuan, string tanggal_berangkat, string tanggal_kembali, string lama_keberangkatan, string pengikut1, string jabatan_pengikut1, string pengikut2, string jabatan_pengikut2, string sumber_anggaran, string keterangan)
        {
            using (SQLiteConnection cc = new SQLiteConnection(cu.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"UPDATE sppd SET no_agenda = @no_agenda, dasar_surat = @dasar_surat, tanggal_surat = @tanggal_surat, nama_perangkat = @nama_perangkat, nip = @nip, jabatan = @jabatan, maksud_sppd = @maksud_sppd, alat_transportasi = @alat_transportasi, tempat_berangkat = @tempat_berangkat, tempat_tujuan = @tempat_tujuan, tanggal_berangkat = @tanggal_berangkat, tanggal_kembali = @tanggal_kembali, lama_keberangkatan = @lama_keberangkatan, pengikut1 = @pengikut1, jabatan_pengikut1 = @jabatan_pengikut1, pengikut2 = @pengikut2, jabatan_pengikut2 = @jabatan_pengikut2, sumber_anggaran = @sumber_anggaran, keterangan = @keterangan WHERE no_agenda = '" + no_agenda + "'";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    cmd.Parameters.Add(new SQLiteParameter("@no_agenda", no_agenda));
                    cmd.Parameters.Add(new SQLiteParameter("@dasar_surat", dasar_surat));
                    cmd.Parameters.Add(new SQLiteParameter("@tanggal_surat", tanggal_surat));
                    cmd.Parameters.Add(new SQLiteParameter("@nama_perangkat", nama_perangkat));
                    cmd.Parameters.Add(new SQLiteParameter("@nip", nip));
                    cmd.Parameters.Add(new SQLiteParameter("@jabatan", jabatan));
                    cmd.Parameters.Add(new SQLiteParameter("@maksud_sppd", maksud_sppd));
                    cmd.Parameters.Add(new SQLiteParameter("@alat_transportasi", alat_transportasi));
                    cmd.Parameters.Add(new SQLiteParameter("@tempat_berangkat", tempat_berangkat));
                    cmd.Parameters.Add(new SQLiteParameter("@tempat_tujuan", tempat_tujuan));
                    cmd.Parameters.Add(new SQLiteParameter("@tanggal_berangkat", tanggal_berangkat));
                    cmd.Parameters.Add(new SQLiteParameter("@tanggal_kembali", tanggal_kembali));
                    cmd.Parameters.Add(new SQLiteParameter("@lama_keberangkatan", lama_keberangkatan));
                    cmd.Parameters.Add(new SQLiteParameter("@pengikut1", pengikut1));
                    cmd.Parameters.Add(new SQLiteParameter("@jabatan_pengikut1", jabatan_pengikut1));
                    cmd.Parameters.Add(new SQLiteParameter("@pengikut2", pengikut2));
                    cmd.Parameters.Add(new SQLiteParameter("@jabatan_pengikut2", jabatan_pengikut2));
                    cmd.Parameters.Add(new SQLiteParameter("@sumber_anggaran", sumber_anggaran));
                    cmd.Parameters.Add(new SQLiteParameter("@keterangan", keterangan));
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
