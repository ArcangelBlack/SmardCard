using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace SmardCardActiveX
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("F5D0799C-CBF2-466a-B293-F68C6236C5B9")]
    [ProgId("SmardCardActiveX.UserControl1")]
    public partial class UserControl1: UserControl
    {
        public UserControl1()
        {
            InitializeComponent();

            this.listBox1.DataSource = this.ReadSmartCard();
        }

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Get the currently selected item in the ListBox.
            string curItem = listBox1.SelectedItem.ToString();

            // Find the string in ListBox2.
            int index = this.listBox1.FindString(curItem);
            // If the item was not found in ListBox 2 display a message box, otherwise select it in ListBox2.
            if (index == -1)
                MessageBox.Show("Item is not available in ListBox2");
            else
                this.listBox1.SetSelected(index, true);
        }

        public X509Certificate2 GetCurrentCertificate()
        {
            var result = new X509Certificate2();

            string text = this.listBox1.GetItemText(listBox1.SelectedItem);

            return result;
        }

        public IEnumerable<X509Certificate2> ReadSmartCard()
        {
            var smartCardCerts = new List<X509Certificate2>();
            var myStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            myStore.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in myStore.Certificates)
            {
                try
                {
                    if (!cert.HasPrivateKey) continue;// not smartcard for sure

                    if (string.IsNullOrEmpty(cert.FriendlyName))
                    {
                        continue;
                    }
                    if (cert.PrivateKey is RSACryptoServiceProvider)
                    {
                        var rsa = cert.PrivateKey as RSACryptoServiceProvider;
                        if (rsa == null) continue; // not smart card cert again
                        if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
                        {
                            // inspect rsa.CspKeyContainerInfo.KeyContainerName Property
                            // or rsa.CspKeyContainerInfo.ProviderName (your smartcard provider, such as 
                            // "Schlumberger Cryptographic Service Provider" for Schlumberger Cryptoflex 4K
                            // card, etc
                            Console.WriteLine(cert.FriendlyName);
                            Console.WriteLine(cert.SerialNumber);
                            Console.WriteLine(cert.Issuer);

                            smartCardCerts.Add(cert);
                            //rsa.SignData(); // to confirm presence of private key - to finally authenticate
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            myStore.Close();

            return smartCardCerts;
        }

        [ComVisible(true)]
        public String SayHello()

        {

            return "Hello World!";

        }
    }
}
