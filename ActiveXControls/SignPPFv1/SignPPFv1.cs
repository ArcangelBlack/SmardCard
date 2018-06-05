using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SignPPFv1
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("F5D0799C-CBF2-466a-B293-F68C6236C5B9")]
    [ProgId("SignPPFv1.SignPPFv1")]
    [ComDefaultInterface(typeof(ISignPPFv1))]
    public class SignPPFv1 : UserControl, ISignPPFv1, IObjectSafety
    {


        X509Store myStore2;

        #region ISignPPFv1 Members

        public string GetText()
        {

            //SignXML2();

            //capicom();

            return "Hello ActiveX World!";
        }

        public string Signar2(string xmlIn, string node, bool p1, bool p2)
        {
            string sRes;
            string sResXml;
            sResXml = "";
            sRes = "";
            if ((xmlIn == null) || (xmlIn == ""))
            {
                sRes = "KO - No hi document a signar";
            }
            else
            {
                sResXml = SignXML(xmlIn, ref sRes);
            }
            if ((sRes == null) || (sRes == ""))
            {
                sRes = sResXml;
            }
            return sRes;
        }

        public string Signar2v1(string xmlIn)
        {
            string sRes;
            string sResXml;
            sResXml = "";
            sRes = "";
            if ((xmlIn == null) || (xmlIn == ""))
            {
                sRes = "KO - No hi document a signar";
            }
            else
            {
                sResXml = SignXML(xmlIn, ref sRes);
            }
            if ((sRes == null) || (sRes == ""))
            {
                sRes = sResXml;
            }
            return sRes;
        }

        public RSACryptoServiceProvider RecuperaProviderRSA_old(ref string vError)
        {
            RSACryptoServiceProvider AuxRSA;
            AuxRSA = null;
            bool bProvider;
            bProvider= false;
            int caseSwitch = 0;
            string sProviderCSP;
            sProviderCSP = "SafeSign CSP Version 1.0";

            while (bProvider==false)
            {
             try
                {
                    CspParameters csp = new CspParameters(1, sProviderCSP);
                    csp.Flags = CspProviderFlags.UseDefaultKeyContainer;
                    RSACryptoServiceProvider key = new RSACryptoServiceProvider(csp);                    
                    //RSACryptoServiceProvider key = new RSACryptoServiceProvider(csp);

                    AuxRSA = key; 
                    bProvider=true;
                }
                catch (Exception ex)
                {
                    caseSwitch = caseSwitch + 1;
                    switch (caseSwitch)
                    {
                        case 1:
                            sProviderCSP = "SafeSign Standard Cryptographic Service Provider";
                            break;
                        case 2:
                            sProviderCSP = "Gemalto Classic Card CSP";
                            break;
                        case 3:
                            sProviderCSP = "Bit4id Universal Middleware Provider";
                            break;
                        case 4:
                            sProviderCSP = "SafeNet RSA CSP";
                            break;
                        case 5:
                            bProvider = true;
                            vError ="KO .- RSA no trobat " + ex.Message.ToString();
                            break; 
                        default :
                            sProviderCSP = "SafeSign CSP Version 1.0";
                            break;
                    }
                }
            }

            return AuxRSA;
        }

        public RSACryptoServiceProvider RecuperaProviderRSA(string sProviderCSP, ref string vError)
        {
            RSACryptoServiceProvider AuxRSA;
            AuxRSA = null;
            bool bProvider;
            bProvider = false;
           // int caseSwitch; = 0;
            //string sProviderCSP;
            //sProviderCSP = "SafeSign CSP Version 1.0";

            while (bProvider == false)
            {
                try
                {
                    CspParameters csp = new CspParameters(1, sProviderCSP);
                    csp.Flags = CspProviderFlags.UseDefaultKeyContainer;
                    RSACryptoServiceProvider key = new RSACryptoServiceProvider(csp);
                    //RSACryptoServiceProvider key = new RSACryptoServiceProvider(csp);

                    AuxRSA = key;
                    bProvider = true;
                }
                catch (Exception ex)
                {
                    //caseSwitch = caseSwitch + 1;
                    //switch (caseSwitch)
                    //{
                    //    case 1:
                    //        sProviderCSP = "SafeSign Standard Cryptographic Service Provider";
                    //        break;
                    //    case 2:
                    //        sProviderCSP = "Gemalto Classic Card CSP";
                    //        break;
                    //    case 3:
                    //        sProviderCSP = "Bit4id Universal Middleware Provider";
                    //        break;
                    //    case 4:
                    //        sProviderCSP = "SafeNet RSA CSP";
                    //        break;
                    //    case 5:
                    //        bProvider = true;
                    //        vError = "KO .- RSA no trobat " + ex.Message.ToString();
                    //        break;
                    //    default:
                    //        sProviderCSP = "SafeSign CSP Version 1.0";
                    //        break;
                    //}
                }
            }

            return AuxRSA;
        }

        public string  SignXML(string xmlIn,ref string sError)
        {
            string XmlDSigOut;
            XmlDSigOut = "";
            //setup the document to sign 
            XmlDocument doc = new XmlDocument();
           
                MessageBox.Show("Load xml");
                doc.LoadXml(xmlIn);
                SignedXml signer = new SignedXml(doc);

                MessageBox.Show("Load xml - OK ");
                sError = "";
           

            if (sError == "")
            {
                string sProviderName;
                sProviderName = "";
                X509Certificate2 cert;
                cert = BuscaCert(ref sProviderName);

                RSACryptoServiceProvider key;
                key = RecuperaProviderRSA(sProviderName,ref sError);
                if (sError != "")
                {
                    return "";
                }
                if (key == null)
                {
                    sError = "KO .- Problemes al recuperar RSA";

                }
                else
                {
                    MessageBox.Show("BuscaCert ");

                    //X509Certificate2 cert;
                    //cert = BuscaCert();

                    MessageBox.Show("BuscaCert - OK ");
                    //csp.Flags = CspProviderFlags.UseDefaultKeyContainer;
                    //RSACryptoServiceProvider key = new RSACryptoServiceProvider(csp);

                    signer.KeyInfo = new KeyInfo();

                    KeyInfo keyInfo1 = new KeyInfo();
                    keyInfo1.AddClause(new KeyInfoX509Data(cert));
                    signer.KeyInfo = keyInfo1;

                    signer.KeyInfo.AddClause(new RSAKeyValue(key));
                    signer.SigningKey = key;
                    //The key must be set as the signing key, as well as placed in an RSAKeyValue clause.  The RSAKeyValue clause puts the public portion of the keypair into the signature itself, allowing anyone who retrieves the document to validate the signature, without having to know what key was used to sign it with.  The next step is to create a reference to the data being signed. 

                    //// create a reference to the root of the document 
                    Reference orderRef = new Reference("#data1");
                    orderRef.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                    signer.AddReference(orderRef);
                    //A reference with a URI that is the empty string refers to the entire containing document.  However, since this is going to be an enveloped signature, validating the entire document would result in an invalid signature, since the signature value itself will be a part of the document.  Therefore, we must add an XmlDsigEnvelopedSignatureTransform, which prevents the signature validator from looking at the actual signature itself when validating the document.  The last step is to compute the signature, and add it to the document: 


                    //// add transforms that only select the order items, type, and 
                    //// compute the signature, and add it to the document 
                    try
                    {
                        MessageBox.Show("ComputeSignature ");
                        signer.ComputeSignature();
                        MessageBox.Show("ComputeSignature - OK ");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ComputeSignature - Error " + ex.Message.ToString() );
                        sError = "KO .- " + ex.Message.ToString();
                        XmlDSigOut = "";
                    }

                    // Get the XML representation of the signature and save
                    // it to an XmlElement object.
                    XmlElement xmlDigitalSignature = signer.GetXml();

                    // Append the element to the XML document.
                    doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

                    // sHash = doc.GetElementsByTagName("DigestValue")(0).InnerText

                    if (doc.FirstChild is XmlDeclaration)
                    {
                        doc.RemoveChild(doc.FirstChild);
                    }

                    // Save the signed XML document to a file specified
                    // using the passed string.
                    XmlDSigOut = doc.DocumentElement.OuterXml.ToString();
                    //doc.Save("c:\\temp\\signedNet_21.xml");
                    doc = null;
                    signer = null;

                }


            }
            else
            {

            }

            MessageBox.Show("Return - " + XmlDSigOut);
            return XmlDSigOut;

        }

        public string SignXMLv1(string xmlIn, ref string sError)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNamespaceManager namespaces = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaces.AddNamespace("ns", "http://catsalut.sire.es/signeddata");

            xmlDoc.PreserveWhitespace = false;
            //xmlDoc.Load(new XmlTextReader("C:\\Development\\testheader.xml"));
            xmlDoc.LoadXml(xmlIn);

            SignedXml signedXml = new SignedXml(xmlDoc);

            //X509Certificate2 cert;
            //cert = BuscaCert();

            string sProviderName;
            sProviderName = "";
            X509Certificate2 cert;
            cert = BuscaCert(ref sProviderName);


            if (cert == null)
            {
                sError = "Error";

            }
            else
            {

                RSACryptoServiceProvider key;
                key = RecuperaProviderRSA(sProviderName,ref sError);

                signedXml.SigningKey = cert.PrivateKey;

                Reference reference = new Reference();
                reference.Uri = "#data1";

                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);

                XmlDsigC14NTransform c14n = new XmlDsigC14NTransform();
                reference.AddTransform(c14n);

                signedXml.AddReference(reference);

                KeyInfo keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(cert));
                signedXml.KeyInfo = keyInfo;

                signedXml.KeyInfo.AddClause(new RSAKeyValue(key));
                signedXml.SigningKey = key;
                
                signedXml.ComputeSignature();

                XmlNode parentNode = xmlDoc.SelectSingleNode("/ns:TopLevelNode", namespaces);
                parentNode.InsertAfter(xmlDoc.ImportNode(signedXml.GetXml(), true), parentNode.FirstChild);

                if (xmlDoc.FirstChild is XmlDeclaration)
                {
                    xmlDoc.RemoveChild(xmlDoc.FirstChild);
                }

                //XmlTextWriter xmltw = new XmlTextWriter("C:\\Development\\test2.xml", new UTF8Encoding(false));
                //xmlDoc.WriteTo(xmltw);
                //xmltw.Close();
            }
            return xmlDoc.DocumentElement.OuterXml.ToString();
        }

        public string BuscaNomCert()
        {
            X509Certificate2 AuxCert = null;
            string sRes = null;
            //Dim smartCardCerts = New List(Of X509Certificate2)()
            X509Store myStore2 = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            myStore2.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in myStore2.Certificates)
            {                
                if (!cert.HasPrivateKey)
                {
                    continue;
                }
                try
                {
                    RSACryptoServiceProvider rsa;
                    rsa = (RSACryptoServiceProvider)cert.PrivateKey;
                    if (rsa ==null )
                    {
                     continue;   
                    }
                    if ( rsa.CspKeyContainerInfo.HardwareDevice )
                    {
                        AuxCert = cert;
                        myStore2.Close();
                        break; 
                    }
                    
                    //if (IsCertificateValid(cert, ref sRes))
                    //{
                    //    // not smartcard for sure
                    //    RSACryptoServiceProvider rsa =  (RSACryptoServiceProvider)cert.PrivateKey;

                    //    if (rsa == null)
                    //    {
                    //        continue;
                    //    }
                    //    // not smart card cert again
                    //    if (rsa.CspKeyContainerInfo.HardwareDevice)
                    //    {
                    //        // sure - smartcard
                    //        // inspect rsa.CspKeyContainerInfo.KeyContainerName Property
                    //        // or rsa.CspKeyContainerInfo.ProviderName (your smartcard provider, such as 
                    //        // "Schlumberger Cryptographic Service Provider" for Schlumberger Cryptoflex 4K
                    //        // card, etc
                    //        //name = cert.SubjectName.Name
                    //        //cert.SerialNumber.ToString()
                    //        //cert.GetExpirationDateString()
                    //        AuxCert = cert;
                    //        myStore2.Close();
                    //        // to confirm presence of private key - to finally authenticate
                    //        //rsa.SignData
                    //        break; // TODO: might not be correct. Was : Exit For
                    //    }
                    //}

                }
                catch (Exception ex)
                {
                    sRes = ex.Message.ToString();
                }

            }
            //myStore2.Close()
            if (AuxCert == null)
            {
                sRes = "KO - Tarjeta Signatura no detectada. Certificat no trobat";
            }
            else
            {
                sRes = AuxCert.SubjectName.Name.ToString();
            }

            return sRes;

        }

        public X509Certificate2 BuscaCert(ref string sProviderName)
        {
            X509Certificate2 AuxCert = null;
            //string sRes = null;
            //Dim smartCardCerts = New List(Of X509Certificate2)()
            //X509Store myStore2 = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            myStore2 = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            myStore2.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in myStore2.Certificates)
            {
                if (!cert.HasPrivateKey)
                {
                    continue;
                }
                try
                {
                    RSACryptoServiceProvider rsa;
                    rsa = (RSACryptoServiceProvider)cert.PrivateKey;
                    if (rsa == null)
                    {
                        continue;
                    }
                    if (rsa.CspKeyContainerInfo.HardwareDevice)
                    {
                        AuxCert = cert;
                        sProviderName = rsa.CspKeyContainerInfo.ProviderName;
                        myStore2.Close();
                        break;
                    }

                    //if (IsCertificateValid(cert, ref sRes))
                    //{
                    //    // not smartcard for sure
                    //    RSACryptoServiceProvider rsa =  (RSACryptoServiceProvider)cert.PrivateKey;

                    //    if (rsa == null)
                    //    {
                    //        continue;
                    //    }
                    //    // not smart card cert again
                    //    if (rsa.CspKeyContainerInfo.HardwareDevice)
                    //    {
                    //        // sure - smartcard
                    //        // inspect rsa.CspKeyContainerInfo.KeyContainerName Property
                    //        // or rsa.CspKeyContainerInfo.ProviderName (your smartcard provider, such as 
                    //        // "Schlumberger Cryptographic Service Provider" for Schlumberger Cryptoflex 4K
                    //        // card, etc
                    //        //name = cert.SubjectName.Name
                    //        //cert.SerialNumber.ToString()
                    //        //cert.GetExpirationDateString()
                    //        AuxCert = cert;
                    //        myStore2.Close();
                    //        // to confirm presence of private key - to finally authenticate
                    //        //rsa.SignData
                    //        break; // TODO: might not be correct. Was : Exit For
                    //    }
                    //}

                }
                catch (Exception ex)
                {
                }

            }
            ////myStore2.Close()
            //if (AuxCert == null)
            //{
            //    sRes = "KO - Tarjeta Signatura no detectada. Certificat no trobat";
            //}
            //else
            //{
            //    sRes = AuxCert.SubjectName.Name.ToString();
            //}

            return AuxCert;

        }

        //public X509Certificate2 BuscaCert()
        //{
        //    X509Certificate2 AuxCert = null;
        //    string sRes;
        //    sRes = null;
        //    //Dim smartCardCerts = New List(Of X509Certificate2)()
        //    //X509Store myStore2 = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        //    myStore2 = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        //    myStore2.Open(OpenFlags.ReadOnly);
        //    foreach (X509Certificate2 cert in myStore2.Certificates)
        //    {
        //        if (!cert.HasPrivateKey)
        //        {
        //            continue;
        //        }
        //        try
        //        {
        //            RSACryptoServiceProvider rsa;
        //            rsa = (RSACryptoServiceProvider)cert.PrivateKey;
        //            if (rsa == null)
        //            {
        //                continue;
        //            }
        //            if (rsa.CspKeyContainerInfo.HardwareDevice)
        //            {
        //                AuxCert = cert;
        //                myStore2.Close();
        //                break;
        //            }

        //            //if (IsCertificateValid(cert, ref sRes))
        //            //{
        //            //    // not smartcard for sure
        //            //    RSACryptoServiceProvider rsa =  (RSACryptoServiceProvider)cert.PrivateKey;

        //            //    if (rsa == null)
        //            //    {
        //            //        continue;
        //            //    }
        //            //    // not smart card cert again
        //            //    if (rsa.CspKeyContainerInfo.HardwareDevice)
        //            //    {
        //            //        // sure - smartcard
        //            //        // inspect rsa.CspKeyContainerInfo.KeyContainerName Property
        //            //        // or rsa.CspKeyContainerInfo.ProviderName (your smartcard provider, such as 
        //            //        // "Schlumberger Cryptographic Service Provider" for Schlumberger Cryptoflex 4K
        //            //        // card, etc
        //            //        //name = cert.SubjectName.Name
        //            //        //cert.SerialNumber.ToString()
        //            //        //cert.GetExpirationDateString()
        //            //        AuxCert = cert;
        //            //        myStore2.Close();
        //            //        // to confirm presence of private key - to finally authenticate
        //            //        //rsa.SignData
        //            //        break; // TODO: might not be correct. Was : Exit For
        //            //    }
        //            //}

        //        }
        //        catch (Exception ex)
        //        {
        //            sRes = ex.Message.ToString(); 
        //        }

        //    }
        //    ////myStore2.Close()
        //    //if (AuxCert == null)
        //    //{
        //    //    sRes = "KO - Tarjeta Signatura no detectada. Certificat no trobat";
        //    //}
        //    //else
        //    //{
        //    //    sRes = AuxCert.SubjectName.Name.ToString();
        //    //}

        //    return AuxCert;

        //}

        public string CerrarStore()
        {
            string sRes;
            sRes = "";
            try
            {
                myStore2.Close();
               
            }
            catch (Exception ex)
            {
                sRes = "KO - No s'ha pogut tancar el magatzem " + ex.Message.ToString(); 
            }
            return sRes;
        }

        public string BuscaDataExpired()
        {
            X509Certificate2 AuxCert = null;
            string sRes = null;
            //Dim smartCardCerts = New List(Of X509Certificate2)()
            X509Store myStore2 = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            myStore2.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in myStore2.Certificates)
            {
                if (!cert.HasPrivateKey)
                {
                    continue;
                }
                try
                {
                    RSACryptoServiceProvider rsa;
                    rsa = (RSACryptoServiceProvider)cert.PrivateKey;
                    if (rsa == null)
                    {
                        continue;
                    }
                    if (rsa.CspKeyContainerInfo.HardwareDevice)
                    {
                        AuxCert = cert;
                        myStore2.Close();
                        break;
                    }

                    //if (IsCertificateValid(cert, ref sRes))
                    //{
                    //    // not smartcard for sure
                    //    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;
                    //    if (rsa == null)
                    //    {
                    //        continue;
                    //    }
                    //    // not smart card cert again
                    //    if (rsa.CspKeyContainerInfo.HardwareDevice)
                    //    {
                    //        // sure - smartcard
                    //        // inspect rsa.CspKeyContainerInfo.KeyContainerName Property
                    //        // or rsa.CspKeyContainerInfo.ProviderName (your smartcard provider, such as 
                    //        // "Schlumberger Cryptographic Service Provider" for Schlumberger Cryptoflex 4K
                    //        // card, etc
                    //        //name = cert.SubjectName.Name
                    //        //cert.SerialNumber.ToString()
                    //        //cert.GetExpirationDateString()
                    //        AuxCert = cert;
                    //        myStore2.Close();
                    //        // to confirm presence of private key - to finally authenticate
                    //        //rsa.SignData
                    //        break; // TODO: might not be correct. Was : Exit For
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    sRes = ex.Message.ToString();
                }

            }
            //myStore2.Close()
            if (AuxCert == null)
            {
                sRes = "KO - Tarjeta Signatura no detectada. Certificat no trobat";
            }
            else
            {
                sRes = AuxCert.GetExpirationDateString() ;
            }

            return sRes;

        }

        private bool IsCertificateValid(X509Certificate2 certificate, ref string sError)
        {
            // valid until proved otherwhise
            bool isValid = true;
            X509Chain x509Chain = this.CreateChain(certificate,ref sError);

            if (sError.ToString() != "") 
            {
                isValid=false;
                return isValid;
            }

            // Modified chain validation of the certificate. We are not interested in Ctl lists
            X509ChainStatus status;
            int index = 0;

            while (isValid && index < x509Chain.ChainStatus.Length)
            {

                status = x509Chain.ChainStatus[index];

                switch (status.Status)
                {
                    case X509ChainStatusFlags.CtlNotSignatureValid:
                    case X509ChainStatusFlags.CtlNotTimeValid:
                    case X509ChainStatusFlags.CtlNotValidForUsage:
                    case X509ChainStatusFlags.NoError:
                    case X509ChainStatusFlags.OfflineRevocation:
                    case X509ChainStatusFlags.RevocationStatusUnknown:
                        {
                            // so far, still valid
                            break;
                        }
                    case X509ChainStatusFlags.Cyclic:
                    case X509ChainStatusFlags.HasExcludedNameConstraint:
                    case X509ChainStatusFlags.HasNotDefinedNameConstraint:
                    case X509ChainStatusFlags.HasNotPermittedNameConstraint:
                    case X509ChainStatusFlags.HasNotSupportedNameConstraint:
                    case X509ChainStatusFlags.InvalidBasicConstraints:
                    case X509ChainStatusFlags.InvalidExtension:
                    case X509ChainStatusFlags.InvalidNameConstraints:
                    case X509ChainStatusFlags.InvalidPolicyConstraints:
                    case X509ChainStatusFlags.NoIssuanceChainPolicy:
                    case X509ChainStatusFlags.NotSignatureValid:
                    case X509ChainStatusFlags.NotTimeNested:
                    case X509ChainStatusFlags.NotTimeValid:
                    case X509ChainStatusFlags.NotValidForUsage:
                    case X509ChainStatusFlags.PartialChain:
                    case X509ChainStatusFlags.Revoked:
                    case X509ChainStatusFlags.UntrustedRoot:
                        {
                            sError = "KO - X509ChainStatusFlags '" + status.Status + "' is not valid, so the certificate '" + certificate.Subject + "' is not valid.";
                            //this.logger.Debug("x509Chain.ChainStatus.Length:" + x509Chain.ChainStatus.Length + ". Index: " + index + ".");
                            isValid = false;
                            break;
                        }
                    default:
                        {
                            sError = "KO - The certificate chain.ChainStatus '" + status.Status + "' is not implemented.";
                            isValid = false;
                            break;
                        }
                }

                index++;
            }

            if (isValid)
            {
                isValid = true;
                //// Check if the certificate has the default root certificate as its root
                //string x509CertificateThumbprint = certificate.Thumbprint.ToLowerInvariant();
                //if (rootCertificateDirectory.ContainsKey(x509CertificateThumbprint))
                //{
                //    // certificate is a root certificate - not valid
                //    isValid = false;
                //}
                //else
                //{
                //    // Iterate though the chain, to validate if it contain a valid root vertificate
                //    X509ChainElementCollection x509ChainElementCollection = x509Chain.ChainElements;
                //    X509ChainElementEnumerator enumerator = x509ChainElementCollection.GetEnumerator();
                //    X509ChainElement x509ChainElement;
                //    X509Certificate2 x509Certificate2;

                //    // At this point, the certificate is not valid, until a 
                //    // it is proved that it has a valid root certificate
                //    isValid = false;

                //    while (isValid == false && enumerator.MoveNext())
                //    {
                //        x509ChainElement = enumerator.Current;
                //        x509Certificate2 = x509ChainElement.Certificate;
                //        x509CertificateThumbprint = x509Certificate2.Thumbprint.ToLowerInvariant();
                //        if (rootCertificateDirectory.ContainsKey(x509CertificateThumbprint))
                //        {
                //            // The current chain element is in the trusted rootCertificateDirectory
                //            isValid = true;

                //            // now the loop will break, as we have found a trusted root certificate
                //        }
                //    }

                //    this.logger.Debug("Found root certificate in the root certificate list: " + isValid.ToString());
                //}
            }

            return isValid;
        }

        private X509Chain CreateChain(X509Certificate2 certificate, ref string sError)
        {
            X509Chain chain;
            chain = null;
            try
            {
                chain = new X509Chain();
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.Build(certificate);
            }
            catch (Exception e)
            {
                sError ="KO - CreateChain exception " + e.Message.ToString();
            }

            return chain;
        }

        #endregion

        #region IObjectSafety Members

        public enum ObjectSafetyOptions
        {
            INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001,
            INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002,
            INTERFACE_USES_DISPEX = 0x00000004,
            INTERFACE_USES_SECURITY_MANAGER = 0x00000008
        };

        public int GetInterfaceSafetyOptions(ref Guid riid, out int pdwSupportedOptions, out int pdwEnabledOptions)
        {
            ObjectSafetyOptions m_options = ObjectSafetyOptions.INTERFACESAFE_FOR_UNTRUSTED_CALLER | ObjectSafetyOptions.INTERFACESAFE_FOR_UNTRUSTED_DATA;
            pdwSupportedOptions = (int)m_options;
            pdwEnabledOptions = (int)m_options;
            return 0;
        }

        public int SetInterfaceSafetyOptions(ref Guid riid, int dwOptionSetMask, int dwEnabledOptions)
        {
            return 0;
        }

        #endregion
    
    }
}
