using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using  BilgeKafe.Data;
using BilgeKafe.UI.Properties;
using Newtonsoft.Json;

namespace BilgeKafe.UI
{
    public partial class AnaForm : Form
    {
        private KafeVeri db = new KafeVeri();
        public AnaForm()
        {
            VerileriOku();

            //OrnekUrunleriOlustur();
            InitializeComponent();
            MasalariOlustur();
        }

        private void VerileriOku()
        {
            try
            {
                string jsonDb = File.ReadAllText("db.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(jsonDb);
            }
            catch (Exception e)
            {
               
            }
        }

        //private void OrnekUrunleriOlustur()
        //{
        //    db.Urunler.Add(new Urun(){UrunAd = "Kola",BirimFiyat = 5.99m} );
        //    db.Urunler.Add(new Urun(){UrunAd = "Çay",BirimFiyat = 3.50m} );
        //}

        private void MasalariOlustur()
        {
            #region ImageList oluşturulması

            ImageList imageList = new ImageList();
            imageList.Images.Add("bos", Resources.bos);
            imageList.Images.Add("dolu", Resources.dolu);
            imageList.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = imageList;
            #endregion
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                ListViewItem lvi = new ListViewItem($"Masa {i}");
                lvi.ImageKey = db.AktifSiparisler.Any(s => s.MasaNo == i) ? "dolu" : "bos";
                lvi.Tag = i; //Sonra tıklarken hangi masaya tıklandığını anlamak için koyduk.
                lvwMasalar.Items.Add(lvi);
            }
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            // Seçili lvi'yi bir lvi'ye ata.Sonra onun tagini masaNo'ya aktar.
            ListViewItem lvi = lvwMasalar.SelectedItems[0];
            int masaNo = (int) lvi.Tag;
            lvi.ImageKey = "dolu";

            // Tıklanan masaya ait varsa siparişi bul
            // Aktif sipariş var mı kontrol ettik eğer yoksa null dönecek.

            Siparis siparis= db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo);

            // Eğer sipariş henüz oluşturulmadıysa yani masa kapalıysa yeni sipariş oluştur

            if (siparis==null)
            {
                siparis = new Siparis()
                    { MasaNo = masaNo };
                db.AktifSiparisler.Add(siparis);
            }
            SiparisForm formSiparis = new SiparisForm(db, siparis);
            
            //Sipariş formunu açtık
            formSiparis.ShowDialog();

            //Sipariş formu kapanırken durumu kontrol ediyoruz aktif değilse yani ödeme alındıysa
            // yada iptal edildiyse imageKey'i boşa çekiyoruz.

            if (siparis.Durum!=SiparisDurum.Aktif)
            {
                lvi.ImageKey = "bos";
            }
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            new GecmisSiparislerForm(db).ShowDialog();
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            new UrunlerForm(db).ShowDialog();
        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string dbJson = JsonConvert.SerializeObject(db);
            File.WriteAllText("db.json",dbJson);

        }
    }
}
