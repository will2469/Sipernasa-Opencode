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
using System.Windows.Shapes;
using System.IO;
using System.Data.SQLite;

namespace Sipernasa
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            init();
            loadRememberMe();
        }
        private void LoginWindow_Load(object sender, RoutedEventArgs e)
        {
            init();
        }
        ConnectionUtils c;
        private void init()
        {
            c = new ConnectionUtils();
            LoginDAO ld = new LoginDAO();
            if (!c.checkLoginTable())
            {
                ld.createTableLogin("admin", "admin");
            }
        }
        private void forceClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            LoginDAO ld = new LoginDAO();
            if ((txt_namaPengguna.Text == "") && (txt_password.Password == "") || (txt_namaPengguna.Text == "") || (txt_password.Password == ""))
            {
                MessageBox.Show("Nama Pengguna atau Password masih kosong bosku", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                try
                {
                    if (ld.Login(txt_namaPengguna.Text.ToString(), txt_password.Password.ToString()))
                    {
                        saveRememberMe();
                        DashboardWindow dw = new DashboardWindow();
                        Hide();
                        dw.Show();
                    }
                    else
                    {
                        MessageBox.Show("Nama Pengguna atau Kata Sandi Tidak Sesuai Bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        txt_namaPengguna.Clear();
                        txt_password.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kesalahan :" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        void saveRememberMe()
        {
            if (rememberMe.IsChecked == true)
            {
                Properties.Settings.Default.Username = txt_namaPengguna.Text.ToString();
                Properties.Settings.Default.Password = txt_password.Password.ToString();
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.Username = "";
                Properties.Settings.Default.Password = "";
                Properties.Settings.Default.Save();
            }
        }
        void loadRememberMe()
        {
            if (Properties.Settings.Default.Username != string.Empty)
            {
                txt_namaPengguna.Text = Properties.Settings.Default.Username;
                txt_password.Password = Properties.Settings.Default.Password;
                rememberMe.IsChecked = true;
            }
        }
    }
}
