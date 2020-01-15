using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Sipernasa
{
    /// <summary>
    /// Interaction logic for SPPD.xaml
    /// </summary>
    public partial class SPPD : UserControl
    {
        public SPPD()
        {
            InitializeComponent();
            generateAutoNumber();
            getCurrentRomanNumberMont();
            getCurrentYear();
            loadComboboxItem();
        }
        private void dataGridSppd_Loaded(object sender, RoutedEventArgs e)
        {
            loadTableSPPD();
        }
        private void loadTableSPPD()
        {
            c = new ConnectionUtils();

            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = cc.CreateCommand();
                    cmd.CommandText = @"SELECT no_agenda, dasar_surat, tanggal_surat, nama_perangkat, nip, jabatan, maksud_sppd, alat_transportasi, tempat_berangkat, tempat_tujuan, tanggal_berangkat, tanggal_kembali, lama_keberangkatan, pengikut1, jabatan_pengikut1, pengikut2, jabatan_pengikut2, sumber_anggaran, keterangan FROM sppd ORDER BY no_agenda";
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, cc))
                    {
                        DataTable ddta = new DataTable();
                        da.Fill(ddta);
                        dataGridSppd.ItemsSource = ddta.AsDataView();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Bosku!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        sppdDAO sd;
        ConnectionUtils c;
        SQLiteCommand cmd;
        private void generateAutoNumber()
        {
            sd = new sppdDAO();
            int autonumber = (sd.getLastInput()) + 1;
            txt_autoNumber.Text = autonumber.ToString("D3");
        }
        private void getCurrentRomanNumberMont()
        {
            int cMonth = DateTime.Now.Month;
            string rMonth = RomanNumeralConverter.ToRoman(cMonth);
            txt_currentMonth.Text = rMonth;
        }
        private void getCurrentYear()
        {
            int cYear = DateTime.Now.Year;
            txt_currentYear.Text = cYear.ToString("D4");
        }
        private void loadComboboxItem()
        {
            c = new ConnectionUtils();
            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"select nama_perangkat from perangkat";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        cmb_namaPerangkat.Items.Add(r["nama_perangkat"].ToString());
                        cmb_pengikut1.Items.Add(r["nama_perangkat"].ToString());
                        cmb_pengikut2.Items.Add(r["nama_perangkat"].ToString());
                    }
                    cc.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private void cmb_perangkat_DropDownClosed(object sender, EventArgs e)
        {
            c = new ConnectionUtils();
            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                if (cmb_namaPerangkat.Text != null)
                {
                    try
                    {
                        cc.Open();
                        cmd = new SQLiteCommand();
                        string query = @"select * from perangkat where nama_perangkat = '" + cmb_namaPerangkat.Text + "'";
                        cmd.CommandText = query;
                        cmd.Connection = cc;
                        SQLiteDataReader r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            string nip = r.GetInt32(1).ToString();
                            string jabatan = r.GetString(2);
                            txt_nip.Text = nip;
                            txt_jabatan.Text = jabatan;
                        }
                        cc.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private void dp_tk_CalendarClosed(object sender, RoutedEventArgs e)
        {
            DateTime tb = dp_tb.SelectedDate.Value.Date;
            DateTime tk = dp_tk.SelectedDate.Value.Date;
            TimeSpan diff = tk.Subtract(tb);
            int df = Math.Abs(diff.Days);
            int result = df + 1;
            string res = result.ToString();
            var lk = new StringBuilder();
            lk.Append(res);
            lk.Append(" Hari");
            if (dp_tb.SelectedDate.HasValue || dp_tk.SelectedDate.HasValue)
            {
                txt_lk.Text = lk.ToString();
                return;
            }
            else
            {
                MessageBox.Show("Harap Masukkan Tanggal Keberangkatan Dulu Bosku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dp_tk.Text = "";
            }
        }

        private void cmb_pengikut1_DropDownClosed(object sender, EventArgs e)
        {
            c = new ConnectionUtils();
            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                string aval = cmb_namaPerangkat.Text;
                if (cmb_namaPerangkat.Text != null)
                {
                    if (aval != string.Empty)
                    {
                        if (cmb_pengikut1.Text.ToString() != aval)
                        {
                            try
                            {
                                cc.Open();
                                cmd = new SQLiteCommand();
                                string query = @"select * from perangkat where nama_perangkat = '" + cmb_pengikut1.Text + "'";
                                cmd.CommandText = query;
                                cmd.Connection = cc;
                                SQLiteDataReader r = cmd.ExecuteReader();
                                while (r.Read())
                                {
                                    string jabatan = r.GetString(2);
                                    txt_jabatan_pengikut1.Text = jabatan;
                                }
                                cc.Close();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Maaf Nama Pengikut tidak boleh sama dengan nama perangkat yang diutus bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            cmb_pengikut1.SelectedIndex = -1;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Harap Masukkan Nama Perangkat yang akan melakukan perjalanan dulu bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        cmb_pengikut1.SelectedIndex = -1;
                    }
                }
            }
        }

        private void cmb_pengikut2_DropDownClosed(object sender, EventArgs e)
        {
            c = new ConnectionUtils();
            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                string aval = cmb_namaPerangkat.Text;
                string bval = cmb_pengikut1.Text;
                if (cmb_namaPerangkat.Text != null)
                {
                    if (aval != string.Empty)
                    {
                        if (cmb_pengikut2.Text != aval)
                        {
                            if (bval != string.Empty
                                && cmb_pengikut2.Text != bval)
                            {
                                try
                                {
                                    cc.Open();
                                    cmd = new SQLiteCommand();
                                    string query = @"select * from perangkat where nama_perangkat = '" + cmb_pengikut2.Text + "'";
                                    cmd.CommandText = query;
                                    cmd.Connection = cc;
                                    SQLiteDataReader r = cmd.ExecuteReader();
                                    while (r.Read())
                                    {
                                        string jabatan = r.GetString(2);
                                        txt_jabatan_pengikut2.Text = jabatan;
                                    }
                                    cc.Close();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Maaf Nama Pengikut pertama tidak boleh kosong atau sama dengan Nama Pengikut kedua bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                cmb_pengikut2.SelectedIndex = -1;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Maaf Nama Pengikut tidak boleh sama dengan nama perangkat yang diutus bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            cmb_pengikut2.SelectedIndex = -1;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Harap Masukkan Nama Perangkat yang akan melakukan perjalanan dulu bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        cmb_pengikut2.SelectedIndex = -1;
                    }
                }
            }
        }
        private void cmb_namaPerangkat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_pengikut1.HasItems)
            {
                cmb_pengikut1.SelectedIndex = -1;
            }
            if (cmb_pengikut2.HasItems)
            {
                cmb_pengikut2.SelectedIndex = -1;
            }
        }
        bool edit = false;
        private void btn_new_Click(object sender, RoutedEventArgs e)
        {
            edit = false;
            c = new ConnectionUtils();
            sd = new sppdDAO();
            var na = new StringBuilder();
            try
            {
                na.Append(agenda.Text);
                na.Append(txt_separator1.Text);
                na.Append(txt_autoNumber.Text);
                na.Append(txt_separator2.Text);
                na.Append(txt_currentMonth.Text);
                na.Append(txt_separator3.Text);
                na.Append(txt_currentYear.Text);
                string no_agenda = na.ToString();
                try
                {
                    if (txt_autoNumber.Text != string.Empty
                        && txt_ds.Text != string.Empty
                        && dp_ts.Text.ToString() != string.Empty
                        && cmb_namaPerangkat.Text.ToString() != string.Empty
                        && txt_maksud.Text != string.Empty
                        && cmb_alat.Text.ToString() != string.Empty
                        && txt_tempatBerangkat.Text != string.Empty
                        && txt_tempatTujuan.Text != string.Empty
                        && dp_tb.Text != string.Empty
                        && dp_tk.Text != string.Empty
                        && txt_sumberAnggaran.Text != string.Empty
                        )
                    {
                        sppdModel sm = new sppdModel();
                        try
                        {
                            sm.no_agenda = no_agenda.ToString();
                            sm.dasar_surat = txt_ds.Text.ToString();
                            sm.tanggal_surat = dp_ts.Text.ToString();
                            sm.nama_perangkat = cmb_namaPerangkat.Text.ToString();
                            sm.nip = IntegerExtensions.ParseInt(txt_nip.Text);
                            sm.jabatan = txt_jabatan.Text.ToString();
                            sm.maksud_sppd = txt_maksud.Text.ToString();
                            sm.alat_transportasi = cmb_alat.Text.ToString();
                            sm.tempat_berangkat = txt_tempatBerangkat.Text.ToString();
                            sm.tempat_tujuan = txt_tempatTujuan.Text.ToString();
                            sm.tanggal_berangkat = dp_tb.Text.ToString();
                            sm.tanggal_kembali = dp_tk.Text.ToString();
                            sm.lama_keberangkatan = txt_lk.Text.ToString();
                            sm.pengikut1 = cmb_pengikut1.Text.ToString();
                            sm.jabatan_pengikut1 = txt_jabatan_pengikut1.Text.ToString();
                            sm.pengikut2 = cmb_pengikut2.Text.ToString();
                            sm.jabatan_pengikut2 = txt_jabatan_pengikut2.Text.ToString();
                            sm.sumber_anggaran = txt_sumberAnggaran.Text.ToString();
                            sm.keterangan = txt_ket.Text.ToString();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            try
                            {
                                using (SQLiteConnection con = new SQLiteConnection(c.getConnection()))
                                {
                                    try
                                    {
                                        con.Open();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                    using (SQLiteTransaction trn = con.BeginTransaction(IsolationLevel.ReadCommitted))
                                    {
                                        try
                                        {
                                            sd.insertDataSPPD(sm, trn);
                                        }
                                        catch (Exception ex)
                                        {
                                            trn.Rollback();
                                            MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            finally
                            {
                                emptyFields();
                                loadTableSPPD();
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("Semua yang terdapat tanda (*) wajib diisi Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void emptyFields()
        {
            generateAutoNumber();
            txt_ds.Text = "";
            dp_ts.Text = "";
            cmb_namaPerangkat.SelectedIndex = -1;
            txt_nip.Text = "";
            txt_jabatan.Text = "";
            txt_maksud.Text = "";
            cmb_alat.SelectedIndex = -1;
            txt_tempatBerangkat.Text = "";
            txt_tempatTujuan.Text = "";
            dp_tb.Text = "";
            dp_tk.Text = "";
            txt_lk.Text = "";
            cmb_pengikut1.SelectedIndex = -1;
            txt_jabatan_pengikut1.Text = "";
            cmb_pengikut2.SelectedIndex = -1;
            txt_jabatan_pengikut2.Text = "";
            txt_ket.Text = "";
        }
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            c = new ConnectionUtils();
            sd = new sppdDAO();
            var na = new StringBuilder();
            na.Append(agenda.Text);
            na.Append(txt_separator1.Text);
            na.Append(txt_autoNumber.Text);
            na.Append(txt_separator2.Text);
            na.Append(txt_currentMonth.Text);
            na.Append(txt_separator3.Text);
            na.Append(txt_currentYear.Text);
            string no_agenda = na.ToString();
            if (!edit)
            {
                edit = true;
                btn_hapus.IsEnabled = true;
                iconUbah.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSaveOutline;
            }
            else
            {
                edit = false;
                btn_hapus.IsEnabled = false;
                iconUbah.Kind = MaterialDesignThemes.Wpf.PackIconKind.BriefcaseEditOutline;
                if (sd.updateDataSPPD(no_agenda.ToString(), txt_ds.Text.ToString(), dp_ts.Text.ToString(), cmb_namaPerangkat.Text.ToString(), IntegerExtensions.ParseInt(txt_nip.Text), txt_jabatan.Text.ToString(), txt_maksud.Text.ToString(), cmb_alat.Text.ToString(), txt_tempatBerangkat.Text.ToString(), txt_tempatTujuan.Text.ToString(), dp_tb.Text.ToString(), dp_tk.Text.ToString(), txt_lk.Text.ToString(), cmb_pengikut1.Text.ToString(), txt_jabatan_pengikut1.Text.ToString(), cmb_pengikut2.Text.ToString(), txt_jabatan_pengikut2.Text.ToString(), txt_sumberAnggaran.Text.ToString(), txt_ket.Text.ToString()))
                {
                    MessageBox.Show("Berhasil Mengubah Data Bosku", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                emptyFields();
                loadTableSPPD();
                sd.resetAutoIncrement();
            }
        }

        private void btn_hapus_Click(object sender, RoutedEventArgs e)
        {
            c = new ConnectionUtils();
            sd = new sppdDAO();
            var na = new StringBuilder();
            na.Append(agenda.Text);
            na.Append(txt_separator1.Text);
            na.Append(txt_autoNumber.Text);
            na.Append(txt_separator2.Text);
            na.Append(txt_currentMonth.Text);
            na.Append(txt_separator3.Text);
            na.Append(txt_currentYear.Text);
            string no_agenda = na.ToString();
            MessageBoxResult re = MessageBox.Show("Apakah anda yakin ingin menghapus Data ini bosku?", "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (re == MessageBoxResult.Yes)
            {
                if (!sd.CheckAgenda(no_agenda))
                {
                    if (sd.removeData(no_agenda))
                    {
                        MessageBox.Show("Berhasil Menghapus Data Bosku", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                        emptyFields();
                        loadTableSPPD();
                        sd.resetAutoIncrement();
                    }
                }
                else
                {
                    MessageBox.Show("Data Tidak Ditemukan Bosku", "Gagal Menghapus", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            else
            {
                //Exit Selection
            }
        }

        private void dataGridSppd_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (edit)
                {
                    DataGrid dg = (DataGrid)sender;
                    DataRowView drw = dg.SelectedItem as DataRowView;
                    if (drw != null)
                    {
                        string p = drw["no_agenda"].ToString();
                        int l = 4;
                        int m = 3;
                        string o = p.Substring(l, m);
                        txt_autoNumber.Text = o;
                        txt_ds.Text = drw["dasar_surat"].ToString();
                        dp_ts.Text = drw["tanggal_surat"].ToString();
                        cmb_namaPerangkat.Text = drw["nama_perangkat"].ToString();
                        txt_nip.Text = drw["nip"].ToString();
                        txt_jabatan.Text = drw["jabatan"].ToString();
                        txt_maksud.Text = drw["maksud_sppd"].ToString();
                        cmb_alat.Text = drw["alat_transportasi"].ToString();
                        txt_tempatBerangkat.Text = drw["tempat_berangkat"].ToString();
                        txt_tempatTujuan.Text = drw["tempat_tujuan"].ToString();
                        dp_tb.Text = drw["tanggal_berangkat"].ToString();
                        dp_tk.Text = drw["tanggal_kembali"].ToString();
                        txt_lk.Text = drw["lama_keberangkatan"].ToString();
                        cmb_pengikut1.Text = drw["pengikut1"].ToString();
                        txt_jabatan_pengikut1.Text = drw["jabatan_pengikut1"].ToString();
                        cmb_pengikut2.Text = drw["pengikut2"].ToString();
                        txt_jabatan_pengikut2.Text = drw["jabatan_pengikut2"].ToString();
                        txt_sumberAnggaran.Text = drw["sumber_anggaran"].ToString();
                        txt_ket.Text = drw["keterangan"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Data tidak ditemukan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Harap Klik Tombol Ubah Dulu Bosku", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Information);
                    dataGridSppd.UnselectAll();
                    loadTableSPPD();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void dp_ts_Closed(object sender, RoutedEventArgs e)
        {
            if (dp_ts.Text != string.Empty)
            {
                dp_tb.BlackoutDates.Add(new CalendarDateRange(new DateTime(1990, 1, 1), DateTime.Parse(dp_ts.Text).AddDays(-1)));
                dp_tb.IsEnabled = true;
            }
        }

        private void dp_tb_Closed(object sender, RoutedEventArgs e)
        {
            if (dp_tb.Text != string.Empty)
            {
                dp_tk.BlackoutDates.Add(new CalendarDateRange(new DateTime(1990, 1, 1), DateTime.Parse(dp_tb.Text).AddDays(-1)));
                dp_tk.IsEnabled = true;
            }
            else
            {
                Console.WriteLine("error");
            }
        }

        private void dp_ts_Open(object sender, RoutedEventArgs e)
        {
            dp_ts.DisplayDateEnd = DateTime.Now;
        }
    }
}
