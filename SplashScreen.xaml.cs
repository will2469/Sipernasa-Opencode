using System;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using SipernasaDemoLicense;
using SipernasaLicensing;
using System.Reflection;

namespace Sipernasa
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        byte[] _certPubicKeyData;
        int progress = 0;
        DispatcherTimer dT = new DispatcherTimer();
        public SplashScreen()
        {
            InitializeComponent();
            //Initialize variables with default values
            SipernasaLicense _lic = null;
            string _msg = string.Empty;
            LicenseStatus _status = LicenseStatus.UNDEFINED;

            //Read public key from assembly
            Assembly _assembly = Assembly.GetExecutingAssembly();
            using (MemoryStream _mem = new MemoryStream())
            {
                _assembly.GetManifestResourceStream("Sipernasa.LicenseVerify.cer").CopyTo(_mem);

                _certPubicKeyData = _mem.ToArray();
            }

            //Check if the XML license file exists
            if (File.Exists("license.lic"))
            {
                _lic = (SipernasaLicense)LicenseHandler.ParseLicenseFromBASE64String(
                    typeof(SipernasaLicense),
                    File.ReadAllText("license.lic"),
                    _certPubicKeyData,
                    out _status,
                    out _msg);
            }
            else
            {
                _status = LicenseStatus.INVALID;
                _msg = "Your copy of this application is not activated";
            }

            switch (_status)
            {
                case LicenseStatus.VALID:

                    //TODO: If license is valid, you can do extra checking here
                    //TODO: E.g., check license expiry date if you have added expiry date property to your license entity
                    //TODO: Also, you can set feature switch here based on the different properties you added to your license entity 

                    //Here for demo, just show the license information and RETURN without additional checking
                    Pengaturan p = new Pengaturan();
                    p.liControl.ShowLicenseInfo(_lic);
            initDbFile();
            dT.Tick += new EventHandler(dT_Tick);
            dT.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dT.Start();

                    return;

                default:
                    //for the other status of license file, show the warning message
                    //and also popup the activation form for user to activate your application
                    MessageBox.Show(_msg, string.Empty, MessageBoxButton.OK, MessageBoxImage.Warning);

                    ActivateSipernasa frm = new ActivateSipernasa();
                    frm.CertificatePublicKeyData = _certPubicKeyData;
                    frm.Show();

                    break;
            }
        }
        public void SplashScreen_Load(object sender, EventArgs e)
        {
            initDbFile();
        }
        private void dT_Tick(object sender, EventArgs e)
        {
            if (loadingProgress.Value < 100)
            {
                progress += 5;
                loadingProgress.Value = progress;
            }
            else
            {
                dT.Stop();
                LoginWindow lw = new LoginWindow();
                lw.Show();
                this.Hide();
            }
        }
        ConnectionUtils c;
        public void initDbFile()
        {
            c = new ConnectionUtils();
            if (!c.IsDBFileAvailable("sipernasa.db"))
            {
                if (c.CreateDBFile("sipernasa.db"))
                {
                    System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
                    System.Windows.Application.Current.Shutdown();
                }
            }
            else
            {
                if (!c.checkLoginTable())
                {
                    if (c.createLoginInfo())
                    {
                        Console.WriteLine("table pengguna telah dibuat bosku");
                    }
                }
            }
        }
    }
}
