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
using System.Windows.Threading;
using System.IO;
using System.Windows.Media.Animation;
using SipernasaDemoLicense;
using System.Data.SQLite;
using SipernasaLicensing;
using SipernasaActivationControl;
using System.Reflection;

namespace Sipernasa
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        DispatcherTimer dT = new DispatcherTimer();
        public DashboardWindow()
        {
            initTablePerangkat();
            initTableSppd();
            InitializeComponent();
            DateTime dt;
            dt = DateTime.Now;
            currentDay.Content = dt.DayOfWeek.ToString();
            currentDate.Content = dt.Date.ToShortDateString();
            currentTime.Content = dt.ToLongTimeString();
            dT.Tick += new EventHandler(dT_Tick);
            dT.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dT.Start();
        }
        private void DashboardWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        private void dT_Tick(object sender, EventArgs e)
        {
            DateTime dt;
            dt = DateTime.Now;
            currentTime.Content = dt.ToLongTimeString();
            dT.Start();
        }
        private void ListMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListMenu.SelectedIndex;
            MoveCursorIndex(index);
            switch (index)
            {
                case 0:
                    try
                    {
                        SwitcherPane.Children.Clear();
                        currentMenu.Content = "DashBoard";
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                case 1:
                    try
                    {
                        SwitcherPane.Children.Clear();
                        try
                        {
                            SwitcherPane.Children.Add(new Perangkat());
                            currentMenu.Content = "Daftar Perangkat";
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                case 2:
                    try
                    {
                        if (pd.checkPerangkat())
                        {
                            try
                            {
                                SwitcherPane.Children.Clear();
                                try
                                {
                                    SwitcherPane.Children.Add(new SPPD());
                                    currentMenu.Content = "Perjalanan Dinas";
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Harap Masukkan setidaknya satu informasi perangkat dahulu bosku!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            ListMenu.SelectedIndex = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                case 3:
                    try
                    {
                        SwitcherPane.Children.Clear();
                        try
                        {
                            SwitcherPane.Children.Add(new Cetak());
                            currentMenu.Content = "Cetak";
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                case 4:
                    try
                    {
                        SwitcherPane.Children.Clear();
                        try
                        {
                            SwitcherPane.Children.Add(new Pengaturan());
                            currentMenu.Content = "Pengaturan";
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                default:
                    break;
            }
        }

        private void MoveCursorIndex(int index)
        {
            TransEffect.OnApplyTemplate();
            movingCursor.Margin = new Thickness(0, (0 + ((300 / 250) * 50 * index)), 0, (300 - ((300 / 250) * 50 * index)));
        }

        private void forcedCLose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        bool flag = false;
        private void BurgerMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!flag)
            {
                flag = true;
                Storyboard x = (this.FindResource("openMenuBar") as Storyboard);
                x.Begin();
                changeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.RestoreClock;
            }
            else
            {
                flag = false;
                Storyboard y = (this.FindResource("closeMenuBar") as Storyboard);
                y.Begin();
                changeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.HamburgerMenu;
            }
        }

        private void closeMe_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        PerangkatDAO pd;
        ConnectionUtils c;
        private void initTablePerangkat()
        {
            c = new ConnectionUtils();
            pd = new PerangkatDAO();
            if (!pd.CheckDBPerangkat())
            {
                if (pd.CreateTablePerangkat())
                {
                    Console.WriteLine("table pengguna telah dibuat bosku");
                }
            }
        }
        sppdDAO sd;
        private void initTableSppd()
        {
            c = new ConnectionUtils();
            sd = new sppdDAO();
            if (!sd.CheckDBsppd())
            {
                if (sd.CreateTableSPPD())
                {
                    Console.WriteLine("table sppd telah dibuat bosku");
                }
            }
        }
    }
}