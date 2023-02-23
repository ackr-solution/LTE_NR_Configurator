using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;

namespace Configurator
{
    public partial class WaitForm_Loading : WaitForm
    {
        public WaitForm_Loading()
        {
            InitializeComponent();
            this.pPanel_Loading.AutoHeight = true;
        }

        #region Overrides

        public override void SetCaption(string caption)
        {
            base.SetCaption(caption);
            this.pPanel_Loading.Caption = caption;
        }
        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            this.pPanel_Loading.Description = description;
        }
        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum WaitFormCommand
        {
        }
    }
}