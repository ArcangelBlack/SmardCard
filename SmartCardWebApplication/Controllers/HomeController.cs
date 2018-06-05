using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SmartCardWebApplication.Models.ViewModel;

namespace SmartCardWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var vm = new CardsViewmodel();

            //
            var result1 = GetCertificate();
            if (result1 != null)
            {
                foreach (var r in result1)
                {
                    var item = new CardViewModel();
                    item.FriendlyName = r.FriendlyName;
                    item.SerialNumber = r.SerialNumber;
                    item.Issuer = r.Issuer;

                    vm.Cards.Add(item);
                }

            }
            else
            {
                vm.Cards.Add(new CardViewModel { FriendlyName = "No encontro en GetCertificate"});
            }

            vm.Cards.Add(new CardViewModel { FriendlyName = "Siguiente >" });

            var result = ReadSmartCards4().ToList();
            if (result.Any())
            {
                foreach(var c in result)
                {
                    vm.Cards.Add(c);
                }
            }
            else{
                vm.Cards.Add(new CardViewModel { FriendlyName = "No encontro en ReadSmartCards" });
            }

            return View(vm);
        }

        private IEnumerable<CardViewModel> ReadSmartCards()
        {
            var result = new List<CardViewModel>();

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

                            var item = new CardViewModel();

                            item.FriendlyName = cert.FriendlyName;
                            item.SerialNumber = cert.SerialNumber;
                            item.Issuer = cert.Issuer;

                            result.Add(item);
                            //rsa.SignData(); // to confirm presence of private key - to finally authenticate
                        }
                    }
                }
                catch (Exception ex)
                {
                    var item = new CardViewModel();
                    item.FriendlyName = ex.StackTrace;
                    item.SerialNumber = "No acceso";
                    item.Issuer = "No acceso";
                    result.Add(item);

                    Console.WriteLine("Fallo");
                    continue;
                }
            }

            myStore.Close();
            return result;
            //vM.FriendlyName = "NO Existen certificados";
        }

        private IEnumerable<CardViewModel> ReadSmartCards2()
        {
            var result = new List<CardViewModel>();

            var smartCardCerts = new List<X509Certificate2>();
            var myStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);

            myStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            foreach (X509Certificate2 cert in myStore.Certificates)
            {
                try
                {
                    //if (!cert.HasPrivateKey) continue;// not smartcard for sure

                    //if (string.IsNullOrEmpty(cert.FriendlyName))
                    //{
                    //    continue;
                    //}
                    //if (cert.PrivateKey is RSACryptoServiceProvider)
                    //{
                    //    var rsa = cert.PrivateKey as RSACryptoServiceProvider;
                    //    if (rsa == null) continue; // not smart card cert again
                    //    if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
                    //    {
                            // inspect rsa.CspKeyContainerInfo.KeyContainerName Property
                            // or rsa.CspKeyContainerInfo.ProviderName (your smartcard provider, such as 
                            // "Schlumberger Cryptographic Service Provider" for Schlumberger Cryptoflex 4K
                            // card, etc
                            var item = new CardViewModel();

                            item.FriendlyName = cert.FriendlyName;
                            item.SerialNumber = cert.SerialNumber;
                            item.Issuer = cert.Issuer;

                            result.Add(item);
                            //rsa.SignData(); // to confirm presence of private key - to finally authenticate
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    var item = new CardViewModel();
                    item.FriendlyName = ex.StackTrace;
                    item.SerialNumber = "No acceso";
                    item.Issuer = "No acceso";
                    result.Add(item);

                    Console.WriteLine("Fallo");
                    continue;
                }
            }

            myStore.Close();
            return result;
            //vM.FriendlyName = "NO Existen certificados";
        }

        private IEnumerable<CardViewModel> ReadSmartCards3()
        {
            var result = new List<CardViewModel>();

            X509Store userCaStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            try
            {
                userCaStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificatesInStore = userCaStore.Certificates;
                //X509Certificate2Collection findResult = certificatesInStore.Find(X509FindType.FindBySubjectName, "localtestclientcert", true);
                foreach (X509Certificate2 cert in certificatesInStore)
                {
                    X509Chain chain = new X509Chain();
                    X509ChainPolicy chainPolicy = new X509ChainPolicy()
                    {
                        RevocationMode = X509RevocationMode.Online,
                        RevocationFlag = X509RevocationFlag.EntireChain
                    };
                    chain.ChainPolicy = chainPolicy;
                    if (!chain.Build(cert))
                    {
                        foreach (X509ChainElement chainElement in chain.ChainElements)
                        {
                            foreach (X509ChainStatus chainStatus in chainElement.ChainElementStatus)
                            {
                                Console.WriteLine(chainStatus.StatusInformation);
                            }
                        }
                    }
                }
            }
            finally
            {
                userCaStore.Close();
            }
            return result;
            //vM.FriendlyName = "NO Existen certificados";
        }

        private IEnumerable<CardViewModel> ReadSmartCards4()
        {
            var result = new List<CardViewModel>();

            X509Store userCaStore = new X509Store(StoreName.My);

            try
            {
                userCaStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2Collection certificatesInStore = userCaStore.Certificates;
                //X509Certificate2Collection findResult = certificatesInStore.Find(X509FindType.FindBySubjectName, "localtestclientcert", true);
                foreach (X509Certificate2 cert in certificatesInStore)
                {
                    X509Chain chain = new X509Chain();
                    X509ChainPolicy chainPolicy = new X509ChainPolicy()
                    {
                        RevocationMode = X509RevocationMode.Online,
                        RevocationFlag = X509RevocationFlag.EntireChain
                    };
                    chain.ChainPolicy = chainPolicy;
                    if (!chain.Build(cert))
                    {
                        foreach (X509ChainElement chainElement in chain.ChainElements)
                        {
                            foreach (X509ChainStatus chainStatus in chainElement.ChainElementStatus)
                            {
                                Console.WriteLine(chainStatus.StatusInformation);
                            }
                        }
                    }
                }
            }
            finally
            {
                userCaStore.Close();
            }
            return result;
        }

        /// <summary>
        /// Gets the certificate from the current users store.
        /// </summary>
        /// <returns>
        /// The <see cref="X509Certificate2"/>.
        /// </returns>
        private static X509Certificate2Collection GetCertificate()
        {
            // Open personal certificate store for the logged-in user
            var l_certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            // var l_issuerName = "Laia Pons Gimeno";//ConfigurationManager.AppSettings["CertificateIssuerName"];


            l_certStore.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            // Get all certificates in store
            X509Certificate2Collection l_certs = l_certStore.Certificates;
            l_certStore.Close();

            // A sequence of filtering operations follows to end up with the certificate we should use.
            // Could use enumeration instead.

            // 1. Filter out expired certificates
            l_certs = l_certs.Find(X509FindType.FindByTimeValid, DateTime.Now, true);

            // 2. Filter on issuer.
            // NOTE! Issuer name may change.
            //l_certs = l_certs.Find(X509FindType.FindByIssuerName, l_issuerName, true);

            // 3. Filter on key usage. We usually want NonRepudiation for document signatures
            l_certs = l_certs.Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.NonRepudiation, true);

            // If only one user's certificates are in the store,
            // we should be left with only one certificate at this point.
            //if (l_certs.Count > 1)
            //{
            //    // If there are still more than one, let the user choose
            //    throw new Exception(
            //        $"There are more than one valid, non-expired, non-repudiation certificates issued by  available in the certificate store");
            //}

            // At this point, there is 1 or 0 certificates in the collection
            return l_certs;//.Count == 0 ? null : l_certs[0];
        }

        //controller para poder seleccionar archivo y poder firmarlo

        [HttpPost]
        public ActionResult About(HttpPostedFileBase file, CardViewModel cvm)
        {


            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Documents"), Path.GetFileName(file.FileName));

                    file.SaveAs(path);

                    X509Certificate2 certificado = GetCert();


                    SignHashed(certificado.FriendlyName, "mariano", true, path);

                    ViewBag.Message = "File uploaded successfully";

                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            ViewBag.Message = "Selecciona el fichero para poder firmar.";



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

        public void SignHashed(string SigReason, string SigContact, bool visible, string path)
        {
            string input = path;
            string output = "C:\\Users\\ajaouhari\\Documents\\Proyectos\\ODP\\SmartCardWebApplication\\Documents\\Document-Signed.pdf";


            using (MemoryStream xd = new MemoryStream())
            {


                using (PdfReader reader = new PdfReader(input))
                //create PdfStamper object to write to get the pages from reader 
                using (PdfStamper stamper = new PdfStamper(reader, new FileStream(output, FileMode.Create)))
                {
                    //select two pages from the original document
                    reader.SelectPages("1" + "-" + reader.NumberOfPages);

                    //gettins the page size in order to substract from the iTextSharp coordinates
                    var pageSize = reader.GetPageSize(reader.NumberOfPages);

                    // PdfContentByte from stamper to add content to the pages over the original content
                    PdfContentByte pbover = stamper.GetOverContent(reader.NumberOfPages);

                    //add content to the page using ColumnText
                    Font font = new Font();
                    font.Size = 10;

                    //setting up the X and Y coordinates of the document
                    int x = 150;
                    int y = 200;

                    y = (int)(pageSize.Height - y);

                    ColumnText.ShowTextAligned(pbover, Element.ALIGN_CENTER, new Phrase(SigReason, font), x, y, 0);
                }
            }
        }

        #region Cert

        public X509Certificate2 GetCert()
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

                            return cert;
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Fallo");
                    continue;
                }
            }

            return null;
        }

        #endregion

        #region add digital signature to PDF document


        #endregion
    }
}