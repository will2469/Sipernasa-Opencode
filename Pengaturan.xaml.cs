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
using SipernasaActivationControl;

namespace Sipernasa
{
    /// <summary>
    /// Interaction logic for Pengaturan.xaml
    /// </summary>
    public partial class Pengaturan : UserControl
    {
        public Pengaturan()
        {
            InitializeComponent();
            liControl.Content = new LicenseInfoControl();
            load_ttd_kades();
            load_ttd_sekdes();
            load_ttd_ppk();
            load_Profil();
            load_Pembiayaan();
            initImage();
            load_ttd_bend();
            loadComboboxItem();
        }
        SQLiteCommand cmd;
        ConnectionUtils cu = new ConnectionUtils();
        bool generate = false;
        bool edit = false;
        private void btn_generate_kades_Click(object sender, RoutedEventArgs e)
        {
            if (!generate)
            {
                generate = true;
                Properties.Settings.Default.ttd_nama_kades = "";
                Properties.Settings.Default.ttd_nip_kades = "";
                Properties.Settings.Default.ttd_jabatan_kades = "";
                Properties.Settings.Default.ttd_customHeader_kades = "";
                Properties.Settings.Default.Save();
                using (SQLiteConnection cc1 = new SQLiteConnection(cu.getConnection()))
                {
                    try
                    {
                        cc1.Open();
                        cmd = new SQLiteCommand();
                        string query1 = @"SELECT nama_perangkat FROM perangkat WHERE jabatan = 'Kepala Desa'";
                        cmd.CommandText = query1;
                        cmd.Connection = cc1;
                        SQLiteDataReader r1 = cmd.ExecuteReader();
                        while (r1.Read())
                        {
                            string nk = r1[0].ToString();
                            txt_kades.Text = nk;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    using (SQLiteConnection cc2 = new SQLiteConnection(cu.getConnection()))
                    {
                        try
                        {
                            cc2.Open();
                            cmd = new SQLiteCommand();
                            string query2 = @"SELECT nip FROM perangkat WHERE jabatan = 'Kepala Desa'";
                            cmd.CommandText = query2;
                            cmd.Connection = cc2;
                            SQLiteDataReader r2 = cmd.ExecuteReader();
                            while (r2.Read())
                            {
                                string nipk = r2[0].ToString();
                                if (nipk != "0")
                                {
                                    txt_nip_kades.Text = nipk;
                                }
                                else
                                {
                                    txt_nip_kades.Text = nipk.Replace('0', '-');
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    using (SQLiteConnection cc3 = new SQLiteConnection(cu.getConnection()))
                    {
                        try
                        {
                            cc3.Open();
                            cmd = new SQLiteCommand();
                            string query3 = @"SELECT jabatan FROM perangkat WHERE jabatan = 'Kepala Desa'";
                            cmd.CommandText = query3;
                            cmd.Connection = cc3;
                            SQLiteDataReader r3 = cmd.ExecuteReader();
                            while (r3.Read())
                            {
                                string jk = r3[0].ToString();
                                txt_jabatan_kades.Text = jk;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                txt_customHeader_kades.IsEnabled = true;
                generate_txt1.Content = "Simpan";
                iconUbah1.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSaveOutline;
            }
            else
            {
                generate = false;
                txt_customHeader_kades.IsEnabled = false;
                generate_txt1.Content = "Generate";
                iconUbah1.Kind = MaterialDesignThemes.Wpf.PackIconKind.Buffer;
                if (Properties.Settings.Default.ttd_nama_kades == string.Empty)
                {
                    Properties.Settings.Default.ttd_nama_kades = txt_kades.Text.ToString();
                    Properties.Settings.Default.ttd_nip_kades = txt_nip_kades.Text.ToString();
                    Properties.Settings.Default.ttd_jabatan_kades = txt_jabatan_kades.Text.ToString();
                    Properties.Settings.Default.ttd_customHeader_kades = txt_customHeader_kades.Text.ToString();
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Data Kepala Desa Sudah Berhasil Disimpan Bosku!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        void load_ttd_kades()
        {
            if (Properties.Settings.Default.ttd_nama_kades != string.Empty)
            {
                txt_kades.Text = Properties.Settings.Default.ttd_nama_kades;
                txt_nip_kades.Text = Properties.Settings.Default.ttd_nip_kades;
                txt_jabatan_kades.Text = Properties.Settings.Default.ttd_jabatan_kades;
                txt_customHeader_kades.Text = Properties.Settings.Default.ttd_customHeader_kades;
            }
        }

        private void btn_generate_sekdes_Click(object sender, RoutedEventArgs e)
        {
            if (!generate)
            {
                Properties.Settings.Default.ttd_nama_sekdes = "";
                Properties.Settings.Default.ttd_nip_sekdes = "";
                Properties.Settings.Default.ttd_jabatan_sekdes = "";
                Properties.Settings.Default.ttd_customHeader_sekdes = "";
                Properties.Settings.Default.Save();
                using (SQLiteConnection cc1 = new SQLiteConnection(cu.getConnection()))
                {
                    try
                    {
                        cc1.Open();
                        cmd = new SQLiteCommand();
                        string query1 = @"SELECT nama_perangkat FROM perangkat WHERE jabatan = 'Sekretaris Desa'";
                        cmd.CommandText = query1;
                        cmd.Connection = cc1;
                        SQLiteDataReader r1 = cmd.ExecuteReader();
                        while (r1.Read())
                        {
                            string nk = r1[0].ToString();
                            txt_sekdes.Text = nk;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    using (SQLiteConnection cc2 = new SQLiteConnection(cu.getConnection()))
                    {
                        try
                        {
                            cc2.Open();
                            cmd = new SQLiteCommand();
                            string query2 = @"SELECT nip FROM perangkat WHERE jabatan = 'Sekretaris Desa'";
                            cmd.CommandText = query2;
                            cmd.Connection = cc2;
                            SQLiteDataReader r2 = cmd.ExecuteReader();
                            while (r2.Read())
                            {
                                string nipk = r2[0].ToString();
                                if (nipk != "0")
                                {
                                    txt_nip_sekdes.Text = nipk;
                                }
                                else
                                {
                                    txt_nip_sekdes.Text = nipk.Replace('0', '-');
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    using (SQLiteConnection cc3 = new SQLiteConnection(cu.getConnection()))
                    {
                        try
                        {
                            cc3.Open();
                            cmd = new SQLiteCommand();
                            string query3 = @"SELECT jabatan FROM perangkat WHERE jabatan = 'Sekretaris Desa'";
                            cmd.CommandText = query3;
                            cmd.Connection = cc3;
                            SQLiteDataReader r3 = cmd.ExecuteReader();
                            while (r3.Read())
                            {
                                string jk = r3[0].ToString();
                                txt_jabatan_sekdes.Text = jk;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                generate = true;
                txt_customHeader_Sekdes.IsEnabled = true;
                generate_txt2.Content = "Simpan";
                iconUbah2.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSaveOutline;
            }
            else
            {
                generate = false;
                txt_customHeader_Sekdes.IsEnabled = false;
                generate_txt2.Content = "Generate";
                iconUbah2.Kind = MaterialDesignThemes.Wpf.PackIconKind.Buffer;
                if (Properties.Settings.Default.ttd_nama_sekdes == string.Empty)
                {
                    Properties.Settings.Default.ttd_nama_sekdes = txt_sekdes.Text.ToString();
                    Properties.Settings.Default.ttd_nip_sekdes = txt_nip_sekdes.Text.ToString();
                    Properties.Settings.Default.ttd_jabatan_sekdes = txt_jabatan_sekdes.Text.ToString();
                    Properties.Settings.Default.ttd_customHeader_sekdes = txt_customHeader_Sekdes.Text.ToString();
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Data Sekretaris Desa Sudah Berhasil Disimpan Bosku!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        void load_ttd_sekdes()
        {
            if (Properties.Settings.Default.ttd_nama_sekdes != string.Empty)
            {
                txt_sekdes.Text = Properties.Settings.Default.ttd_nama_sekdes;
                txt_nip_sekdes.Text = Properties.Settings.Default.ttd_nip_sekdes;
                txt_jabatan_sekdes.Text = Properties.Settings.Default.ttd_jabatan_sekdes;
                txt_customHeader_Sekdes.Text = Properties.Settings.Default.ttd_customHeader_sekdes;
            }
        }

        private void btn_generate_ppk_Click(object sender, RoutedEventArgs e)
        {
            if (!edit)
            {
                Properties.Settings.Default.ttd_nama_ppk = "";
                Properties.Settings.Default.ttd_jabatan_ppk = "";
                Properties.Settings.Default.ttd_nip_ppk = "";
                Properties.Settings.Default.ttd_customHeader_ppk = "";
                Properties.Settings.Default.Save();
                edit = true;
                cmb_namaPPK.IsEnabled = true;
                txt_customHeader_ppk.IsEnabled = true;
                generate_txt3.Content = "Simpan";
                iconUbah3.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSaveOutline;
            }
            else
            {
                edit = false;
                txt_customHeader_ppk.IsEnabled = false;
                generate_txt3.Content = "Edit";
                iconUbah3.Kind = MaterialDesignThemes.Wpf.PackIconKind.Buffer;
                if (Properties.Settings.Default.ttd_nama_ppk == string.Empty)
                {
                    Properties.Settings.Default.ttd_nama_ppk = cmb_namaPPK.Text.ToString();
                    Properties.Settings.Default.ttd_nip_ppk = txt_nip_ppk.Text.ToString();
                    Properties.Settings.Default.ttd_jabatan_ppk = txt_jabatan_ppk.Text.ToString();
                    Properties.Settings.Default.ttd_customHeader_ppk = txt_customHeader_ppk.Text.ToString();
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Data PPK Desa Sudah Berhasil Disimpan Bosku!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        void load_ttd_ppk()
        {
            if (Properties.Settings.Default.ttd_nama_ppk != string.Empty)
            {
                cmb_namaPPK.SelectedItem = Properties.Settings.Default.ttd_nama_ppk;
                txt_nip_ppk.Text = Properties.Settings.Default.ttd_nip_ppk;
                txt_jabatan_ppk.Text = Properties.Settings.Default.ttd_jabatan_ppk;
                txt_customHeader_ppk.Text = Properties.Settings.Default.ttd_customHeader_ppk;
            }
        }

        private void btn_open_FE_Click(object sender, RoutedEventArgs e)
        {
            loadImage();
        }
        void loadImage()
        {
            System.Windows.Forms.OpenFileDialog choofdlog = new System.Windows.Forms.OpenFileDialog();
            choofdlog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"; ;
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;

            if (choofdlog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string sFileName = choofdlog.FileName;
                txt_imgPath.Text = sFileName;
                logo.Source = new BitmapImage(new Uri(sFileName));
                btn_save_img.IsEnabled = true;
            }
        }
        private void btn_save_img_Click(object sender, RoutedEventArgs e)
        {
            if (txt_imgPath.Text != string.Empty)
            {
                Properties.Settings.Default.logo = "";
                Properties.Settings.Default.Save();
                Properties.Settings.Default.logo = txt_imgPath.Text;
                Properties.Settings.Default.Save();
            }
        }
        void initImage()
        {
            if (Properties.Settings.Default.logo != string.Empty)
            {
                try
                {
                    txt_imgPath.Text = Properties.Settings.Default.logo;
                    string source = Properties.Settings.Default.logo.ToString();
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(source);
                    bitmap.EndInit();
                    logo.Source = bitmap;
                }
                catch (Exception ex)
                {
                    Properties.Settings.Default.logo = "";
                    Properties.Settings.Default.Save();
                    MessageBox.Show("File Tidak ditemukan Bosku! Mohon Update File Uri dari Logo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine(ex.Message);
                }
            }
        }
        bool editProfil;
        private void btn_updateProfil_Click(object sender, RoutedEventArgs e)
        {
            if (!editProfil)
            {
                Properties.Settings.Default.Desa = "";
                Properties.Settings.Default.Kecamatan = "";
                Properties.Settings.Default.Kabupaten = "";
                Properties.Settings.Default.Alamat = "";
                Properties.Settings.Default.Save();
                editProfil = true;
                txt_namaDesa.IsEnabled = true;
                txt_namaKecamatan.IsEnabled = true;
                txt_namaKabupaten.IsEnabled = true;
                txt_Alamat.IsEnabled = true;
                updateProfil_txt.Content = "SAVE";
                iconUbahProfil.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSaveOutline;
            }
            else
            {
                editProfil = false;
                txt_namaDesa.IsEnabled = false;
                txt_namaKecamatan.IsEnabled = false;
                txt_namaKabupaten.IsEnabled = false;
                txt_Alamat.IsEnabled = false;
                updateProfil_txt.Content = "UPDATE";
                iconUbahProfil.Kind = MaterialDesignThemes.Wpf.PackIconKind.CircleEditOutline;
                try
                {
                    if (txt_namaDesa.Text != string.Empty
                        && txt_namaKecamatan.Text != string.Empty
                        && txt_namaKabupaten.Text != string.Empty
                        && txt_Alamat.Text != string.Empty)
                    {
                        Properties.Settings.Default.Desa = txt_namaDesa.Text.ToString();
                        Properties.Settings.Default.Kecamatan = txt_namaKecamatan.Text.ToString();
                        Properties.Settings.Default.Kabupaten = txt_namaKabupaten.Text.ToString();
                        Properties.Settings.Default.Alamat = txt_Alamat.Text.ToString();
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        MessageBox.Show("Semua Field Harus Di isi bosku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        txt_namaDesa.Text = "";
                        txt_namaKecamatan.Text = "";
                        txt_namaKabupaten.Text = "";
                        txt_Alamat.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        void load_Profil()
        {
            if (Properties.Settings.Default.Desa != string.Empty)
            {
                txt_namaDesa.Text = Properties.Settings.Default.Desa;
                txt_namaKecamatan.Text = Properties.Settings.Default.Kecamatan;
                txt_namaKabupaten.Text = Properties.Settings.Default.Kabupaten;
                txt_Alamat.Text = Properties.Settings.Default.Alamat;
            }
        }
        bool setBiayaDalamKota;
        private void btn_setdalamKota_Click(object sender, RoutedEventArgs e)
        {
            if (!setBiayaDalamKota)
            {
                Properties.Settings.Default.biayaDalamKota = "";
                Properties.Settings.Default.Save();
                setBiayaDalamKota = true;
                txt_local.IsEnabled = true;
                txt_localEdit.Content = "SAVE";
                iconUbah1local.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSaveOutline;
            }
            else
            {
                setBiayaDalamKota = false;
                txt_local.IsEnabled = false;
                txt_localEdit.Content = "UPDATE";
                iconUbah1local.Kind = MaterialDesignThemes.Wpf.PackIconKind.CircleEditOutline;
                try
                {
                    if (txt_local.Text != string.Empty)
                    {
                        Properties.Settings.Default.biayaDalamKota = txt_local.Text.ToString();
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        MessageBox.Show("Semua Field Harus Di isi bosku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        txt_local.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        bool setBiayaLuarKota;
        private void btn_setluarKota_Click(object sender, RoutedEventArgs e)
        {
            if (!setBiayaLuarKota)
            {
                Properties.Settings.Default.biayaLuarKota = "";
                Properties.Settings.Default.Save();
                setBiayaLuarKota = true;
                txt_luarKota.IsEnabled = true;
                txt_luar.Content = "SAVE";
                iconUbahluar.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSaveOutline;
            }
            else
            {
                setBiayaLuarKota = false;
                txt_luarKota.IsEnabled = false;
                txt_luar.Content = "UPDATE";
                iconUbahluar.Kind = MaterialDesignThemes.Wpf.PackIconKind.CircleEditOutline;
                try
                {
                    if (txt_luarKota.Text != string.Empty)
                    {
                        Properties.Settings.Default.biayaLuarKota = txt_luarKota.Text.ToString();
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        MessageBox.Show("Semua Field Harus Di isi bosku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        txt_luarKota.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        void load_Pembiayaan()
        {
            if (Properties.Settings.Default.Desa != string.Empty)
            {
                txt_local.Text = Properties.Settings.Default.biayaDalamKota;
                txt_luarKota.Text = Properties.Settings.Default.biayaLuarKota;
            }
        }
        private void cmb_namaPPK_DropDownClosed(object sender, EventArgs e)
        {
            ConnectionUtils c = new ConnectionUtils();
            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                if (cmb_namaPPK.Text.ToString() != null)
                {
                    try
                    {
                        cc.Open();
                        cmd = new SQLiteCommand();
                        string query = @"select * from perangkat where nama_perangkat = '" + cmb_namaPPK.Text.ToString() + "'";
                        cmd.CommandText = query;
                        cmd.Connection = cc;
                        SQLiteDataReader r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            string nip = r.GetInt32(1).ToString();
                            string jabatan = r.GetString(2);
                            txt_nip_ppk.Text = nip;
                            txt_jabatan_ppk.Text = jabatan;
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

        private void cmb_namaBend_DropDownClosed(object sender, EventArgs e)
        {
            ConnectionUtils c = new ConnectionUtils();
            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                if (cmb_namaBend.Text.ToString() != null)
                {
                    try
                    {
                        cc.Open();
                        cmd = new SQLiteCommand();
                        string query = @"select * from perangkat where nama_perangkat = '" + cmb_namaBend.Text.ToString() + "'";
                        cmd.CommandText = query;
                        cmd.Connection = cc;
                        SQLiteDataReader r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            string nip = r.GetInt32(1).ToString();
                            string jabatan = r.GetString(2);
                            txt_nip_bend.Text = nip;
                            txt_jabatan_bend.Text = jabatan;
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
        private void btn_generate_bend_Click(object sender, RoutedEventArgs e)
        {
            if (!edit)
            {
                Properties.Settings.Default.ttd_nama_bend = "";
                Properties.Settings.Default.ttd_jabatan_bend = "";
                Properties.Settings.Default.ttd_nip_bend = "";
                Properties.Settings.Default.ttd_customHeader_bend = "";
                Properties.Settings.Default.Save();
                edit = true;
                cmb_namaBend.IsEnabled = true;
                txt_customHeader_bend.IsEnabled = true;
                generate_txt3.Content = "Simpan";
                iconUbah3.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSaveOutline;
            }
            else
            {
                edit = false;
                txt_customHeader_ppk.IsEnabled = false;
                generate_txt3.Content = "Edit";
                iconUbah3.Kind = MaterialDesignThemes.Wpf.PackIconKind.Buffer;
                if (Properties.Settings.Default.ttd_nama_bend != string.Empty)
                {
                    Properties.Settings.Default.ttd_nama_bend = "";
                    Properties.Settings.Default.ttd_jabatan_bend = "";
                    Properties.Settings.Default.ttd_nip_bend = "";
                    Properties.Settings.Default.ttd_customHeader_bend = "";
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.ttd_nama_bend = cmb_namaBend.Text.ToString();
                    Properties.Settings.Default.ttd_nip_bend = txt_nip_bend.Text.ToString();
                    Properties.Settings.Default.ttd_jabatan_bend = txt_jabatan_bend.Text.ToString();
                    Properties.Settings.Default.ttd_customHeader_bend = txt_customHeader_bend.Text.ToString();
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Data Bendahara Desa Sudah Berhasil Disimpan Bosku!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Properties.Settings.Default.ttd_nama_bend = cmb_namaBend.Text.ToString();
                    Properties.Settings.Default.ttd_nip_bend = txt_nip_bend.Text.ToString();
                    Properties.Settings.Default.ttd_jabatan_bend = txt_jabatan_bend.Text.ToString();
                    Properties.Settings.Default.ttd_customHeader_bend = txt_customHeader_bend.Text.ToString();
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Data Bendahara Desa Sudah Berhasil Disimpan Bosku!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        void load_ttd_bend()
        {
            if (Properties.Settings.Default.ttd_nama_ppk != string.Empty)
            {
                cmb_namaBend.SelectedItem = Properties.Settings.Default.ttd_nama_bend;
                txt_nip_bend.Text = Properties.Settings.Default.ttd_nip_bend;
                txt_jabatan_bend.Text = Properties.Settings.Default.ttd_jabatan_bend;
                txt_customHeader_bend.Text = Properties.Settings.Default.ttd_customHeader_bend;
            }
        }
        private void loadComboboxItem()
        {
            ConnectionUtils c = new ConnectionUtils();
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
                        cmb_namaPPK.Items.Add(r["nama_perangkat"].ToString());
                        cmb_namaBend.Items.Add(r["nama_perangkat"].ToString());
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
}
