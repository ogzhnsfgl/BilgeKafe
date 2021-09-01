using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BilgeKafe.Data;

namespace BilgeKafe.UI
{
    public partial class SiparisForm : Form
    {
        private readonly KafeVeri db;
        private readonly Siparis siparis;
        private readonly BindingList<SiparisDetay> blSiparisDetay;

        public SiparisForm(KafeVeri db,Siparis siparis )
        {
            this.db = db;
            this.siparis = siparis;
            //Binding listi siparis detay listesine bağladık 
            blSiparisDetay = new BindingList<SiparisDetay>(siparis.SiparisDetaylar);
            blSiparisDetay.ListChanged += BlSiparisDetay_ListChanged;
            InitializeComponent();
            //Otomatik sütun oluşturmayı kapat:
            dgwSiparisDetayları.AutoGenerateColumns = false;
            dgwSiparisDetayları.DataSource = blSiparisDetay;
            UrunleriListele();
            MasaNoyuGuncelle();
            OdemeTutariniGuncelle();
        }

        //Binding list'te her değişikli kolduğunda odemetutarını güncelleyecek!
        private void BlSiparisDetay_ListChanged(object sender, ListChangedEventArgs e)
        {
            OdemeTutariniGuncelle();
        }

        private void OdemeTutariniGuncelle()
        {
            lblOdemeTutari.Text = siparis.ToplamTutarTL;
        }

        private void UrunleriListele()
        {
            cboUrun.DataSource = db.Urunler;
        }

        private void MasaNoyuGuncelle()
        {
            Text = $"Masa {siparis.MasaNo}";
            lblMasaNo.Text = $"{siparis.MasaNo:00}";
            
        }

        private void btnDetayEkle_Click(object sender, EventArgs e)
        {
            Urun urun = (Urun) cboUrun.SelectedItem;
            int adet = (int) nudAdet.Value;
            if (urun==null)
            {
                MessageBox.Show("Önce bir ürün seçmelisiniz!");
                return;
            }

            SiparisDetay sd = new SiparisDetay()
            {
                UrunAd = urun.UrunAd,
                Adet = adet,
                BirimFiyat = urun.BirimFiyat
            };
            //bl'ye eklemek hem sipariş detay listesine hem dgw ye ekleyecek!
            blSiparisDetay.Add(sd);
            //Ödeme tutarı güncellenecek binding list event'ına atadık.
            
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOdemAl_Click(object sender, EventArgs e)
        {
            DialogResult dr= MessageBox.Show($"{lblOdemeTutari.Text} tutarında ödeme alındı. Onaylıyor musunuz?" ,"Ödeme Onayı",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button1);

            switch (dr)
            {
                case DialogResult.Yes:
                    SiparisiKapat(SiparisDurum.Odendi);
                    break;
                case DialogResult.No:
                    break;
            }
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show($"Sipariş iptal edilecektir. Onaylıyor musunuz?", "İptal Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            switch (dr)
            {
                case DialogResult.Yes:
                    SiparisiKapat(SiparisDurum.Iptal);
                    break;
                case DialogResult.No:
                    break;
            }
        }

        private void SiparisiKapat(SiparisDurum durum)
        {
            siparis.OdenenTutar = durum == SiparisDurum.Odendi ? siparis.ToplamTutar() : 0;
            siparis.Durum = durum;
            siparis.KapanisZamani = DateTime.Now;
            db.AktifSiparisler.Remove(siparis);
            db.GecmisSiparisler.Add(siparis);
            Close();
        }
    }
}
