
namespace Configurator.Dialog
{
    partial class Configurator_Add_Setting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.textEdit_SettingName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleButton_AddSetting = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleButton_CancelSetting = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_SettingName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.textEdit_SettingName);
            this.layoutControl1.Controls.Add(this.simpleButton_AddSetting);
            this.layoutControl1.Controls.Add(this.simpleButton_CancelSetting);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(338, 87);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(338, 87);
            this.Root.TextVisible = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // textEdit_SettingName
            // 
            this.textEdit_SettingName.Location = new System.Drawing.Point(12, 29);
            this.textEdit_SettingName.Name = "textEdit_SettingName";
            this.textEdit_SettingName.Size = new System.Drawing.Size(314, 20);
            this.textEdit_SettingName.StyleController = this.layoutControl1;
            this.textEdit_SettingName.TabIndex = 4;
            this.textEdit_SettingName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textEdit_SettingName_KeyDown);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.textEdit_SettingName;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(318, 41);
            this.layoutControlItem1.Text = "Enter the setting name";
            this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(129, 14);
            // 
            // simpleButton_AddSetting
            // 
            this.simpleButton_AddSetting.Location = new System.Drawing.Point(12, 53);
            this.simpleButton_AddSetting.Name = "simpleButton_AddSetting";
            this.simpleButton_AddSetting.Size = new System.Drawing.Size(155, 22);
            this.simpleButton_AddSetting.StyleController = this.layoutControl1;
            this.simpleButton_AddSetting.TabIndex = 5;
            this.simpleButton_AddSetting.Text = "Add";
            this.simpleButton_AddSetting.Click += new System.EventHandler(this.simpleButton_AddSetting_Click);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.simpleButton_AddSetting;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 41);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(159, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // simpleButton_CancelSetting
            // 
            this.simpleButton_CancelSetting.Location = new System.Drawing.Point(171, 53);
            this.simpleButton_CancelSetting.Name = "simpleButton_CancelSetting";
            this.simpleButton_CancelSetting.Size = new System.Drawing.Size(155, 22);
            this.simpleButton_CancelSetting.StyleController = this.layoutControl1;
            this.simpleButton_CancelSetting.TabIndex = 6;
            this.simpleButton_CancelSetting.Text = "Cancel";
            this.simpleButton_CancelSetting.Click += new System.EventHandler(this.simpleButton_CancelSetting_Click);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.simpleButton_CancelSetting;
            this.layoutControlItem3.Location = new System.Drawing.Point(159, 41);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(159, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // Configurator_Add_Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 87);
            this.Controls.Add(this.layoutControl1);
            this.Name = "Configurator_Add_Setting";
            this.Text = "Add setting information";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_SettingName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit textEdit_SettingName;
        private DevExpress.XtraEditors.SimpleButton simpleButton_AddSetting;
        private DevExpress.XtraEditors.SimpleButton simpleButton_CancelSetting;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}