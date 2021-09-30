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
        public event EventHandler<MasaTasindiEventArgs> MasaTasindi; 
        private readonly KafeVeri db;
        private readonly Siparis siparis;
        private readonly BindingList<SiparisDetay> blSiparisDetay;

        public SiparisForm(KafeVeri db,Siparis siparis )
        {
            this.db = db;
            this.siparis = siparis;
            //Binding listi siparis detay listesine bağladık 
            blSiparisDetay = new BindingList<SiparisDetay>(siparis.SiparisDetaylar.ToList());
            blSiparisDetay.ListChanged += BlSiparisDetay_ListChanged;
            InitializeComponent();
            //Otomatik sütun oluşturmayı kapat:
            dgwSiparisDetayları.AutoGenerateColumns = false;
            dgwSiparisDetayları.DataSource = blSiparisDetay;
            UrunleriListele();
            MasaNoyuGuncelle();
            MasaNolariListele(); //Taşıma için
            OdemeTutariniGuncelle();
        }

        private void MasaNolariListele()
        {
            cboMasaNo.DataSource = Enumerable
                .Range(1, db.MasaAdet)
                .Where(i => db.Siparisler.All(s => s.MasaNo != i && s.Durum==SiparisDurum.Aktif))
                .ToList();
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
            cboUrun.DataSource = db.Urunler.ToList();
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
                BirimFiyat = urun.BirimFiyat,
                UrunId = urun.Id
            };
            //bl'ye eklemek hem sipariş detay listesine hem dgw ye ekleyecek!
            siparis.SiparisDetaylar.Add(sd);
            db.SaveChanges();
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
            db.SaveChanges();
            Close();
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            int yeniMasaNo = Convert.ToInt32(cboMasaNo.SelectedItem);
            int eskiMasaNo = siparis.MasaNo;
            siparis.MasaNo = yeniMasaNo;
            db.SaveChanges();
            MasaNoyuGuncelle();
            MasaNolariListele();

            if (MasaTasindi != null)
                MasaTasindi(this, new MasaTasindiEventArgs() {EskiMasaNo = eskiMasaNo, YeniMasaNo = yeniMasaNo});
        }

        private void dgwSiparisDetayları_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            SiparisDetay sd = (SiparisDetay) e.Row.DataBoundItem;
            db.SiparisDetaylar.Remove(sd);
            db.SaveChanges();
        }
    }
}
