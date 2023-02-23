using System;
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
    public delegate void FormSendDataHandler(string msg);
    

    public partial class Configurator_Add_Setting : DevExpress.XtraEditors.XtraForm
    {
        public event FormSendDataHandler FormSendEvent;
        public Configurator_Add_Setting()
        {
            InitializeComponent();
        }
        private void simpleButton_AddSetting_Click(object sender, EventArgs e)
        {
            SendAddSettingMsg();
        }
        private void textEdit_SettingName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
                SendAddSettingMsg();
        }
        private void simpleButton_CancelSetting_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void SendAddSettingMsg()
        {
            string msg = textEdit_SettingName.Text.ToString();
            string str = @"[~!@\#$%^&*\()\=+|\\/:;?""<>']";
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(str);
            if (!rex.IsMatch(msg))
            {
                this.FormSendEvent(msg);
                this.Close();
            }
            else XtraMessageBox.Show("Please, remove special characters", "Error", MessageBoxButtons.OK);
        }

        
    }
}