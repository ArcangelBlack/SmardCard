using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.SmardCardLibraryClass;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<X509Certificate2> certs = BaseSmartCardCryptoProvider.GetCertificates();

            ReadSmartCard();
            Console.WriteLine("------------------------------ReadSmartCard -----------------------------------");

            EnumCertificates(StoreName.My, StoreLocation.CurrentUser);
            //Console.WriteLine("------------------------------EnumCertificates -----------------------------------");

            //foreach (var x509Certificate2 in certs)
            //{
            //    Console.WriteLine(x509Certificate2);
            //}

            //var manager = SmartcardManager.GetManager();

            Console.ReadLine();
        }

        //Utilizar en ODP
        public static void ReadSmartCard()
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
                            //rsa.SignData(); // to confirm presence of private key - to finally authenticate
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Fallo");
                    continue;
                }
            }
        }


        #region Other

        public static void PrintCertificateInfo(X509Certificate2 certificate)
        {
            Console.WriteLine("Name: {0}", certificate.FriendlyName);
            Console.WriteLine("Issuer: {0}", certificate.IssuerName.Name);
            Console.WriteLine("Subject: {0}", certificate.SubjectName.Name);
            Console.WriteLine("Version: {0}", certificate.Version);
            Console.WriteLine("Valid from: {0}", certificate.NotBefore);
            Console.WriteLine("Valid until: {0}", certificate.NotAfter);
            Console.WriteLine("Serial number: {0}", certificate.SerialNumber);
            Console.WriteLine("Signature Algorithm: {0}", certificate.SignatureAlgorithm.FriendlyName);
            Console.WriteLine("Thumbprint: {0}", certificate.Thumbprint);
            Console.WriteLine();
        }

        public static void EnumCertificates(StoreName name, StoreLocation location)
        {
            X509Store store = new X509Store(name, location);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 certificate in store.Certificates)
                {
                    PrintCertificateInfo(certificate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                store.Close();
            }
        }

        public static void EnumCertificates(string name, StoreLocation location)
        {
            X509Store store = new X509Store(name, location);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 certificate in store.Certificates)
                {
                    PrintCertificateInfo(certificate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                store.Close();
            }
        }

        #endregion
    }
}
