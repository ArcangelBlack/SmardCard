using System;
using System.Collections.Generic;
using System.IO;
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
            CardViewModel cardViewModel = new CardViewModel();

            ReadSmartCard(cardViewModel);

            return View(cardViewModel);
        }

        private void ReadSmartCard(CardViewModel vM)
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
                    reader.SelectPages("1"+"-"+reader.NumberOfPages);

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