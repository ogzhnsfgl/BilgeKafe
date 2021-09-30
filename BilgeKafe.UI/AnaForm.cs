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
            InitializeComponent();
            MasalariOlustur();
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
                lvi.ImageKey = db.Siparisler.Any(s => s.MasaNo == i && s.Durum == SiparisDurum.Aktif) ? "dolu" : "bos";
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

            Siparis siparis= db.Siparisler.FirstOrDefault(x => x.MasaNo == masaNo && x.Durum==SiparisDurum.Aktif);

            // Eğer sipariş henüz oluşturulmadıysa yani masa kapalıysa yeni sipariş oluştur

            if (siparis==null)
            {
                siparis = new Siparis()
                    { MasaNo = masaNo,Durum = SiparisDurum.Aktif};
                db.Siparisler.Add(siparis);
            }
            SiparisForm formSiparis = new SiparisForm(db, siparis);
            formSiparis.MasaTasindi += FormSiparis_MasaTasindi;
            
            //Sipariş formunu açtık
            formSiparis.ShowDialog();

            //Sipariş formu kapanırken durumu kontrol ediyoruz aktif değilse yani ödeme alındıysa
            // yada iptal edildiyse imageKey'i boşa çekiyoruz.

            if (siparis.Durum!=SiparisDurum.Aktif)
            {
                lvi.ImageKey = "bos";
            }
        }

        private void FormSiparis_MasaTasindi(object sender, MasaTasindiEventArgs e)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                if ((int) lvi.Tag == e.EskiMasaNo)
                    lvi.ImageKey = "bos";
                if ((int)lvi.Tag == e.YeniMasaNo)
                    lvi.ImageKey = "dolu";
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

    }
}
