using FiskalizacijaService;
using RestSharp;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CryptographyService
{
    class Program
    {
        private static string Certificate = @"C:\Users\CSVarazdin\Downloads\FISKAL_2.p12";
        private static X509Certificate2 cert = new X509Certificate2(Certificate, "Mediion123!",
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

        [STAThread]
        static void Main(string[] args)
        {
            var invoiceRequest = new InvoiceRequest().ToRacunZahtjev();
            var invoiceResponse = SignAndSendInvoiceRequest(invoiceRequest);
            var responseDocument = new XmlDocument();
            responseDocument.LoadXml(invoiceResponse.Content);
            responseDocument.Save(@".\ResponseXML.xml");
        }

        private static IRestResponse SignAndSendInvoiceRequest(RacunZahtjev request)
        {
            if (request != null && request.Racun.ZastKod == null)
            {
                GenerateZKI(request.Racun);
            }

            request.Id = request.GetType().Name;
            
            var ser = Serialize(request);
            var doc = new XmlDocument();
            doc.LoadXml(ser);

            doc.Save(@".\UnsignedXML.xml");

            doc = Sign(doc, request);

            XDocument xdoc = AddSoapEnvelope(doc);
            xdoc.Save(@".\SoapEnvelope");
           
            var soapEnvelopeXml = xdoc.ToXmlDocument();
            string address = "https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest";

            return SendSoapEnvelope(address,soapEnvelopeXml);
        }

        private static XmlDocument Sign(XmlDocument doc, RacunZahtjev request)
        {
            SignedXml xml = null;
            xml = new SignedXml(doc);
            xml.SigningKey = cert.PrivateKey;
            xml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            xml.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

            var keyInfo = new KeyInfo();
            var keyInfoData = new KeyInfoX509Data();
            keyInfoData.AddCertificate(cert);
            keyInfoData.AddIssuerSerial(cert.Issuer, cert.GetSerialNumberString());
            keyInfo.AddClause(keyInfoData);
            xml.KeyInfo = keyInfo;

            var transforms = new Transform[]
            {
                new XmlDsigEnvelopedSignatureTransform(false),
                new XmlDsigExcC14NTransform(false)
            };

            Reference reference = new Reference("#" + request.Id);
            foreach (var x in transforms)
                reference.AddTransform(x);
            reference.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
            xml.AddReference(reference);
            xml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = xml.GetXml();

            // Append the element to the XML document.
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            if (doc.FirstChild is XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            // Save the signed XML document to a file specified
            // using the passed string.
            XmlTextWriter xmltw = new XmlTextWriter("SignedXML.xml", new UTF8Encoding(false));
            doc.WriteTo(xmltw);
            xmltw.Close();

            return doc;
        }

        private static void GenerateZKI(RacunType invoice)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(invoice.Oib);
            sb.Append(invoice.DatVrijeme);
            sb.Append(invoice.BrRac.BrOznRac);
            sb.Append(invoice.BrRac.OznPosPr);
            sb.Append(invoice.BrRac.OznNapUr);
            sb.Append(invoice.IznosUkupno);

            invoice.ZastKod = SignAndHashMD5(sb.ToString());
        }

        private static string SignAndHashMD5(string value)
        {
            byte[] b = Encoding.ASCII.GetBytes(value);
            RSA provider = cert.GetRSAPrivateKey();
            var signData = provider.SignData(b, HashAlgorithmName.SHA1,RSASignaturePadding.Pss);

            // Compute hash
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(signData);
            var result = new string(hash.SelectMany(x => x.ToString("x2")).ToArray());

            return result;
        }

        private static string Serialize(RacunZahtjev request)
        {
            if (request == null) throw new ArgumentNullException("request");

            // Fix empty arrays to null
            if (request is RacunZahtjev)
            {
                var rz = (RacunZahtjev)request;

                var r = rz.Racun;
                var s = rz.Signature;
                Action<Array, Action> fixArray = (x, y) =>
                {
                    var isEmpty = x != null && !x.OfType<object>().Any(x1 => x1 != null);
                    if (isEmpty)
                        y();
                };
                fixArray(r.Naknade, () => r.Naknade = null);
                fixArray(r.OstaliPor, () => r.OstaliPor = null);
                fixArray(r.Pdv, () => r.Pdv = null);
                fixArray(r.Pnp, () => r.Pnp = null);
            }

            using (var ms = new MemoryStream())
            {
                // Set namespace to root element
                var root = new XmlRootAttribute { Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = false };
                var ser = new XmlSerializer(request.GetType(), root);
                ser.Serialize(ms, request);

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static XDocument AddSoapEnvelope(XmlDocument xml)
        {
            XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
            string soapenvPrefix = "soapenv";
            XNamespace tns = "http://www.apis-it.hr/fin/2012/types/f73";

            //creating the envelope element (1st)
            XElement envelope = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + soapenvPrefix, soapenv));

            XElement body = new XElement(soapenv + "Body");
            XElement racunZahtjev = XElement.Load(new XmlNodeReader(xml));
            body.Add(racunZahtjev);

            //adding body to envelope
            envelope.Add(body);

            //adding envelope to doc and publishing
            XDocument doc = new XDocument();
            doc.Add(envelope);
            return doc;
        }

        private static IRestResponse SendSoapEnvelope(string address, XmlDocument soapEnvelopeXml)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            var client = new RestClient(address);
            client.Timeout = -1;
            var req = new RestRequest(Method.POST);
            req.AddHeader("Content-Type", "application/xml");
            req.AddParameter("application/xml", soapEnvelopeXml.InnerXml, ParameterType.RequestBody);
            return client.Execute(req);
        }
    }
}
