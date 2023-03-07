
namespace Configurator.Dialog
{
    partial class Configurator_Loss_Setting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configurator_Loss_Setting));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.ucLoss_8000 = new AckrLib_Loss.UcLoss_8000();
            this.simpleButton_Ok = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.ucLoss_8000);
            this.layoutControl1.Controls.Add(this.simpleButton_Ok);
            this.layoutControl1.Controls.Add(this.simpleButton_Cancel);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(789, 634);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // ucLoss_8000
            // 
            this.ucLoss_8000.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ucLoss_8000.Appearance.Options.UseFont = true;
            this.ucLoss_8000.Location = new System.Drawing.Point(13, 16);
            this.ucLoss_8000.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ucLoss_8000.Name = "ucLoss_8000";
            this.ucLoss_8000.Size = new System.Drawing.Size(763, 569);
            this.ucLoss_8000.TabIndex = 4;
            // 
            // simpleButton_Ok
            // 
            this.simpleButton_Ok.Location = new System.Drawing.Point(396, 591);
            this.simpleButton_Ok.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.simpleButton_Ok.Name = "simpleButton_Ok";
            this.simpleButton_Ok.Size = new System.Drawing.Size(187, 27);
            this.simpleButton_Ok.StyleController = this.layoutControl1;
            this.simpleButton_Ok.TabIndex = 5;
            this.simpleButton_Ok.Text = "Ok";
            this.simpleButton_Ok.Click += new System.EventHandler(this.simpleButton_Ok_Click);
            // 
            // simpleButton_Cancel
            // 
            this.simpleButton_Cancel.Location = new System.Drawing.Point(587, 591);
            this.simpleButton_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.simpleButton_Cancel.Name = "simpleButton_Cancel";
            this.simpleButton_Cancel.Size = new System.Drawing.Size(189, 27);
            this.simpleButton_Cancel.StyleController = this.layoutControl1;
            this.simpleButton_Cancel.TabIndex = 6;
            this.simpleButton_Cancel.Text = "Cancel";
            this.simpleButton_Cancel.Click += new System.EventHandler(this.simpleButton_Cancel_Click);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(789, 634);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.ucLoss_8000;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(767, 575);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 575);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(383, 33);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.simpleButton_Ok;
            this.layoutControlItem2.Location = new System.Drawing.Point(383, 575);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(191, 33);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.simpleButton_Cancel;
            this.layoutControlItem3.Location = new System.Drawing.Point(574, 575);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(193, 33);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // Configurator_Loss_Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 634);
            this.Controls.Add(this.layoutControl1);
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("Configurator_Loss_Setting.IconOptions.Image")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Configurator_Loss_Setting";
            this.Text = "Loss table setting";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private AckrLib_Loss.UcLoss_8000 ucLoss_8000;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Ok;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
    }
}