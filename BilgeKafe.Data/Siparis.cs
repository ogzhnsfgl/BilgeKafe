using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeKafe.Data
{
    public class Siparis
    { 
        public int MasaNo { get; set; }

        public SiparisDurum Durum { get; set; } = SiparisDurum.Aktif;

        public decimal OdenenTutar { get; set; }
        //DateTime'ları nullable yaptık (?) ile.
        //Bu sayede Datetime null alabilir.
        //= ile default değeri dateTime.now verdik.
        // 2. Yöntem constructor içinde ayarlamak.
        public DateTime? AcilisZamani { get; set; } =DateTime.Now;
        public DateTime? KapanisZamani { get; set; }
        public List<SiparisDetay> SiparisDetaylar { get; set; } = new List<SiparisDetay>();
        public string ToplamTutarTL => $"{ToplamTutar():N2} ₺";

        public decimal ToplamTutar()
        {
            return SiparisDetaylar.Sum(siparis => siparis.Tutar());


            //decimal total = 0;
            //foreach (SiparisDetay detay in SiparisDetaylar)
            //{
            //    total=detay.Tutar();
            //}
            //return total;
        }
    }
}
