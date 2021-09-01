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
    public partial class GecmisSiparislerForm : Form
    {
        private readonly KafeVeri db;

        public GecmisSiparislerForm(KafeVeri db)
        {
            this.db = db;
            
            InitializeComponent();
            dgwSiparisDetaylar.AutoGenerateColumns = false;
            dgwSiparisler.AutoGenerateColumns = false;
            dgwSiparisler.DataSource = db.GecmisSiparisler;
            
        }


    


        private void dgwSiparisler_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwSiparisler.SelectedRows.Count != 1)
            {
                dgwSiparisDetaylar.DataSource = null;
            }
            else
            {
                DataGridViewRow satir = dgwSiparisler.SelectedRows[0];
                Siparis siparis = (Siparis) satir.DataBoundItem;
                dgwSiparisDetaylar.DataSource = siparis.SiparisDetaylar;
            }
        }
    }
}
