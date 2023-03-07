using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Configurator.Dialog
{
    public partial class Configurator_Loss_Setting : DevExpress.XtraEditors.XtraForm
    {
        public Configurator_Loss_Setting()
        {
            InitializeComponent();
            ucLoss_8000.InitLossTable("ERF", 10);

            string filename = Environment.CurrentDirectory + string.Format("\\currentLossTbl_ACKRLOSS.lss");
            if (File.Exists(filename) == true)
            {
                ucLoss_8000.ImportLossList(filename);
            }
        }

        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton_Ok_Click(object sender, EventArgs e)
        {
            string filename = Environment.CurrentDirectory + string.Format("\\currentLossTbl_ACKRLOSS.lss");
            if (File.Exists(filename) == true)
            {
                File.Delete(filename);
            }
            ucLoss_8000.ExportLossList(filename);
            this.Close();
        }
    }
}