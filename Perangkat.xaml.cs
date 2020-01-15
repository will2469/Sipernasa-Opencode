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
using System.Data;
using System.IO;

namespace Sipernasa
{
    /// <summary>
    /// Interaction logic for Perangkat.xaml
    /// </summary>
    public partial class Perangkat : UserControl
    {
        public Perangkat()
        {
            InitializeComponent();
        }
        SQLiteCommand cmd;
        PerangkatDAO pd;
        ConnectionUtils c;

        private void loadTablePerangkat()
        {
            c = new ConnectionUtils();

            using (SQLiteConnection cc = new SQLiteConnection(c.getConnection()))
            {
                try
                {
                    cc.Open();
                    cmd = cc.CreateCommand();
                    cmd.CommandText = @"SELECT nama_perangkat,nip,jabatan FROM perangkat ORDER BY nama_perangkat";
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, cc))
                    {
                        DataTable ddt = new DataTable();
                        da.Fill(ddt);
                        dataGrid.ItemsSource = ddt.AsDataView();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Bosku!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        bool edit = false;
        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            loadTablePerangkat();
        }
        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (edit)
                {
                    DataGrid dg = (DataGrid)sender;
                    DataRowView drw = dg.SelectedItem as DataRowView;
                    var comboBoxItem = cmb_jabatan.Items.OfType<ComboBoxItem>().FirstOrDefault(x => x.Content.ToString() == drw["jabatan"].ToString());
                    int index = cmb_jabatan.SelectedIndex = cmb_jabatan.Items.IndexOf(comboBoxItem);
                    if (drw != null)
                    {
                        txt_namaPerangkat.Text = drw["nama_perangkat"].ToString();
                        txt_Nip.Text = drw["nip"].ToString();
                        if (index > 0)
                        {
                            cmb_jabatan.SelectedIndex = index;
                            Properties.Settings.Default.tempJabatan = drw["jabatan"].ToString();
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            dataGrid.UnselectAll();
                            loadTablePerangkat();
                            MessageBox.Show(index.ToString(), "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Harap Klik Tombol Ubah Dulu Bosku", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Information);
                    dataGrid.UnselectAll();
                    loadTablePerangkat();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void emptyFields()
        {
            txt_namaPerangkat.Text = string.Empty;
            txt_Nip.Text = string.Empty;
            cmb_jabatan.SelectedIndex = 0;
        }

        private void btn_baru_Click(object sender, RoutedEventArgs e)
        {
            c = new ConnectionUtils();
            pd = new PerangkatDAO();
            string a = "- Pilih Jabatan -";
            if (txt_namaPerangkat.Text != string.Empty
                && cmb_jabatan.Text.ToString() != a)
            {
                if (!pd.CheckJabatan(cmb_jabatan.Text.ToString()))
                {
                    if (pd.insertData(txt_namaPerangkat.Text.ToString(), IntegerExtensions.ParseInt(txt_Nip.Text), cmb_jabatan.Text.ToString()))
                    {
                        MessageBox.Show("Data Perangkat Berhasil ditambahkan Bosku!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                        emptyFields();
                        loadTablePerangkat();
                    }
                }
            }
            else
            {
                MessageBox.Show("Nama Perangkat dan Jabatan Wajib di isi Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            c = new ConnectionUtils();
            pd = new PerangkatDAO();
            if (!edit)
            {
                edit = true;
                btn_hapus.IsEnabled = true;
                labelUbah.Content = "Simpan";
                iconUbah.Kind = MaterialDesignThemes.Wpf.PackIconKind.ContentSaveOutline;
            }
            else
            {
                edit = false;
                btn_hapus.IsEnabled = false;
                labelUbah.Content = "Ubah";
                iconUbah.Kind = MaterialDesignThemes.Wpf.PackIconKind.SquareEditOutline;
                if (Properties.Settings.Default.tempJabatan != string.Empty)
                {
                    if (pd.updateData(txt_namaPerangkat.Text.ToString(), IntegerExtensions.ParseInt(txt_Nip.Text), cmb_jabatan.Text.ToString()))
                    {
                        MessageBox.Show("Berhasil Mengubah Data Bosku", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                Properties.Settings.Default.tempJabatan = "";
                Properties.Settings.Default.Save();
                emptyFields();
                loadTablePerangkat();
                pd.resetAutoIncrement();
            }
        }

        private void btn_hapus_Click(object sender, RoutedEventArgs e)
        {
            c = new ConnectionUtils();
            string a = "- Pilih Jabatan -";
            pd = new PerangkatDAO();
            MessageBoxResult re = MessageBox.Show("Apakah anda yakin ingin menghapus file ini bosku?", "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (re == MessageBoxResult.Yes)
            {
                if (cmb_jabatan.Text.ToString() != a)
                {
                    if (pd.CheckJabatan(cmb_jabatan.Text.ToString()))
                    {
                        if (pd.removeData(cmb_jabatan.Text.ToString()))
                        {
                            MessageBox.Show("Berhasil Menghapus Data Bosku", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                            emptyFields();
                            loadTablePerangkat();
                            pd.resetAutoIncrement();
                            Properties.Settings.Default.tempJabatan = "";
                            Properties.Settings.Default.Save();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data Tidak Ditemukan Bosku", "Gagal Menghapus", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
