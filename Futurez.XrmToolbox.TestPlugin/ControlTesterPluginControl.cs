using System;
using System.ComponentModel;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;

namespace Futurez.Xrm.XrmToolbox.Controls
{
    public partial class ControlTesterPluginControl : PluginControlBase
    {
 
        public ControlTesterPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("https://github.com/MscrmTools/XrmToolBox"));

            EntitiesListControl.Initialize(this, Service);

            EntitiesListControl.EntityFilters
                .Add(new FilterInfo() {
                        FilterMatchType = EnumFilterMatchType.StartsWith,
                        FilterString = "gnext_"
                    });
            propertyGrid1.SelectedObject = EntitiesListControl.EntityFilters;

            EntitiesListControl.LoadData();
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);

            // ENTITIES LIST - update the connection 
            EntitiesListControl.UpdateConnection(newService);
        }

        #region Child control event handlers
        private void EntitiesListControl1_CloseComplete(object sender, EventArgs e)
        {
            textBoxEventLog.Text += string.Format($"CloseComplete") + Environment.NewLine;
        }
        private void EntitiesListControl1_InitializeComplete(object sender, EventArgs e)
        {
            // ENTITIES LIST - log event
            textBoxEventLog.Text += string.Format($"InitializeComplete") + Environment.NewLine;
        }

        private void EntitiesListControl1_LoadDataComplete(object sender, EventArgs e)
        {
            // ENTITIES LIST - log event
            textBoxEventLog.Text += string.Format($"LoadDataComplete - All Entities count: {EntitiesListControl.AllEntities.Count }\n") + Environment.NewLine;

            // NOW THAT DATA IS LOADED, APPLY FILTER
            // EntitiesListControl.FilterEntitiesList("Account");
        }

        private void EntitiesListControl1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // ENTITIES LIST - log event
            textBoxEventLog.Text += string.Format($"ProgressChanged: {e.ProgressPercentage}") + Environment.NewLine;
        }

        private void EntitiesListControl1_CheckedItemsChanged(object sender, EventArgs e)
        {
            // ENTITIES LIST - log event
            textBoxEventLog.Text += string.Format($"CheckedItemsChanged - Checked Entities count: {EntitiesListControl.CheckedEntities.Count}") + Environment.NewLine;
        }
        private void EntitiesListControl1_SelectedItemChanged(object sender, EntitiesListControl.SelectedItemChangedEventArgs e)
        {
            // ENTITIES LIST - log event
            textBoxEventLog.Text += string.Format($"SelectedItemChanged: {e.Entity?.SchemaName}") + Environment.NewLine;

            SetPropertySelectedObject();
        }
        private void EntitiesListControl1_ClearDataComplete(object sender, EventArgs e)
        {
            // ENTITIES LIST - log event
            textBoxEventLog.Text += string.Format($"ClearDataComplete - All Entities count: {EntitiesListControl.AllEntities.Count}") + Environment.NewLine;
        }
        #endregion

        private void checkBoxGridProps_CheckedChanged(object sender, EventArgs e)
        {
            SetPropertySelectedObject();
        }

        private void SetPropertySelectedObject() {
            if (checkBoxGridProps.Checked) {
                propertyGrid1.SelectedObject = EntitiesListControl;
            }
            else {
                propertyGrid1.SelectedObject = EntitiesListControl.SelectedEntity;
            }

        }
    }
}