using FiskalizacijaService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CryptographyService
{
    public class InvoiceRequest : RacunZahtjev
    {
        public InvoiceRequest()
        {
            Zaglavlje = GetDefaultHeader();
            Racun = GetDefaultInvoice();
        }

        private RacunType GetDefaultInvoice()
        {
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
                Oib = "07989965722",
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

            return invoice;
        }

        private ZaglavljeType GetDefaultHeader()
        {
            var header = new ZaglavljeType()
            {
                IdPoruke = Guid.NewGuid().ToString(),
                DatumVrijeme = DateTime.Now.ToString("dd'.'MM'.'yyyy'T'HH':'mm':'ss")
            };

            return header;
        }

        public RacunZahtjev ToRacunZahtjev()
        {
            RacunZahtjev racunZahtjev = new RacunZahtjev();
            racunZahtjev.Zaglavlje = this.Zaglavlje;
            racunZahtjev.Racun = this.Racun;
            return racunZahtjev;
        }
    }
}
