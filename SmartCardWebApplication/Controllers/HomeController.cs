using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
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
                    string path = Path.Combine(Server.MapPath("~/Documents"),
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);

                    SignHashed("mariano", "mariano", true, path);

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

        public static void SignHashed(string SigReason, string SigContact, bool visible, string path)
        {

            using (MemoryStream pdfData = new MemoryStream())
            {
                //string template = @"C:\Documents and Settings\mshyavitz\My Documents\Visual Studio 2008\Projects\WFMO.RADS.Web\print\sf52.pdf";
                using (PdfReader pdfReader = new PdfReader(path))
                {
                    using (PdfStamper st = PdfStamper.CreateSignature(pdfReader, pdfData, '\0', null, true))
                    {

                        PdfSignatureAppearance sap = st.SignatureAppearance;

                        sap.Reason = SigReason;
                        sap.Contact = SigContact;

                        if (visible)
                            sap.SetVisibleSignature(new iTextSharp.text.Rectangle(100, 100, 250, 150), 1, null);

                        st.FormFlattening = true;
                        st.Writer.CloseStream = false;
                        st.Close();

                        //            }
                        //using (PdfStamper pdfStamper = new PdfStamper(pdfReader, pdfData))
                        //{
                        //    AcroFields pdfFormFields = pdfStamper.AcroFields;

                        //    // set fields here. 

                        //    pdfStamper.FormFlattening = true;
                        //    pdfStamper.Writer.CloseStream = false;
                        //    pdfStamper.Close();
                        //}
                    }
                }




                //    PdfReader reader = new PdfReader(path);
                //    //Activate MultiSignatures
                //    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                //    {
                //        using (MemoryStream ms = new MemoryStream())
                //        {
                //            using (PdfStamper st = PdfStamper.CreateSignature(reader, ms, '\0', null, true))
                //            {

                //                PdfSignatureAppearance sap = st.SignatureAppearance;

                //                sap.Reason = SigReason;
                //                sap.Contact = SigContact;

                //                if (visible)
                //                    sap.SetVisibleSignature(new iTextSharp.text.Rectangle(100, 100, 250, 150), 1, null);

                //                st.Close();
                //            }
                //        }
                //    }

                //}
            }
        }
    }
}