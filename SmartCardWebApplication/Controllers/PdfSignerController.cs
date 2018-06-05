using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SmartCardWebApplication.Models;
using SmartCardWebApplication.Models.ViewModel;

namespace SmartCardWebApplication.Controllers
{
    public class PdfSignerController : Controller
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
                        }
                    }
                }
                catch (Exception)
                {


                    Console.WriteLine("Fallo");
                    continue;
                }
            }

            vM.FriendlyName = "NO Existen certificados";
        }

        [System.Web.Http.HttpPost]
        public ActionResult Sign(HttpPostedFileBase file, CardViewModel cvm)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
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

        /// <summary>
        /// The POST method handler.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        //public HttpResponseMessage SignPost(HttpPostedFileBase file)
        public ActionResult SignPost(HttpPostedFileBase file)
        {
            //var l_task = this.Request.Content.ReadAsStreamAsync();
            //l_task.Wait();
            var l_requestStream = file.InputStream;


            var l_filename = System.IO.Path.GetTempFileName();

            var l_issuerName = "CN = PREPRODUCCIO EC - SectorPublic, OU = Serveis Públics de Certificació, O = CONSORCI ADMINISTRACIO OBERTA DE CATALUNYA, C = ES";

            //PdfDocumentSigner.SignDocumentStream(l_requestStream, l_filename, l_issuerName);

            //var l_response = new HttpResponseMessage(HttpStatusCode.OK);
            //try
            //{
            //    l_response.Content = this.TemporaryFile(l_filename);
            //    l_response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            //    l_response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            //    {
            //        FileName = "SignedDocument.pdf"
            //    };
            //}
            //catch (Exception l_ex)
            //{
            //    // log your exception details here
            //    l_response =
            //        new HttpResponseMessage(HttpStatusCode.InternalServerError)
            //        {
            //            Content = new StringContent(l_ex.Message)
            //        };
            //}

            //return l_response;


            //var l_bytes = System.IO.File.ReadAllBytes(l_filename);
            //System.IO.File.Delete(l_filename);
            //var ms =  new StreamContent(new MemoryStream(l_bytes));

            //HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            //httpResponseMessage.Content = this.TemporaryFile(l_filename); 
            //httpResponseMessage.Content.Headers.Add("x-filename", "SignedDocument.pdf");
            //httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            //httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            //httpResponseMessage.Content.Headers.ContentDisposition.FileName = "SignedDocument.pdf";
            //httpResponseMessage.StatusCode = HttpStatusCode.OK;
            //return httpResponseMessage;
            Response.AppendHeader("Content-Disposition", "inline; filename=hola.pdf");
            var l_bytes = System.IO.File.ReadAllBytes(l_filename);
            System.IO.File.Delete(l_filename);
            var memoryStream = new MemoryStream(l_bytes);

            return File(memoryStream.ToArray(), "application/pdf");
        }

        /// <summary>
        /// Reads the contents of the temporary signed file into a HttpContent instance, and deletes the file.
        /// </summary>
        /// <param name = "fileName" >
        /// The file name.
        /// </param>
        /// <returns>
        /// The<see cref="HttpContent"/>.
        /// </returns>
        private HttpContent TemporaryFile(string fileName)
        {
            var l_bytes = System.IO.File.ReadAllBytes(fileName);
            System.IO.File.Delete(fileName);
            return new StreamContent(new MemoryStream(l_bytes));
        }
    }
}