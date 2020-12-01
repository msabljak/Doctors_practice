using System;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using FiskalizacijaService;
using System.Globalization;
using System.Text;
using System.Linq;
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
            var header = new ZaglavljeType()
            {
                IdPoruke = Guid.NewGuid().ToString(),
                DatumVrijeme = DateTime.Now.ToString("dd'.'MM'.'yyyy'T'HH':'mm':'ss")
            };

            var invoice = new RacunType()
            {
                BrRac = new BrojRacunaType()
                {
                    BrOznRac = "1",
                    OznPosPr = "1",
                    OznNapUr = "1"
                },
                DatVrijeme = DateTime.Now.ToString("dd'.'MM'.'yyyy'T'HH':'mm':'ss"),
                IznosUkupno = 3.ToString("N2", CultureInfo.InvariantCulture),
                NacinPlac = NacinPlacanjaType.G,
                NakDost = false,
                Oib = "98765432198",
                OibOper = "98642375382",
                OznSlijed = OznakaSlijednostiType.N,
                Pdv = new[]
                {
                    new PorezType
                    {
                        Stopa = 25.ToString("N2", CultureInfo.InvariantCulture),
                        Osnovica = 2.34.ToString("N2", CultureInfo.InvariantCulture),
                        Iznos = .56.ToString("N2", CultureInfo.InvariantCulture)
                    }
                },
                USustPdv = true
            };

            var invoiceRequest = new RacunZahtjev()
            {
                Zaglavlje = header,
                Racun = invoice
            };

            var invoiceResponse = SignAndSendInvoiceRequest(invoiceRequest);
            Console.WriteLine("Zahtjev poslan");
            Console.ReadLine();
        }

        private static RacunOdgovor SignAndSendInvoiceRequest(RacunZahtjev request)
        {
            if (request != null && request.Racun.ZastKod == null)
            {
                GenerateZKI(request.Racun);
            }

            request.Id = request.GetType().Name;

            #region Serialise XML from Request and Sign XML

            SignedXml xml = null;
            var ser = Serialize(request);
            var doc = new XmlDocument();
            doc.LoadXml(ser);

            xml = new SignedXml(doc);
            xml.SigningKey = cert.PrivateKey;
            xml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

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
            xml.AddReference(reference);
            xml.ComputeSignature();
            #endregion

            #region Add signature elements from signed XML to RacunZahtjev

            var s = xml.Signature;
            var certSerial = (X509IssuerSerial)keyInfoData.IssuerSerials[0];
            request.Signature = new SignatureType
            {
                SignedInfo = new SignedInfoType
                {
                    CanonicalizationMethod = new CanonicalizationMethodType { Algorithm = s.SignedInfo.CanonicalizationMethod },
                    SignatureMethod = new SignatureMethodType { Algorithm = s.SignedInfo.SignatureMethod },
                    Reference =
                        (from x in s.SignedInfo.References.OfType<Reference>()
                         select new ReferenceType
                         {
                             URI = x.Uri,
                             Transforms =
                                 (from t in transforms
                                  select new TransformType { Algorithm = t.Algorithm }).ToArray(),
                             DigestMethod = new DigestMethodType { Algorithm = x.DigestMethod },
                             DigestValue = x.DigestValue
                         }).ToArray()
                },
                SignatureValue = new SignatureValueType { Value = s.SignatureValue },
                KeyInfo = new KeyInfoType
                {
                    ItemsElementName = new[] { ItemsChoiceType2.X509Data },
                    Items = new[]
                    {
                        new X509DataType
                        {
                            ItemsElementName = new[]
                            {
                                ItemsChoiceType.X509IssuerSerial,
                                ItemsChoiceType.X509Certificate
                            },
                            Items = new object[]
                            {
                                new X509IssuerSerialType
                                {
                                    X509IssuerName = certSerial.IssuerName,
                                    X509SerialNumber = certSerial.SerialNumber
                                },
                                cert.RawData
                            }
                        }
                    }
                }
            };
            #endregion


            FiskalizacijaPortTypeClient client = new FiskalizacijaPortTypeClient();
            return client.racuniAsync(request).Result.RacunOdgovor;
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
            var signData = provider.SignData(b, HashAlgorithmName.SHA1,RSASignaturePadding.Pkcs1);

            // Compute hash
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(signData);
            var result = new string(hash.SelectMany(x => x.ToString("x2")).ToArray());

            return result;
        }

        public static string Serialize(RacunZahtjev request)
        {
            if (request == null) throw new ArgumentNullException("request");

            // Fix empty arrays to null
            if (request is RacunZahtjev)
            {
                var rz = (RacunZahtjev)request;

                var r = rz.Racun;
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
    }
}
