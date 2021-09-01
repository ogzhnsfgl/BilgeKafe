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
    public partial class UrunlerForm : Form
    {
        private readonly KafeVeri db;
        private BindingList<Urun> blUrunler;

        public UrunlerForm(KafeVeri db)
        {
            blUrunler = new BindingList<Urun>(db.Urunler);
            this.db = db;
            InitializeComponent();
            UrunleriListele();
        }


        private void UrunleriListele()
        {
            dgwUrunler.AutoGenerateColumns = false;
            dgwUrunler.DataSource = blUrunler;
        }

        private void btnUrunEkle_Click(object sender, EventArgs e)
        {
            string ad = txtUrunAd.Text.Trim();
            if (ad == "")
            {
                MessageBox.Show("Ürün ismi alanı boş olamaz!");
                return;
            }
            if (btnUrunEkle.Text == "Ekle")
            {

                Urun urun = new Urun() { UrunAd = txtUrunAd.Text, BirimFiyat = nudBirimFiyat.Value };
                blUrunler.Add(urun);

            }
            else
            {
                DataGridViewRow satir = dgwUrunler.SelectedRows[0];
                Urun urun = (Urun)satir.DataBoundItem;
                urun.UrunAd = ad;
                urun.BirimFiyat = nudBirimFiyat.Value;
                blUrunler.ResetBindings();
            }

            FormuTemizle();

        }

        private void dgwUrunler_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Ürünü silmek istediğinizden emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            e.Cancel = dr == DialogResult.No;


        }

        private void btnDuzenle_Click(object sender, EventArgs e)
        {
            if (dgwUrunler.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow satir = dgwUrunler.SelectedRows[0];
            Urun urun = (Urun)satir.DataBoundItem;
            txtUrunAd.Text = urun.UrunAd;
            nudBirimFiyat.Value = urun.BirimFiyat;
            btnUrunEkle.Text = "Kaydet";
            btnIptal.Show();
            dgwUrunler.Enabled = false;
            btnDuzenle.Enabled = false;
            txtUrunAd.Focus();
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            FormuTemizle();
        }

        private void FormuTemizle()
        {
            txtUrunAd.Clear();
            nudBirimFiyat.Value = 0;
            btnUrunEkle.Text = "Ekle";
            btnIptal.Hide();
            dgwUrunler.Enabled = true;
            btnDuzenle.Enabled = true;
            txtUrunAd.Focus();
        }
    }
}
