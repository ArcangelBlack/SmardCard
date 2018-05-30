using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using SmartCardWebApplication.Models;
using SmartCardWebApplication.Models.ViewModel;

namespace SmartCardWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            CardViewModel cardViewModel = new CardViewModel();

            ReadSmartCard(cardViewModel);

            return View(cardViewModel);
        }

        private void ReadSmartCard(CardViewModel vM )
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


                            vM.FriendlyName = cert.FriendlyName;
                            vM.SerialNumber = cert.SerialNumber;
                            vM.Issuer = cert.Issuer;




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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AppleCat()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}