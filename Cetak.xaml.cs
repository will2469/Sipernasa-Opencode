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
using System.Data.SQLite;
using System.IO;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Drawing;
using System.Collections.ObjectModel;

namespace Sipernasa
{
    /// <summary>
    /// Interaction logic for Cetak.xaml
    /// </summary>
    public partial class Cetak : UserControl
    {
        public Cetak()
        {
            InitializeComponent();
            //untuk memuat item combobox identitas penandatangan
            load_penandatangan();
            //memuat item combobox jenis perjalanan dinas Dalam/Luar Kabupaten/Kota
            jenis_perjalanan();
            //memuat item combobox nomor agenda yang sudah dibuat selumnya
            loadComboboxItem();
        }
        ConnectionUtils c;
        SQLiteCommand cmd;
        //Memuat Data dari Database SQLite ke DataSet
        private datasetSPPD getData()
        {
            c = new ConnectionUtils();

            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                cc.Open();
                cmd = cc.CreateCommand();
                cmd.CommandText = @"SELECT no_agenda, dasar_surat, tanggal_surat, nama_perangkat, nip, jabatan, maksud_sppd, alat_transportasi, tempat_berangkat, tempat_tujuan, tanggal_berangkat, tanggal_kembali, lama_keberangkatan, pengikut1, jabatan_pengikut1, pengikut2, jabatan_pengikut2, sumber_anggaran, keterangan FROM sppd WHERE tanggal_berangkat >= '" + dp_start.Text + "' AND tanggal_berangkat <= '" + dp_end.Text + "'";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, cc))
                {
                    using (datasetSPPD dss = new datasetSPPD())
                    {
                        da.Fill(dss, "SPPD");
                        return dss;
                    }
                }
            }
        }
        // mengumpulkan semua item penandatangan yang sudah disimpan di pengaturan default
        private void load_penandatangan()
        {
            try
            {
                string kades = Properties.Settings.Default.ttd_jabatan_kades;
                string sekdes = Properties.Settings.Default.ttd_jabatan_sekdes;
                string ppk = Properties.Settings.Default.ttd_jabatan_ppk;
                string[] penandatangan = new string[] { kades, sekdes, ppk };
                try
                {
                    cmb_penandatanganRekap.ItemsSource = penandatangan;
                    cmb_penandatanganRekap.SelectedIndex = 0;
                    try
                    {
                        cmb_penandatanganSPT.ItemsSource = penandatangan;
                        cmb_penandatanganSPT.SelectedIndex = 0;
                        try
                        {
                            cmb_penandatanganSPPD.ItemsSource = penandatangan;
                            cmb_penandatanganSPPD.SelectedIndex = 0;
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
        // mengklarifikasikan jenis perjalanan dinas menjadi Dalam/Luar Kabupaten/Kota guna menentukan besaran nominal pembiayaan perhari
        private void jenis_perjalanan()
        {
            string[] jenisPerjalanan = new string[] { "Dalam Kabupaten/Kota", "Luar Kabupaten/Kota" };
            try
            {
                cmb_jenisPerjalanan.ItemsSource = jenisPerjalanan;
                cmb_jenisPerjalanan.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Mencetak Rekapitulasi Perjalanan Dinas yang dilakukan selama rentang waktu tertentu
        private void btn_printRekap_Click(object sender, RoutedEventArgs e)
        {
            // memastikan terlebih dahulu isi dari rentang waktu yang ditentukan untuk dicetak
            if (dp_start.Text != string.Empty
                && dp_end.Text != string.Empty)
            {
                // deklarasi format report yang sesuai dan memuat Database
                rekapSPPDReport rsr = new rekapSPPDReport();
                datasetSPPD dss = getData();
                rsr.SetDataSource(dss);
                try
                {
                    // menampilkan logo kop surat dari menu pengaturan
                    if (Properties.Settings.Default.logo.ToString() != string.Empty)
                    {
                        try
                        {
                            string imgPathSource = Properties.Settings.Default.logo;
                            rsr.SetParameterValue("imgPath", imgPathSource);
                            try
                            {
                                //menampilkan informasi Detail Desa Ke Kop Surat
                                if (Properties.Settings.Default.Kabupaten.ToString() != string.Empty
                                    && Properties.Settings.Default.Kecamatan.ToString() != string.Empty
                                    && Properties.Settings.Default.Desa.ToString() != string.Empty)
                                {
                                    // Mengumpulkan Detail Pada Report yang Memuat Detail Desa
                                    TextObject txt_namaKab = (TextObject)rsr.ReportDefinition.Sections["Section1"].ReportObjects["namaKab"];
                                    TextObject txt_namaKec = (TextObject)rsr.ReportDefinition.Sections["Section1"].ReportObjects["namaKec"];
                                    TextObject txt_namaDesa = (TextObject)rsr.ReportDefinition.Sections["Section1"].ReportObjects["namaDesa"];
                                    TextObject txt_alamatDesa = (TextObject)rsr.ReportDefinition.Sections["Section1"].ReportObjects["alamatDesa"];
                                    TextObject txt_startDate = (TextObject)rsr.ReportDefinition.Sections["Section2"].ReportObjects["startDate"];
                                    TextObject txt_endDate = (TextObject)rsr.ReportDefinition.Sections["Section2"].ReportObjects["endDate"];
                                    TextObject txt_bottomNamaDesa = (TextObject)rsr.ReportDefinition.Sections["Section4"].ReportObjects["bottomNamaDesa"];
                                    // Mengatur Font Detail agar sesuai dengan format Report
                                    Font font1 = new Font("Bookman Old Style", 12, System.Drawing.FontStyle.Bold);
                                    Font font2 = new Font("Bookman Old Style", 14, System.Drawing.FontStyle.Bold);
                                    try
                                    {
                                        txt_namaKab.Text = Properties.Settings.Default.Kabupaten.ToUpper();
                                        txt_namaKec.Text = Properties.Settings.Default.Kecamatan.ToUpper();
                                        txt_namaDesa.Text = Properties.Settings.Default.Desa.ToUpper();
                                        txt_namaKab.ApplyFont(font1);
                                        txt_namaKec.ApplyFont(font1);
                                        txt_namaDesa.ApplyFont(font2);
                                        txt_alamatDesa.Text = Properties.Settings.Default.Alamat;
                                        txt_startDate.Text = dp_start.Text;
                                        txt_startDate.ApplyFont(font1);
                                        txt_endDate.Text = dp_end.Text;
                                        txt_endDate.ApplyFont(font1);
                                        txt_bottomNamaDesa.Text = Properties.Settings.Default.Desa + ",";
                                        try
                                        {
                                            TextObject txt_customHeader = (TextObject)rsr.ReportDefinition.Sections["Section4"].ReportObjects["customHeader"];
                                            TextObject txt_jabatanttd = (TextObject)rsr.ReportDefinition.Sections["Section4"].ReportObjects["jabatanttd"];
                                            TextObject txt_namattd = (TextObject)rsr.ReportDefinition.Sections["Section4"].ReportObjects["namattd"];
                                            if (cmb_penandatanganRekap.SelectedIndex == 0)
                                            {
                                                txt_customHeader.Text = Properties.Settings.Default.ttd_customHeader_kades;
                                                txt_jabatanttd.Text = Properties.Settings.Default.ttd_jabatan_kades;
                                                txt_namattd.Text = "(" + Properties.Settings.Default.ttd_nama_kades.ToUpper() + ")";
                                            }
                                            else if (cmb_penandatanganRekap.SelectedIndex == 1)
                                            {
                                                txt_customHeader.Text = Properties.Settings.Default.ttd_customHeader_sekdes;
                                                txt_jabatanttd.Text = Properties.Settings.Default.ttd_jabatan_sekdes;
                                                txt_namattd.Text = "(" + Properties.Settings.Default.ttd_nama_sekdes.ToUpper() + ")";
                                            }
                                            else if (cmb_penandatanganRekap.SelectedIndex == 2)
                                            {
                                                txt_customHeader.Text = Properties.Settings.Default.ttd_customHeader_ppk;
                                                txt_jabatanttd.Text = Properties.Settings.Default.ttd_jabatan_ppk;
                                                txt_namattd.Text = "(" + Properties.Settings.Default.ttd_nama_ppk.ToUpper() + ")";
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
                                else
                                {
                                    MessageBox.Show("Mohon isi detail informasidesa yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    DashboardWindow dw = new DashboardWindow();
                                    dw.SwitcherPane.Children.Clear();
                                    dw.SwitcherPane.Children.Add(new Pengaturan());
                                    dw.currentMenu.Content = "Pengaturan";
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
                    else
                    {
                        MessageBox.Show("Mohon pilih logo Kop Surat yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        DashboardWindow dw = new DashboardWindow();
                        dw.SwitcherPane.Children.Clear();
                        dw.SwitcherPane.Children.Add(new Pengaturan());
                        dw.currentMenu.Content = "Pengaturan";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    rpt_rekapSPPDview.ViewerCore.ReportSource = rsr;
                }
            }
            else
            {
                MessageBox.Show("Harap Masukkan rentang tanggal yang akan dicetak Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //memuat nomor agenda dari Database ke combobox sebagai dasar penentuan laporan yang akan dicetak
        private void loadComboboxItem()
        {
            c = new ConnectionUtils();
            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    string query = @"select no_agenda from sppd order by no_agenda desc";
                    cmd.CommandText = query;
                    cmd.Connection = cc;
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        cmb_noAgendaSPT.Items.Add(r["no_agenda"].ToString());
                        cmb_noAgendaSPPD.Items.Add(r["no_agenda"].ToString());
                        cmb_noAgendaLPD.Items.Add(r["no_agenda"].ToString());
                        cmb_noAgendaRBPD.Items.Add(r["no_agenda"].ToString());
                    }
                    cc.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        //memuat informasi DataBase SQLite ke DataSet Sebagai Bahan Laporan SPT
        private datasetSPPD getDataSPT()
        {
            c = new ConnectionUtils();

            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                cc.Open();
                cmd = cc.CreateCommand();
                cmd.CommandText = @"SELECT no_agenda, dasar_surat, tanggal_surat, nama_perangkat, nip, jabatan, maksud_sppd, alat_transportasi, tempat_berangkat, tempat_tujuan, tanggal_berangkat, tanggal_kembali, lama_keberangkatan, pengikut1, jabatan_pengikut1, pengikut2, jabatan_pengikut2, sumber_anggaran, keterangan FROM sppd WHERE no_agenda = '" + cmb_noAgendaSPT.Text + "'";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, cc))
                {
                    using (datasetSPPD dss = new datasetSPPD())
                    {
                        da.Fill(dss, "SPPD");
                        return dss;
                    }
                }
            }
        }
        //mencetak SPT
        private void btn_printSPT_Click(object sender, RoutedEventArgs e)
        {
            //memastikan No Agenda sudah terpilih
            if (cmb_noAgendaSPT.Text != string.Empty)
            {
                // Menentukan Jenis dan DataBase Report yang akan dipakai
                SPTReport rspt = new SPTReport();
                datasetSPPD dsspt = getDataSPT();
                rspt.SetDataSource(dsspt);
                try
                {
                    try
                    {
                        //Memuat Logo
                        if (Properties.Settings.Default.logo.ToString() != string.Empty)
                        {
                            string imgpathSourceSPT = Properties.Settings.Default.logo;
                            rspt.SetParameterValue("logo_spt", imgpathSourceSPT);
                            try
                            {
                                //Memuat Detail Informasi Desa
                                if (Properties.Settings.Default.Kabupaten.ToString() != string.Empty
                                    && Properties.Settings.Default.Kecamatan.ToString() != string.Empty
                                    && Properties.Settings.Default.Desa.ToString() != string.Empty)
                                {
                                    try
                                    {
                                        //memuat object detail report
                                        TextObject txt_sptKab = (TextObject)rspt.ReportDefinition.Sections["Section1"].ReportObjects["sptKab"];
                                        TextObject txt_sptKec = (TextObject)rspt.ReportDefinition.Sections["Section1"].ReportObjects["sptKec"];
                                        TextObject txt_sptDesa = (TextObject)rspt.ReportDefinition.Sections["Section1"].ReportObjects["sptDesa"];
                                        TextObject txt_sptAlamat = (TextObject)rspt.ReportDefinition.Sections["Section1"].ReportObjects["sptAlamat"];
                                        TextObject txt_sptBottomDesa = (TextObject)rspt.ReportDefinition.Sections["Section4"].ReportObjects["sptBottomDesa"];
                                        //menyesuaikan font
                                        Font font1 = new Font("Bookman Old Style", 12, System.Drawing.FontStyle.Bold);
                                        Font font2 = new Font("Bookman Old Style", 14, System.Drawing.FontStyle.Bold);
                                        //memasukkan informasi ke object report
                                        try
                                        {
                                            txt_sptKab.Text = Properties.Settings.Default.Kabupaten.ToUpper();
                                            txt_sptKec.Text = Properties.Settings.Default.Kecamatan.ToUpper();
                                            txt_sptDesa.Text = Properties.Settings.Default.Desa.ToUpper();
                                            txt_sptKab.ApplyFont(font1);
                                            txt_sptKec.ApplyFont(font1);
                                            txt_sptDesa.ApplyFont(font2);
                                            txt_sptAlamat.Text = Properties.Settings.Default.Alamat;
                                            txt_sptBottomDesa.Text = Properties.Settings.Default.Desa.ToUpper();
                                            try
                                            {
                                                //Memuat Nomor Agenda Ke Laporan
                                                if (cmb_noAgendaSPT.Text.ToString() != string.Empty)
                                                {
                                                    string no_agendaOri = cmb_noAgendaSPT.Text.ToString();
                                                    //membuat string agenda
                                                    var renderNoAgenda = new StringBuilder();
                                                    renderNoAgenda.Append("No :");
                                                    renderNoAgenda.Append(no_agendaOri);
                                                    string NoAgenda_result = renderNoAgenda.ToString();
                                                    TextObject txt_sptNoAgenda = (TextObject)rspt.ReportDefinition.Sections["Section2"].ReportObjects["sptNoAgenda"];
                                                    try
                                                    {
                                                        txt_sptNoAgenda.Text = NoAgenda_result.ToString();
                                                        try
                                                        {

                                                            TextObject txt_sptCustomHeader = (TextObject)rspt.ReportDefinition.Sections["Section4"].ReportObjects["sptCustomHeader"];
                                                            TextObject txt_sptJabatanTTD = (TextObject)rspt.ReportDefinition.Sections["Section4"].ReportObjects["sptJabatanTTD"];
                                                            TextObject txt_sptNamaTTD = (TextObject)rspt.ReportDefinition.Sections["Section4"].ReportObjects["sptNamaTTD"];
                                                            try
                                                            {
                                                                if (cmb_penandatanganSPT.SelectedIndex == 0)
                                                                {
                                                                    txt_sptCustomHeader.Text = Properties.Settings.Default.ttd_customHeader_kades;
                                                                    txt_sptJabatanTTD.Text = Properties.Settings.Default.ttd_jabatan_kades;
                                                                    txt_sptNamaTTD.Text = "(" + Properties.Settings.Default.ttd_nama_kades.ToUpper() + ")";
                                                                }
                                                                else if (cmb_penandatanganSPT.SelectedIndex == 1)
                                                                {
                                                                    txt_sptCustomHeader.Text = Properties.Settings.Default.ttd_customHeader_sekdes;
                                                                    txt_sptJabatanTTD.Text = Properties.Settings.Default.ttd_jabatan_sekdes;
                                                                    txt_sptNamaTTD.Text = "(" + Properties.Settings.Default.ttd_nama_sekdes.ToUpper() + ")";
                                                                }
                                                                else if (cmb_penandatanganSPT.SelectedIndex == 2)
                                                                {
                                                                    txt_sptCustomHeader.Text = Properties.Settings.Default.ttd_customHeader_ppk;
                                                                    txt_sptJabatanTTD.Text = Properties.Settings.Default.ttd_jabatan_ppk;
                                                                    txt_sptNamaTTD.Text = "(" + Properties.Settings.Default.ttd_nama_ppk.ToUpper() + ")";
                                                                }
                                                                else
                                                                {
                                                                    MessageBox.Show("Data Kosong Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                                                    catch (Exception ex)
                                                    {
                                                        MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Harap pilih No. Agenda yang akan dicetak dulu bosku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Mohon isi detail informasi desa yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    DashboardWindow dw = new DashboardWindow();
                                    dw.SwitcherPane.Children.Clear();
                                    dw.SwitcherPane.Children.Add(new Pengaturan());
                                    dw.currentMenu.Content = "Pengaturan";
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Mohon pilih logo Kop Surat yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            DashboardWindow dw = new DashboardWindow();
                            dw.SwitcherPane.Children.Clear();
                            dw.SwitcherPane.Children.Add(new Pengaturan());
                            dw.currentMenu.Content = "Pengaturan";
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
                finally
                {
                    rpt_SPTview.ViewerCore.ReportSource = rspt;
                }
            }
            else
            {
                MessageBox.Show("Harap pilih No. Agenda yang akan dicetak dulu bosku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //memuat DataBase SQLite ke DataSet untuk Laporan SPPD
        private datasetSPPD getDataSPPD()
        {
            c = new ConnectionUtils();

            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                cc.Open();
                cmd = cc.CreateCommand();
                cmd.CommandText = @"SELECT no_agenda, dasar_surat, tanggal_surat, nama_perangkat, nip, jabatan, maksud_sppd, alat_transportasi, tempat_berangkat, tempat_tujuan, tanggal_berangkat, tanggal_kembali, lama_keberangkatan, pengikut1, jabatan_pengikut1, pengikut2, jabatan_pengikut2, sumber_anggaran, keterangan FROM sppd WHERE no_agenda = '" + cmb_noAgendaSPPD.Text + "'";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, cc))
                {
                    using (datasetSPPD dss = new datasetSPPD())
                    {
                        da.Fill(dss, "SPPD");
                        return dss;
                    }
                }
            }
        }
        //mencetak SPPD
        private void btn_printSPPD_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_noAgendaSPPD.Text != string.Empty)
            {
                // Menentukan Jenis dan DataBase Report yang akan dipakai
                SPPDReport sr = new SPPDReport();
                datasetSPPD dssppd = getDataSPPD();
                sr.SetDataSource(dssppd);
                try
                {
                    if (Properties.Settings.Default.logo.ToString() != string.Empty)
                    {
                        string imgpathSourceSPPD = Properties.Settings.Default.logo;
                        sr.SetParameterValue("logo_sppdKab", imgpathSourceSPPD);
                        try
                        {
                            //Memuat Detail Informasi Desa
                            if (Properties.Settings.Default.Kabupaten.ToString() != string.Empty
                                && Properties.Settings.Default.Kecamatan.ToString() != string.Empty
                                && Properties.Settings.Default.Desa.ToString() != string.Empty)
                            {
                                TextObject txt_sppdKab = (TextObject)sr.ReportDefinition.Sections["Section2"].ReportObjects["sppdKab"];
                                TextObject txt_sppdKec = (TextObject)sr.ReportDefinition.Sections["Section2"].ReportObjects["sppdKec"];
                                TextObject txt_sppdDesa = (TextObject)sr.ReportDefinition.Sections["Section2"].ReportObjects["sppdDesa"];
                                TextObject txt_sppdAlamat = (TextObject)sr.ReportDefinition.Sections["Section2"].ReportObjects["sppdAlamat"];
                                TextObject txt_sppdBottomDesa = (TextObject)sr.ReportDefinition.Sections["Section3"].ReportObjects["sppdBottomDesa"];
                                Font font1 = new Font("Bookman Old Style", 12, System.Drawing.FontStyle.Bold);
                                Font font2 = new Font("Bookman Old Style", 14, System.Drawing.FontStyle.Bold);
                                //memuat object detail report
                                try
                                {
                                    txt_sppdKab.Text = Properties.Settings.Default.Kabupaten.ToUpper();
                                    txt_sppdKec.Text = Properties.Settings.Default.Kecamatan.ToUpper();
                                    txt_sppdDesa.Text = Properties.Settings.Default.Desa.ToUpper();
                                    txt_sppdKab.ApplyFont(font1);
                                    txt_sppdKec.ApplyFont(font1);
                                    txt_sppdDesa.ApplyFont(font2);
                                    txt_sppdAlamat.Text = Properties.Settings.Default.Alamat;
                                    txt_sppdBottomDesa.Text = Properties.Settings.Default.Desa.ToUpper();
                                    try
                                    {
                                        //Memuat Nomor Agenda Ke Laporan
                                        if (cmb_noAgendaSPPD.Text.ToString() != string.Empty)
                                        {
                                            string p = cmb_noAgendaSPPD.Text.ToString();
                                            //menentukan Lembar SPPD
                                            int l = 4;
                                            int m = 3;
                                            string o = p.Substring(l, m);
                                            TextObject txt_sppdLembar = (TextObject)sr.ReportDefinition.Sections["Section3"].ReportObjects["sppdLembar"];
                                            TextObject txt_sppdPPK1 = (TextObject)sr.ReportDefinition.Sections["Section3"].ReportObjects["sppdPPK1"];
                                            try
                                            {
                                                txt_sppdLembar.Text = o.ToString();
                                                txt_sppdPPK1.Text = cmb_penandatanganSPPD.Text;
                                                try
                                                {
                                                    TextObject txt_sppdCustomHeader = (TextObject)sr.ReportDefinition.Sections["Section3"].ReportObjects["sppdCustomHeader"];
                                                    TextObject txt_sppdJabatanTTD = (TextObject)sr.ReportDefinition.Sections["Section3"].ReportObjects["sppdJabatanTTD"];
                                                    TextObject txt_sppdNamaTTD = (TextObject)sr.ReportDefinition.Sections["Section3"].ReportObjects["sppdNamaTTD"];
                                                    TextObject txt_sppdCustomHeaderPPK1 = (TextObject)sr.ReportDefinition.Sections["DetailSection2"].ReportObjects["sppdCustomHeaderPPK1"];
                                                    TextObject txt_sppdJabatan1 = (TextObject)sr.ReportDefinition.Sections["DetailSection2"].ReportObjects["sppdJabatan1"];
                                                    TextObject txt_sppdNama1 = (TextObject)sr.ReportDefinition.Sections["DetailSection2"].ReportObjects["sppdNama1"];
                                                    TextObject txt_sppdCustomHeader2 = (TextObject)sr.ReportDefinition.Sections["DetailSection2"].ReportObjects["sppdCustomHeader2"];
                                                    TextObject txt_sppdJabatan2 = (TextObject)sr.ReportDefinition.Sections["DetailSection2"].ReportObjects["sppdJabatan2"];
                                                    TextObject txt_sppdNama2 = (TextObject)sr.ReportDefinition.Sections["DetailSection2"].ReportObjects["sppdNama2"];
                                                    TextObject txt_sppdCustomHeader3 = (TextObject)sr.ReportDefinition.Sections["DetailSection2"].ReportObjects["sppdCustomHeader3"];
                                                    TextObject txt_sppdJabatan3 = (TextObject)sr.ReportDefinition.Sections["DetailSection2"].ReportObjects["sppdJabatan3"];
                                                    TextObject txt_sppdNama3 = (TextObject)sr.ReportDefinition.Sections["DetailSection2"].ReportObjects["sppdNama3"];
                                                    try
                                                    {
                                                        if (cmb_penandatanganSPPD.SelectedIndex == 0)
                                                        {
                                                            txt_sppdCustomHeader.Text = Properties.Settings.Default.ttd_customHeader_kades;
                                                            txt_sppdJabatanTTD.Text = Properties.Settings.Default.ttd_jabatan_kades;
                                                            txt_sppdNamaTTD.Text = "(" + Properties.Settings.Default.ttd_nama_kades.ToUpper() + ")";
                                                            txt_sppdCustomHeaderPPK1.Text = Properties.Settings.Default.ttd_customHeader_kades;
                                                            txt_sppdJabatan1.Text = Properties.Settings.Default.ttd_jabatan_kades;
                                                            txt_sppdNama1.Text = "(" + Properties.Settings.Default.ttd_nama_kades.ToUpper() + ")";
                                                            txt_sppdCustomHeader2.Text = Properties.Settings.Default.ttd_customHeader_kades;
                                                            txt_sppdJabatan2.Text = Properties.Settings.Default.ttd_jabatan_kades;
                                                            txt_sppdNama2.Text = "(" + Properties.Settings.Default.ttd_nama_kades.ToUpper() + ")";
                                                            txt_sppdCustomHeader3.Text = Properties.Settings.Default.ttd_customHeader_ppk;
                                                            txt_sppdJabatan3.Text = Properties.Settings.Default.ttd_jabatan_ppk;
                                                            txt_sppdNama3.Text = "(" + Properties.Settings.Default.ttd_nama_ppk.ToUpper() + ")";
                                                        }
                                                        else if (cmb_penandatanganSPT.SelectedIndex == 1)
                                                        {
                                                            txt_sppdCustomHeader.Text = Properties.Settings.Default.ttd_customHeader_sekdes;
                                                            txt_sppdJabatanTTD.Text = Properties.Settings.Default.ttd_jabatan_sekdes;
                                                            txt_sppdNamaTTD.Text = "(" + Properties.Settings.Default.ttd_nama_sekdes.ToUpper() + ")";
                                                            txt_sppdCustomHeaderPPK1.Text = Properties.Settings.Default.ttd_customHeader_sekdes;
                                                            txt_sppdJabatan1.Text = Properties.Settings.Default.ttd_jabatan_sekdes;
                                                            txt_sppdNama1.Text = "(" + Properties.Settings.Default.ttd_nama_sekdes.ToUpper() + ")";
                                                            txt_sppdCustomHeader2.Text = Properties.Settings.Default.ttd_customHeader_sekdes;
                                                            txt_sppdJabatan2.Text = Properties.Settings.Default.ttd_jabatan_sekdes;
                                                            txt_sppdNama2.Text = "(" + Properties.Settings.Default.ttd_nama_sekdes.ToUpper() + ")";
                                                            txt_sppdCustomHeader3.Text = Properties.Settings.Default.ttd_customHeader_ppk;
                                                            txt_sppdJabatan3.Text = Properties.Settings.Default.ttd_jabatan_ppk;
                                                            txt_sppdNama3.Text = "(" + Properties.Settings.Default.ttd_nama_ppk.ToUpper() + ")";
                                                        }
                                                        else if (cmb_penandatanganSPT.SelectedIndex == 2)
                                                        {
                                                            txt_sppdCustomHeader.Text = Properties.Settings.Default.ttd_customHeader_ppk;
                                                            txt_sppdJabatanTTD.Text = Properties.Settings.Default.ttd_jabatan_ppk;
                                                            txt_sppdNamaTTD.Text = "(" + Properties.Settings.Default.ttd_nama_ppk.ToUpper() + ")";
                                                            txt_sppdCustomHeaderPPK1.Text = Properties.Settings.Default.ttd_customHeader_ppk;
                                                            txt_sppdJabatan1.Text = Properties.Settings.Default.ttd_jabatan_ppk;
                                                            txt_sppdNama1.Text = "(" + Properties.Settings.Default.ttd_nama_ppk.ToUpper() + ")";
                                                            txt_sppdCustomHeader2.Text = Properties.Settings.Default.ttd_customHeader_ppk;
                                                            txt_sppdJabatan2.Text = Properties.Settings.Default.ttd_jabatan_ppk;
                                                            txt_sppdNama2.Text = "(" + Properties.Settings.Default.ttd_nama_ppk.ToUpper() + ")";
                                                            txt_sppdCustomHeader3.Text = Properties.Settings.Default.ttd_customHeader_ppk;
                                                            txt_sppdJabatan3.Text = Properties.Settings.Default.ttd_jabatan_ppk;
                                                            txt_sppdNama3.Text = "(" + Properties.Settings.Default.ttd_nama_ppk.ToUpper() + ")";
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Data Kosong Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Harap pilih No. Agenda yang akan dicetak dulu bosku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                            else
                            {
                                MessageBox.Show("Mohon isi detail informasi desa yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                DashboardWindow dw = new DashboardWindow();
                                dw.SwitcherPane.Children.Clear();
                                dw.SwitcherPane.Children.Add(new Pengaturan());
                                dw.currentMenu.Content = "Pengaturan";
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Mohon pilih logo Kop Surat yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        DashboardWindow dw = new DashboardWindow();
                        dw.SwitcherPane.Children.Clear();
                        dw.SwitcherPane.Children.Add(new Pengaturan());
                        dw.currentMenu.Content = "Pengaturan";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    rpt_SPPDview.ViewerCore.ReportSource = sr;
                }
            }
            else
            {
                MessageBox.Show("Harap pilih No. Agenda yang akan dicetak bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private datasetSPPD getDataLPD()
        {
            c = new ConnectionUtils();

            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                cc.Open();
                cmd = cc.CreateCommand();
                cmd.CommandText = @"SELECT no_agenda, dasar_surat, tanggal_surat, nama_perangkat, nip, jabatan, maksud_sppd, alat_transportasi, tempat_berangkat, tempat_tujuan, tanggal_berangkat, tanggal_kembali, lama_keberangkatan, pengikut1, jabatan_pengikut1, pengikut2, jabatan_pengikut2, sumber_anggaran, keterangan FROM sppd WHERE no_agenda = '" + cmb_noAgendaLPD.Text + "'";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, cc))
                {
                    using (datasetSPPD dss = new datasetSPPD())
                    {
                        da.Fill(dss, "SPPD");
                        return dss;
                    }
                }
            }
        }
        private void btn_printLPD_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_noAgendaLPD.Text != string.Empty)
            {
                LPDReport lr = new LPDReport();
                datasetSPPD dslpd = getDataLPD();
                lr.SetDataSource(dslpd);
                try
                {
                    try
                    {
                        if (Properties.Settings.Default.logo.ToString() != string.Empty)
                        {
                            string imgpathSourceLPD = Properties.Settings.Default.logo;
                            try
                            {
                                lr.SetParameterValue("logo_lpd", imgpathSourceLPD);
                                try
                                {
                                    //Memuat Detail Informasi Desa
                                    if (Properties.Settings.Default.Kabupaten.ToString() != string.Empty
                                        && Properties.Settings.Default.Kecamatan.ToString() != string.Empty
                                        && Properties.Settings.Default.Desa.ToString() != string.Empty)
                                    {
                                        TextObject txt_lpdKab = (TextObject)lr.ReportDefinition.Sections["Section1"].ReportObjects["lpdKab"];
                                        TextObject txt_lpdKec = (TextObject)lr.ReportDefinition.Sections["Section1"].ReportObjects["lpdKec"];
                                        TextObject txt_lpdDesa = (TextObject)lr.ReportDefinition.Sections["Section1"].ReportObjects["lpdDesa"];
                                        TextObject txt_lpdAlamat = (TextObject)lr.ReportDefinition.Sections["Section1"].ReportObjects["lpdAlamat"];
                                        TextObject txt_lpdLokasi = (TextObject)lr.ReportDefinition.Sections["Section3"].ReportObjects["lpdLokasi"];
                                        TextObject txt_lpdDesaKades = (TextObject)lr.ReportDefinition.Sections["Section3"].ReportObjects["lpdDesaKades"];
                                        TextObject txt_lpdBottomDesa = (TextObject)lr.ReportDefinition.Sections["Section4"].ReportObjects["lpdBottomDesa"];
                                        Font font1 = new Font("Bookman Old Style", 12, System.Drawing.FontStyle.Bold);
                                        Font font2 = new Font("Bookman Old Style", 14, System.Drawing.FontStyle.Bold);
                                        try
                                        {
                                            txt_lpdKab.Text = Properties.Settings.Default.Kabupaten.ToUpper();
                                            txt_lpdKec.Text = Properties.Settings.Default.Kecamatan.ToUpper();
                                            txt_lpdDesa.Text = Properties.Settings.Default.Desa.ToUpper();
                                            txt_lpdAlamat.Text = Properties.Settings.Default.Alamat;
                                            txt_lpdLokasi.Text = Properties.Settings.Default.Desa;
                                            txt_lpdDesaKades.Text = Properties.Settings.Default.Desa;
                                            txt_lpdBottomDesa.Text = Properties.Settings.Default.Desa;
                                            txt_lpdKab.ApplyFont(font1);
                                            txt_lpdKec.ApplyFont(font1);
                                            txt_lpdDesa.ApplyFont(font2);
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Mohon isi detail informasi desa yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        DashboardWindow dw = new DashboardWindow();
                                        dw.SwitcherPane.Children.Clear();
                                        dw.SwitcherPane.Children.Add(new Pengaturan());
                                        dw.currentMenu.Content = "Pengaturan";
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
                        else
                        {
                            MessageBox.Show("Mohon pilih logo Kop Surat yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            DashboardWindow dw = new DashboardWindow();
                            dw.SwitcherPane.Children.Clear();
                            dw.SwitcherPane.Children.Add(new Pengaturan());
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
                finally
                {
                    rpt_LPDview.ViewerCore.ReportSource = lr;
                }
            }
            else
            {
                MessageBox.Show("Harap Pilih No. Agenda yang akan dicetak Bosku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private datasetSPPD getDataRBPD()
        {
            c = new ConnectionUtils();

            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                cc.Open();
                cmd = cc.CreateCommand();
                cmd.CommandText = @"SELECT no_agenda, dasar_surat, tanggal_surat, nama_perangkat, nip, jabatan, maksud_sppd, alat_transportasi, tempat_berangkat, tempat_tujuan, tanggal_berangkat, tanggal_kembali, lama_keberangkatan, pengikut1, jabatan_pengikut1, pengikut2, jabatan_pengikut2, sumber_anggaran, keterangan FROM sppd WHERE no_agenda = '" + cmb_noAgendaRBPD.Text + "'";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, cc))
                {
                    using (datasetSPPD dss = new datasetSPPD())
                    {
                        da.Fill(dss, "SPPD");
                        return dss;
                    }
                }
            }
        }
        private void btn_printRBPD_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_noAgendaRBPD.Text != string.Empty)
            {
                RBPDReport rr = new RBPDReport();
                datasetSPPD ds = getDataRBPD();
                rr.SetDataSource(ds);
                try
                {
                    try
                    {
                        if (Properties.Settings.Default.logo.ToString() != string.Empty)
                        {
                            string imgpathSourceRBPD = Properties.Settings.Default.logo;
                            try
                            {
                                rr.SetParameterValue("logo_rbpd", imgpathSourceRBPD);
                                try
                                {
                                    TextObject txt_rbpdKab = (TextObject)rr.ReportDefinition.Sections["Section1"].ReportObjects["rbpdKab"];
                                    TextObject txt_rbpdKec = (TextObject)rr.ReportDefinition.Sections["Section1"].ReportObjects["rbpdKec"];
                                    TextObject txt_rbpdDesa = (TextObject)rr.ReportDefinition.Sections["Section1"].ReportObjects["rbpdDesa"];
                                    TextObject txt_lpdAlamat = (TextObject)rr.ReportDefinition.Sections["Section1"].ReportObjects["rbpdAlamat"];
                                    TextObject txt_rbpdBottomDesa = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["rbpdBottomDesa"];
                                    TextObject txt_rbpdNamaBend = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["rbpdNamaBend"];
                                    TextObject txt_rbpdNamaTTD = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["rbpdNamaTTD"];
                                    TextObject txt_rbpdJabatanTTD = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["rbpdJabatanTTD"];
                                    Font font1 = new Font("Bookman Old Style", 12, System.Drawing.FontStyle.Bold);
                                    Font font2 = new Font("Bookman Old Style", 14, System.Drawing.FontStyle.Bold);
                                    try
                                    {
                                        txt_rbpdKab.Text = Properties.Settings.Default.Kabupaten.ToUpper();
                                        txt_rbpdKec.Text = Properties.Settings.Default.Kecamatan.ToUpper();
                                        txt_rbpdDesa.Text = Properties.Settings.Default.Desa.ToUpper();
                                        txt_lpdAlamat.Text = Properties.Settings.Default.Alamat;
                                        txt_rbpdBottomDesa.Text = Properties.Settings.Default.Desa;
                                        txt_rbpdNamaBend.Text = "( " + Properties.Settings.Default.ttd_nama_bend + " )";
                                        txt_rbpdNamaTTD.Text = "( " + Properties.Settings.Default.ttd_nama_ppk + " )";
                                        txt_rbpdJabatanTTD.Text = Properties.Settings.Default.ttd_jabatan_ppk;
                                        txt_rbpdKab.ApplyFont(font1);
                                        txt_rbpdKec.ApplyFont(font1);
                                        txt_rbpdDesa.ApplyFont(font2);
                                        try
                                        {
                                            if (Properties.Settings.Default.lkFix != string.Empty)
                                            {
                                                string lk = Properties.Settings.Default.lkFix;
                                                decimal lkCal = Convert.ToDecimal(lk);
                                                TextObject txt_rpbdBiaya = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["rpbdBiaya"];
                                                TextObject txt_rpbd_jumlahBiaya = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["rpbd_jumlahBiaya"];
                                                TextObject txt_rpbdTotalBiaya = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["rpbdTotalBiaya"];
                                                TextObject txt_rpbdTerbilang = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["rpbdTerbilang"];
                                                TextObject txt_tetapanJumlah = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["tetapanJumlah"];
                                                TextObject txt_rbpdNpengikut = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["rbpdNpengikut"];
                                                TextObject txt_pembiayaanPengikut = (TextObject)rr.ReportDefinition.Sections["Section3"].ReportObjects["pembiayaanPengikut"];
                                                string xxx = cmb_jenisPerjalanan.SelectedItem.ToString();
                                                if (xxx == "Dalam Kabupaten/Kota")
                                                {
                                                    if (Properties.Settings.Default.biayaDalamKota.ToString() != string.Empty)
                                                    {
                                                        string BiayaDalamKotaRaw = Properties.Settings.Default.biayaDalamKota;
                                                        decimal BiayaDalamKotaToConvert = Convert.ToDecimal(BiayaDalamKotaRaw);
                                                        decimal nPengikut = getCountPengikut();
                                                        string biayaCalDK = Properties.Settings.Default.biayaDalamKota;
                                                        try
                                                        {
                                                            decimal jumlahDec = (lkCal * Convert.ToDecimal(biayaCalDK));
                                                            decimal biayaPengikut = (nPengikut * jumlahDec);
                                                            decimal totalBiaya = jumlahDec + biayaPengikut;
                                                            int jumlahDK = Convert.ToInt32(totalBiaya);
                                                            try
                                                            {
                                                                string BiayaDalamKotaFormatted = string.Format("{0:n0}", BiayaDalamKotaToConvert) + ",-";
                                                                string biayaPengikutFormatted = string.Format("{0:n0}", biayaPengikut) + ",-";
                                                                string jumlahBiayaThoushand = string.Format("{0:n0}", jumlahDec);
                                                                string totalBiayaFormatted = string.Format("{0:n0}", totalBiaya) + ",-";
                                                                string terbilangTotalDK = TerbilangHelper.Terbilang(jumlahDK) + " RUPIAH";
                                                                try
                                                                {
                                                                    string jumlahBiaya = jumlahBiayaThoushand + ",-";
                                                                    try
                                                                    {
                                                                        txt_rbpdNpengikut.Text = nPengikut.ToString();
                                                                        txt_rpbdBiaya.Text = BiayaDalamKotaFormatted;
                                                                        txt_rpbd_jumlahBiaya.Text = jumlahBiaya;
                                                                        txt_rpbdTotalBiaya.Text = totalBiayaFormatted;
                                                                        txt_pembiayaanPengikut.Text = biayaPengikutFormatted;
                                                                        txt_tetapanJumlah.Text = totalBiayaFormatted;
                                                                        txt_rpbdTerbilang.Text = terbilangTotalDK;
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
                                                    else
                                                    {
                                                        MessageBox.Show("Mohon isi beban biaya perjalanan Dalam Kabupaten/Kota yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                        DashboardWindow dw = new DashboardWindow();
                                                        dw.SwitcherPane.Children.Clear();
                                                        dw.SwitcherPane.Children.Add(new Pengaturan());
                                                        dw.currentMenu.Content = "Pengaturan";
                                                    }
                                                }
                                                else if (xxx == "Luar Kabupaten/Kota")
                                                {
                                                    if (Properties.Settings.Default.biayaLuarKota.ToString() != string.Empty)
                                                    {
                                                        string BiayaLuarKotaRaw = Properties.Settings.Default.biayaLuarKota;
                                                        decimal BiayaLuarKotaToConvert = Convert.ToDecimal(BiayaLuarKotaRaw);
                                                        decimal nPengikut = getCountPengikut();
                                                        int biayaCalLK = IntegerExtensions.ParseInt(Properties.Settings.Default.biayaLuarKota);
                                                        try
                                                        {
                                                            decimal jumlahDec = (lkCal * Convert.ToDecimal(biayaCalLK));
                                                            decimal biayaPengikut = (nPengikut * jumlahDec);
                                                            decimal totalBiaya = jumlahDec + biayaPengikut;
                                                            int jumlahLK = Convert.ToInt32(totalBiaya);
                                                            try
                                                            {
                                                                string terbilangTotallK = TerbilangHelper.Terbilang(jumlahLK) + " RUPIAH";
                                                                string jumlahBiaya = string.Format("{0:n0}", jumlahDec) + ",-";
                                                                string totalBiayaFormat = string.Format("{0:n0}", totalBiaya) + ",-";
                                                                string biayaPengikutFormat = string.Format("{0:n0}", biayaPengikut) + ",-";
                                                                string BiayaLuarKotaFormatted = string.Format("{0:n0}", BiayaLuarKotaToConvert) + ",-";
                                                                try
                                                                {
                                                                    txt_rpbdBiaya.Text = BiayaLuarKotaFormatted;
                                                                    txt_rpbd_jumlahBiaya.Text = jumlahBiaya;
                                                                    txt_pembiayaanPengikut.Text = biayaPengikutFormat;
                                                                    txt_rpbdTotalBiaya.Text = totalBiayaFormat;
                                                                    txt_tetapanJumlah.Text = totalBiayaFormat;
                                                                    txt_rpbdTerbilang.Text = terbilangTotallK;
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
                                                        catch (Exception ex)
                                                        {
                                                            MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Mohon isi beban biaya perjalanan Luar Kabupaten/Kota yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                        DashboardWindow dw = new DashboardWindow();
                                                        dw.SwitcherPane.Children.Clear();
                                                        dw.SwitcherPane.Children.Add(new Pengaturan());
                                                        dw.currentMenu.Content = "Pengaturan";
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Harap pilih jenis perjalanan dinas Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show(Properties.Settings.Default.lkFix.ToString());
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message);
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
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Mohon pilih logo Kop Surat yang akan digunakan di menu pengaturan Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            DashboardWindow dw = new DashboardWindow();
                            dw.SwitcherPane.Children.Clear();
                            dw.SwitcherPane.Children.Add(new Pengaturan());
                            dw.currentMenu.Content = "Pengaturan";
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
                finally
                {
                    rpt_RBPDview.ViewerCore.ReportSource = rr;
                }
            }
            else
            {
                MessageBox.Show("Harap Pilih No. Agenda yang akan dicetak Bosku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string getLK()
        {
            c = new ConnectionUtils();
            string lk = string.Empty;
            try
            {
                using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    cmd.CommandText = @"SELECT lama_keberangkatan FROM sppd WHERE no_agenda = '" + cmb_noAgendaRBPD.Text.ToString() + "'";
                    cmd.Connection = cc;
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        lk = r[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                lk = string.Empty;
            }
            return lk;
        }
        private decimal getCountPengikut()
        {
            c = new ConnectionUtils();
            decimal cP = 0;
            try
            {
                using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
                {
                    cc.Open();
                    cmd = new SQLiteCommand();
                    cmd.CommandText = @"SELECT pengikut1, pengikut2 FROM sppd WHERE no_agenda = '" + cmb_noAgendaRBPD.Text.ToString() + "'";
                    cmd.Connection = cc;
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        string p1 = r["pengikut1"].ToString();
                        string p2 = r["pengikut2"].ToString();
                        if (p1 != string.Empty)
                        {
                            cP = 1;
                        }
                        else if (p2 != string.Empty)
                        {
                            cP = 2;
                        }
                        else
                        {
                            cP = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cP = 0;
            }
            return cP;
        }
        private void cmb_jenisPerjalanan_DropDownClosed(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.lkFix != string.Empty)
            {
                Properties.Settings.Default.lkFix = "";
                Properties.Settings.Default.Save();
                string lk = getLK();
                if (lk != string.Empty)
                {
                    string lkForCal = lk.Substring(0, 1);
                    Properties.Settings.Default.lkFix = lkForCal;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("Data Tidak Ditemukan", "Unkown Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                string lk = getLK();
                if (lk != string.Empty)
                {
                    string lkForCal = lk.Substring(0, 1);
                    Properties.Settings.Default.lkFix = lkForCal;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("Data Tidak Ditemukan", "Unkown Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
