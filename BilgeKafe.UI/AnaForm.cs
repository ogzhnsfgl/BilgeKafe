using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using  BilgeKafe.Data;
using BilgeKafe.UI.Properties;

namespace BilgeKafe.UI
{
    public partial class AnaForm : Form
    {
        private KafeVeri db = new KafeVeri();
        public AnaForm()
        {

            OrnekUrunleriOlustur();
            InitializeComponent();
            MasalariOlustur();
        }

        private void OrnekUrunleriOlustur()
        {
            db.Urunler.Add(new Urun(){UrunAd = "Kola",BirimFiyat = 5.99m} );
            db.Urunler.Add(new Urun(){UrunAd = "Çay",BirimFiyat = 3.50m} );
        }

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
                lvi.ImageKey = "bos";
                lvi.Tag = i; //Sonra tıklarken hangi masaya tıklandığını anlamak için koyduk.
                lvwMasalar.Items.Add(lvi);
            }
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            //Seçili lvi'yi bir lvi'ye ata.Sonra onun tagini masaNo'ya aktar.
            ListViewItem lvi = lvwMasalar.SelectedItems[0];
            int masaNo = (int) lvi.Tag;
            lvi.ImageKey = "dolu";

            // Tıklanan masaya ait varsa siparişi bul

            //Aktif sipariş var mı kontrol ettik eğer yoksa null dönecek.

            Siparis siparis= db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo);

            //Eğer sipariş henüz oluşturulmadıysa yani masa kapalıysa yeni sipariş oluştur

            if (siparis==null)
            {
                siparis = new Siparis()
                    { MasaNo = masaNo };
                db.AktifSiparisler.Add(siparis);
            }
            
            SiparisForm formSiparis = new SiparisForm(db, siparis);
            formSiparis.ShowDialog();


        }
    }
}
