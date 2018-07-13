using System.ComponentModel;

namespace Futurez.Xrm.XrmToolbox.Controls
{
    partial class ControlTesterPluginControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlTesterPluginControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.EntitiesListControl = new Futurez.Xrm.XrmToolbox.Controls.EntitiesListControl();
            this.textBoxEventLog = new System.Windows.Forms.TextBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.checkBoxGridProps = new System.Windows.Forms.CheckBox();
            this.toolStripMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.toolStripButton1});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(987, 31);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(86, 28);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(28, 28);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.EntitiesListControl, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxEventLog, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelMessage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.propertyGrid1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxGridProps, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 31);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(987, 538);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // EntitiesListControl
            // 
            this.EntitiesListControl.ColumnDisplayMode = Futurez.Xrm.XrmToolbox.Controls.ListViewColumnDisplayMode.Compact;
            this.EntitiesListControl.DisplayToolbar = false;
            this.EntitiesListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EntitiesListControl.EntityTypes = Futurez.Xrm.XrmToolbox.Controls.EnumEntityTypes.BothCustomAndSystem;
            this.EntitiesListControl.GroupByType = true;
            this.EntitiesListControl.Location = new System.Drawing.Point(3, 33);
            this.EntitiesListControl.Name = "EntitiesListControl";
            this.EntitiesListControl.Padding = new System.Windows.Forms.Padding(3);
            this.EntitiesListControl.RetrieveAsIfPublished = true;
            this.EntitiesListControl.Size = new System.Drawing.Size(487, 482);
            this.EntitiesListControl.TabIndex = 12;
            this.EntitiesListControl.ProgressChanged += new System.EventHandler<System.ComponentModel.ProgressChangedEventArgs>(this.EntitiesListControl1_ProgressChanged);
            this.EntitiesListControl.InitializeComplete += new System.EventHandler(this.EntitiesListControl1_InitializeComplete);
            this.EntitiesListControl.LoadDataComplete += new System.EventHandler(this.EntitiesListControl1_LoadDataComplete);
            this.EntitiesListControl.ClearDataComplete += new System.EventHandler(this.EntitiesListControl1_ClearDataComplete);
            this.EntitiesListControl.CloseComplete += new System.EventHandler(this.EntitiesListControl1_CloseComplete);
            this.EntitiesListControl.SelectedItemChanged += new System.EventHandler<Futurez.Xrm.XrmToolbox.Controls.EntitiesListControl.SelectedItemChangedEventArgs>(this.EntitiesListControl1_SelectedItemChanged);
            this.EntitiesListControl.CheckedItemsChanged += new System.EventHandler(this.EntitiesListControl1_CheckedItemsChanged);
            // 
            // textBoxEventLog
            // 
            this.textBoxEventLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxEventLog.Enabled = false;
            this.textBoxEventLog.Location = new System.Drawing.Point(742, 3);
            this.textBoxEventLog.Multiline = true;
            this.textBoxEventLog.Name = "textBoxEventLog";
            this.tableLayoutPanel1.SetRowSpan(this.textBoxEventLog, 2);
            this.textBoxEventLog.Size = new System.Drawing.Size(242, 512);
            this.textBoxEventLog.TabIndex = 7;
            // 
            // labelMessage
            // 
            this.labelMessage.BackColor = System.Drawing.SystemColors.Info;
            this.labelMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.Location = new System.Drawing.Point(3, 0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(487, 30);
            this.labelMessage.TabIndex = 10;
            this.labelMessage.Text = " ↓  This is an Entity List View Control!   ↓";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(496, 33);
            this.propertyGrid1.Name = "propertyGrid1";
            this.tableLayoutPanel1.SetRowSpan(this.propertyGrid1, 2);
            this.propertyGrid1.Size = new System.Drawing.Size(240, 502);
            this.propertyGrid1.TabIndex = 8;
            // 
            // checkBoxGridProps
            // 
            this.checkBoxGridProps.AutoSize = true;
            this.checkBoxGridProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxGridProps.Location = new System.Drawing.Point(496, 3);
            this.checkBoxGridProps.Name = "checkBoxGridProps";
            this.checkBoxGridProps.Size = new System.Drawing.Size(240, 24);
            this.checkBoxGridProps.TabIndex = 13;
            this.checkBoxGridProps.Text = "Show Entities List Properties";
            this.checkBoxGridProps.UseVisualStyleBackColor = true;
            this.checkBoxGridProps.CheckedChanged += new System.EventHandler(this.checkBoxGridProps_CheckedChanged);
            // 
            // ControlTesterPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "ControlTesterPluginControl";
            this.Size = new System.Drawing.Size(987, 569);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBoxEventLog;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Label labelMessage;
        private EntitiesListControl EntitiesListControl;
        private System.Windows.Forms.CheckBox checkBoxGridProps;
    }
}
