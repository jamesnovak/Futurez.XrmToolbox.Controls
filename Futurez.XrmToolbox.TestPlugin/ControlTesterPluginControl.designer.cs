using System.ComponentModel;

namespace Futurez.XrmToolbox.Controls
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
            this.EntitiesListControl = new Futurez.XrmToolbox.Controls.EntitiesListControl();
            this.textBoxEventLog = new System.Windows.Forms.TextBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.propertyGridDetails = new System.Windows.Forms.PropertyGrid();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonShowEntList = new System.Windows.Forms.RadioButton();
            this.radioButtonShowEntity = new System.Windows.Forms.RadioButton();
            this.entitiesDropdownControl1 = new Futurez.XrmToolbox.Controls.EntitiesDropdownControl();
            this.toolStripMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
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
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripMenu.Size = new System.Drawing.Size(2047, 37);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(148, 34);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 37);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(112, 34);
            this.toolStripButton1.Text = "Load Data";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
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
            this.tableLayoutPanel1.Controls.Add(this.propertyGridDetails, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.entitiesDropdownControl1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 37);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(2047, 1120);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // EntitiesListControl
            // 
            this.EntitiesListControl.AutoLoadData = false;
            this.EntitiesListControl.Checkboxes = true;
            this.EntitiesListControl.ColumnDisplayMode = Futurez.XrmToolbox.Controls.ListViewColumnDisplayMode.Compact;
            this.EntitiesListControl.DisplayToolbar = true;
            this.EntitiesListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EntitiesListControl.EntityTypes = Futurez.XrmToolbox.Controls.EnumEntityTypes.BothCustomAndSystem;
            this.EntitiesListControl.GroupByType = true;
            this.EntitiesListControl.Location = new System.Drawing.Point(11, 66);
            this.EntitiesListControl.Margin = new System.Windows.Forms.Padding(11);
            this.EntitiesListControl.Name = "EntitiesListControl";
            this.EntitiesListControl.Padding = new System.Windows.Forms.Padding(6);
            this.EntitiesListControl.ParentBaseControl = null;
            this.EntitiesListControl.RetrieveAsIfPublished = true;
            this.EntitiesListControl.Service = null;
            this.EntitiesListControl.Size = new System.Drawing.Size(1001, 965);
            this.EntitiesListControl.TabIndex = 12;
            this.EntitiesListControl.ProgressChanged += new System.EventHandler<System.ComponentModel.ProgressChangedEventArgs>(this.EntitiesListControl1_ProgressChanged);
            this.EntitiesListControl.InitializeComplete += new System.EventHandler(this.EntitiesListControl1_InitializeComplete);
            this.EntitiesListControl.LoadDataComplete += new System.EventHandler(this.EntitiesListControl1_LoadDataComplete);
            this.EntitiesListControl.ClearDataComplete += new System.EventHandler(this.EntitiesListControl1_ClearDataComplete);
            this.EntitiesListControl.CloseComplete += new System.EventHandler(this.EntitiesListControl1_CloseComplete);
            this.EntitiesListControl.SelectedItemChanged += new System.EventHandler<Futurez.XrmToolbox.Controls.EntitiesListControl.SelectedItemChangedEventArgs>(this.EntitiesListControl1_SelectedItemChanged);
            this.EntitiesListControl.CheckedItemsChanged += new System.EventHandler(this.EntitiesListControl1_CheckedItemsChanged);
            // 
            // textBoxEventLog
            // 
            this.textBoxEventLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxEventLog.Enabled = false;
            this.textBoxEventLog.Location = new System.Drawing.Point(1540, 6);
            this.textBoxEventLog.Margin = new System.Windows.Forms.Padding(6);
            this.textBoxEventLog.Multiline = true;
            this.textBoxEventLog.Name = "textBoxEventLog";
            this.tableLayoutPanel1.SetRowSpan(this.textBoxEventLog, 2);
            this.textBoxEventLog.Size = new System.Drawing.Size(501, 1030);
            this.textBoxEventLog.TabIndex = 7;
            // 
            // labelMessage
            // 
            this.labelMessage.BackColor = System.Drawing.SystemColors.Info;
            this.labelMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.Location = new System.Drawing.Point(6, 0);
            this.labelMessage.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(1011, 55);
            this.labelMessage.TabIndex = 10;
            this.labelMessage.Text = " ↓  This is an Entity List View Control!   ↓";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // propertyGridDetails
            // 
            this.propertyGridDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridDetails.Location = new System.Drawing.Point(1029, 61);
            this.propertyGridDetails.Margin = new System.Windows.Forms.Padding(6);
            this.propertyGridDetails.Name = "propertyGridDetails";
            this.tableLayoutPanel1.SetRowSpan(this.propertyGridDetails, 2);
            this.propertyGridDetails.Size = new System.Drawing.Size(499, 1053);
            this.propertyGridDetails.TabIndex = 8;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.radioButtonShowEntList);
            this.flowLayoutPanel1.Controls.Add(this.radioButtonShowEntity);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(1029, 6);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(499, 43);
            this.flowLayoutPanel1.TabIndex = 13;
            // 
            // radioButtonShowEntList
            // 
            this.radioButtonShowEntList.AutoSize = true;
            this.radioButtonShowEntList.Checked = true;
            this.radioButtonShowEntList.Location = new System.Drawing.Point(6, 6);
            this.radioButtonShowEntList.Margin = new System.Windows.Forms.Padding(6);
            this.radioButtonShowEntList.Name = "radioButtonShowEntList";
            this.radioButtonShowEntList.Size = new System.Drawing.Size(236, 29);
            this.radioButtonShowEntList.TabIndex = 3;
            this.radioButtonShowEntList.TabStop = true;
            this.radioButtonShowEntList.Text = "Entity List View Control";
            this.radioButtonShowEntList.UseVisualStyleBackColor = true;
            this.radioButtonShowEntList.CheckedChanged += new System.EventHandler(this.radioButtonShowEntList_CheckedChanged);
            // 
            // radioButtonShowEntity
            // 
            this.radioButtonShowEntity.AutoSize = true;
            this.radioButtonShowEntity.Location = new System.Drawing.Point(254, 6);
            this.radioButtonShowEntity.Margin = new System.Windows.Forms.Padding(6);
            this.radioButtonShowEntity.Name = "radioButtonShowEntity";
            this.radioButtonShowEntity.Size = new System.Drawing.Size(167, 29);
            this.radioButtonShowEntity.TabIndex = 2;
            this.radioButtonShowEntity.TabStop = true;
            this.radioButtonShowEntity.Text = "Selected Entity";
            this.radioButtonShowEntity.UseVisualStyleBackColor = true;
            this.radioButtonShowEntity.CheckedChanged += new System.EventHandler(this.radioButtonShowEntity_CheckedChanged);
            // 
            // entitiesDropdownControl1
            // 
            this.entitiesDropdownControl1.AutoLoadData = false;
            this.entitiesDropdownControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entitiesDropdownControl1.Location = new System.Drawing.Point(6, 1048);
            this.entitiesDropdownControl1.Margin = new System.Windows.Forms.Padding(6);
            this.entitiesDropdownControl1.Name = "entitiesDropdownControl1";
            this.entitiesDropdownControl1.ParentBaseControl = null;
            this.entitiesDropdownControl1.Service = null;
            this.entitiesDropdownControl1.Size = new System.Drawing.Size(1011, 66);
            this.entitiesDropdownControl1.TabIndex = 14;
            // 
            // ControlTesterPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStripMenu);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ControlTesterPluginControl";
            this.Size = new System.Drawing.Size(2047, 1157);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
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
        private System.Windows.Forms.PropertyGrid propertyGridDetails;
        private System.Windows.Forms.Label labelMessage;
        private EntitiesListControl EntitiesListControl;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton radioButtonShowEntList;
        private System.Windows.Forms.RadioButton radioButtonShowEntity;
        private EntitiesDropdownControl entitiesDropdownControl1;
    }
}
