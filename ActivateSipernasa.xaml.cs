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
using SipernasaActivationControl;
using SipernasaDemoLicense;
using SipernasaLicensing;
using System.IO;

namespace Sipernasa
{
    /// <summary>
    /// Interaction logic for ActivateSipernasa.xaml
    /// </summary>
    public partial class ActivateSipernasa : Window
    {
        public byte[] CertificatePublicKeyData { private get; set; }
        public ActivateSipernasa()
        {
			try
			{
            licActCtrl.Content = new LicenseActiveControl();
			}
		catch(Exception ex)
			{
                Console.WriteLine(ex.Message);
			}
            InitializeComponent();
            //Assign the application information values to the license control
            licActCtrl.AppName = "Sipernasa";
            licActCtrl.LicenseObjectType = typeof(SipernasaLicense);
            licActCtrl.CertificatePublicKeyData = this.CertificatePublicKeyData;
            //Display the device unique ID
            licActCtrl.ShowUID();
        }

        private void btn_activate_Click(object sender, RoutedEventArgs e)
        {
            if (licActCtrl.ValidateLicense())
            {
                //If license if valid, save the license string into a local file
                File.WriteAllText(System.IO.Path.Combine((System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)), "license.lic"), licActCtrl.LicenseBASE64String);

                MessageBox.Show("License accepted, the application will be close. Please restart it later", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to cancel?", "Peringatan", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
