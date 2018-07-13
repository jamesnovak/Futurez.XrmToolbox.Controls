using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Metadata;

namespace Futurez.Xrm.XrmToolbox.Controls
{
    public partial class EntitiesListControl : UserControl, IXrmToolboxControl
    {
        #region Event Definitions
        /// <summary>
        /// Event fired when the progress changes for an async event 
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event fired when the progress changes for an async event")]
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// Event fired when Initialize() completes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event fired when Initialize() completes")]
        public event EventHandler InitializeComplete;

        /// <summary>
        /// Event that fires when LoadData() completes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event that fires when LoadData() completes")]
        public event EventHandler LoadDataComplete;

        /// <summary>
        /// Event that fires when ClearData() completes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event that fires when ClearData() completes")]
        public event EventHandler ClearDataComplete;

        /// <summary>
        /// Event that fires when Close() completes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event that fires when Close() completes")]
        public event EventHandler CloseComplete;

        /// <summary>
        /// Event that fires when the Selected Item changes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event that fires when the Selected Item changes")]
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        /// <summary>
        /// Event that fires when the list of Checked Items changes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event that fires when the list of Checked Items changes")]
        public event EventHandler CheckedItemsChanged;

        /// <summary>
        /// Event that fires when FilterEntitiesList() completes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event that fires when the list of Checked Items changes")]
        public event EventHandler FilterEntitiesListComplete;

        public class SelectedItemChangedEventArgs : EventArgs
        {
            private readonly EntityMetadata _entity = null;
            public EntityMetadata Entity { get => _entity; }
            public SelectedItemChangedEventArgs(EntityMetadata entity) {
                _entity = entity;
            }
        }
        #endregion

        #region Public properties

        #region Options
        [DisplayName("Column Display Mode")]
        [Description("Display additional column details or Name and Entity Logical Name only")]
        [Category("XrmToolbox")]
        public ListViewColumnDisplayMode ColumnDisplayMode
        {
            get { return _config.ColumnDisplayMode; }
            set { _config.ColumnDisplayMode = value;
                // reset the list view with the new settings
                PopulateListView();
            }
        }

        [DisplayName("Entity Types")]
        [Description("Which Entity types should be loaded on retrieve.")]
        [Category("XrmToolbox")]
        public EnumEntityTypes EntityTypes
        {
            get { return _config.EntityTypes; }
            set { _config.EntityTypes = value; }
        }

        [DisplayName("Entity Filters")]
        [Description("List of Entities to excluded upon retrieval.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public List<FilterInfo> EntityFilters
        {
            get { return _config.EntityFilters; }
            internal set {  _config.EntityFilters = value; }
        }

        [DisplayName("Retrieve As If Published")]
        [Description("Flag indicating whether to retrieve the metadata that has not been published")]
        [Category("XrmToolbox")]
        public bool RetrieveAsIfPublished
        {
            get { return _config.RetrieveAsIfPublished; }
            set { _config.RetrieveAsIfPublished = value; }
        }

        private bool _groupByType = true;

        [DisplayName("Group By Type")]
        [Description("Flag indicating whether to group the Entities by System or Custom type")]
        [Category("XrmToolbox")]
        public bool GroupByType
        {
            get { return _groupByType; }
            set { _groupByType = value;
                // reset the list view with the new settings
                PopulateListView();
            }
        }

        [DisplayName("Display Toolbar")]
        [Description("Toggle the display of the toolbar within the control")]
        [Category("XrmToolbox")]
        [Browsable(true)]
        public bool DisplayToolbar
        {
            get { return toolStripMain.Visible; }
            set { toolStripMain.Visible = value; }
        }
        #endregion

        #region Runtime Properties

        [DisplayName("Checked Entities List")]
        [Description("List of all Entities that are checked in the control.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public List<EntityMetadata> CheckedEntities { get; private set; } = null;

        [DisplayName("Selected Entity")]
        [Description("The Entity that is currently selected the control.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public EntityMetadata SelectedEntity { get; private set; } = null;

        [DisplayName("All Entities List")]
        [Description("List of all Entities that have been loaded.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public List<EntityMetadata> AllEntities { get; private set; } = null;
        #endregion
        #endregion

        #region Private items
        private PluginControlBase _parent;
        private IOrganizationService _service;
        private List<ListViewItem> _entitiesListViewItemsColl = null;
        private bool _performingBulkSelection = false; // let's keep the listview from flickering and crashing
        
        private ConfigurationInfo _config = null;
        #endregion

        public EntitiesListControl()
        {
            InitializeComponent();

            // set up some default values and uI state
            _config = new ConfigurationInfo();
            ToggleMainControlsEnabled(false);
        }

        #region IXrmToolboxControl

        public void Initialize(PluginControlBase parent, IOrganizationService service)
        {
            // set up some vars
            _parent = parent;
            _service = service;

            ClearData();

            // only enalbe stuff if the connection is made 
            ToggleMainControlsEnabled(_service != null);

            InitializeComplete?.Invoke(this, new EventArgs());
        }

        public void Close()
        {
            _parent = null;
            _service = null;

            ClearData();

            CloseComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Clear out the saved entities list and update the UI accordingly
        /// </summary>
        public void ClearData()
        {
            // clear out list view list, collection of entities, etc.
            _entitiesListViewItemsColl?.Clear();
            CheckedEntities = new List<EntityMetadata>();
            AllEntities = new List<EntityMetadata>();

            listViewEntities.Items.Clear();

            ClearDataComplete?.Invoke(this, new EventArgs());
        }


        /// <summary>
        /// Load the data for this control... We want entities!
        /// </summary>
        public void LoadData()
        {
            ToggleMainControlsEnabled(false);

            // first clear out all data currently loaded
            ClearData();

            _parent.WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving the list of Entities",
                AsyncArgument = _config,
                IsCancelable = true,
                MessageWidth = 340,
                MessageHeight = 150,
                Work = (w, e) =>
                {
                    try
                    {
                        w.ReportProgress(0, "Loading Entities from CRM");

                        var config = e.Argument as ConfigurationInfo;

                        e.Result = CrmActions.GetAllEntities(_service, config);

                        w.ReportProgress(100, "Loading Entities from CRM Complete!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occured attetmpting to load the list of Entities:\n" + ex.Message);
                    }
                },
                ProgressChanged = e =>
                {
                    ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(e.ProgressPercentage, e.UserState.ToString()));
                },
                PostWorkCallBack = e =>
                {
                    // launch the results window... get off this worker thread so we can work with the dialog correctly
                    BeginInvoke(new LoadEntitiesCompleteDelegate(LoadEntitiesComplete), new object[] { e.Result as List<EntityMetadata> });
                }
            });
        }
        /// <summary>
        /// Update the active connection for the control
        /// </summary>
        /// <param name="newService"></param>
        public void UpdateConnection(IOrganizationService newService)
        {
            _service = newService;

            ClearData();

            ToggleMainControlsEnabled(_service != null);
        }

        #endregion

        #region Events

        public delegate void EntitySelectedDelegate(List<EntityMetadata> entity);

        public delegate void LoadEntitiesCompleteDelegate(List<EntityMetadata> entites);

        private void LoadEntitiesComplete(List<EntityMetadata> entities)
        {
            AllEntities = new List<EntityMetadata>();

            foreach (var entity in entities)
            {
                // filter based on configuration settings
                if (_config.FilterEntity(entity.LogicalName))
                {
                    continue;
                }
                // see if we are filtering by system and custom
                else if (_config.EntityTypes != EnumEntityTypes.BothCustomAndSystem)
                {
                    if ((_config.EntityTypes == EnumEntityTypes.Custom) && (!entity.IsCustomEntity.Value))
                    {
                        continue;
                    }
                    else if ((_config.EntityTypes == EnumEntityTypes.System) && (entity.IsCustomEntity.Value))
                    {
                        continue;
                    }
                }

                AllEntities.Add(entity);
            }

            // now that the entities are loaded, populate the list view.
            PopulateListView();

            ToggleMainControlsEnabled(true);

            // signal that the load of the entities is complete 
            LoadDataComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Load the Entities into the list view, binding the columns based on the control properties
        /// </summary>
        private void PopulateListView()
        {
            listViewEntities.Items.Clear();
            listViewEntities.Refresh();

            listViewEntities.SuspendLayout();

            // persist the list of list view items for the filtering
            _entitiesListViewItemsColl = new List<ListViewItem>();

            // init the correct list view columns
            SetUpListViewColumns();

            if (AllEntities != null)
            {
                foreach (var entity in AllEntities)
                {
                    // compact mode uses display name and schema name
                    var displayName = (entity.DisplayName.LocalizedLabels.Count > 0) ? entity.DisplayName.LocalizedLabels[0].Label : entity.SchemaName;
                    var entityType = (entity.IsCustomEntity.Value) ? "Custom" : "System";

                    var lvItem = new ListViewItem()
                    {
                        Name = "Display Name",
                        ImageIndex = 0,
                        StateImageIndex = 0,
                        Text = displayName,
                        Tag = entity,  // stash the template here so we can view details later
                        Group = (GroupByType) ? listViewEntities.Groups[entityType] : null
                    };
                    lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, entity.SchemaName) { Tag = "SchemaName", Name = "Schema Name" });

                    if (ColumnDisplayMode == ListViewColumnDisplayMode.Expanded)
                    {
                        var state = (entity.IsManaged.Value) ? "Managed" : "Unmanaged";
                        var description = (entity.Description.LocalizedLabels.Count > 0) ? entity.Description.LocalizedLabels[0].Label : "";
                        lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, entity.LogicalName) { Tag = "Name", Name = "Name" });
                        lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, state) { Tag = "State", Name = "State" });
                        lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, description) { Tag = "Description", Name = "Description" });
                    }

                    // add to the internal collection of ListView Items and the external list
                    _entitiesListViewItemsColl.Add(lvItem);
                }
                listViewEntities.Items.AddRange(_entitiesListViewItemsColl.ToArray<ListViewItem>());
            }

            listViewEntities.ResumeLayout();
        }

        private void SetUpListViewColumns() {

            var cols = listViewEntities.Columns;
            if (ColumnDisplayMode == ListViewColumnDisplayMode.Compact)
            {
                if (cols.Contains(colDescription))
                    cols.Remove(colDescription);
                if (cols.Contains(colState))
                    cols.Remove(colState);
                if (cols.Contains(colName))
                    cols.Remove(colName);
            }
            else {

                if (!cols.Contains(colName))
                    cols.Insert(1, colName);
                if (!cols.Contains(colState))
                    cols.Add(colState);
                if (!cols.Contains(colDescription))
                    cols.Add(colDescription);
            }

            var groups = listViewEntities.Groups;
            if (GroupByType)
            {
                var listViewGroup1 = new ListViewGroup("System", System.Windows.Forms.HorizontalAlignment.Left) { Header = "System", Name = "System", Tag = "System" };
                var listViewGroup2 = new ListViewGroup("System", System.Windows.Forms.HorizontalAlignment.Left) { Header = "Custom", Name = "Custom", Tag = "Custom" };
                groups.Add(listViewGroup1);
                groups.Add(listViewGroup2);
            }
            else {
                groups.Clear();
            }
        }

        #endregion

        #region UI event handlers

        private void ToolStripTextFilter_TextChanged(object sender, EventArgs e)
        {
            FilterEntitiesList();
        }

        private void ToolButtonLoadEntities_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void ListViewEntities_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!_performingBulkSelection)
            {
                UpdateSelectedItemsList();
            }
        }
 
        private void ListViewEntities_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // clear current selection 
            SelectedEntity = null;

            if (!e.IsSelected)
                return;

            // grab the current selected entity.
            SelectedEntity = (EntityMetadata)e.Item.Tag;
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedEntity));
        }

        private void ListViewEntities_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortEntitiesList(e.Column);
        }

        private void ToolLinkCheckNone_Click(object sender, EventArgs e)
        {
            CheckAllNone(false);
        }

        private void ToolLinkCheckAll_Click(object sender, EventArgs e)
        {
            CheckAllNone(true);
        }
        #endregion

        #region Private helper methods

        /// <summary>
        /// Toggle main ui elements disabled while we show the results
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleMainControlsEnabled(bool enabled)
        {
            listViewEntities.Enabled = enabled;
            toolStripMain.Enabled = enabled;
            toolButtonLoadEntities.Enabled = enabled;
            toolLinkCheckAll.Enabled = enabled;
            toolLinkCheckNone.Enabled = enabled;
            toolStripTextFilter.Enabled = enabled;
        }

        private void SortEntitiesList()
        {
            var currCol = 0;
            int.TryParse(listViewEntities.Tag.ToString(), out currCol);
            SortEntitiesList(currCol);
        }

        /// <summary>
        /// Handle the sorting on each column,using the sort provider
        /// </summary>
        /// <param name="column"></param>
        private void SortEntitiesList(int column)
        {
            _performingBulkSelection = true;

            listViewEntities.SuspendLayout();

            if (column == int.Parse(listViewEntities.Tag.ToString()))
            {
                listViewEntities.Sorting = ((this.listViewEntities.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending);
                listViewEntities.ListViewItemSorter = new ListViewItemComparer(column, listViewEntities.Sorting);
                return;
            }
            listViewEntities.Tag = column;
            listViewEntities.ListViewItemSorter = new ListViewItemComparer(column, SortOrder.Ascending);

            _performingBulkSelection = false;

            listViewEntities.ResumeLayout();
        }

        /// <summary>
        /// Update the list of selected items based on the list of Checked items in the list view
        /// </summary>
        private void UpdateSelectedItemsList()
        {

            if (_performingBulkSelection) {
                return;
            }

            if (CheckedEntities == null) {
                CheckedEntities = new List<EntityMetadata>();
            }

            if (listViewEntities.CheckedItems.Count == 0) {
                CheckedEntities.Clear();
            }
            else
            {
                foreach (ListViewItem item in listViewEntities.Items)
                {
                    var entity = (EntityMetadata)item.Tag;
                    if (item.Checked)
                    {
                        // if not already added, add the checked item
                        if (!CheckedEntities.Contains(entity)) {
                            CheckedEntities.Add(entity);
                        }
                    }
                    else
                    {
                        // if already added, then remove it
                        if (CheckedEntities.Contains(entity)) {
                            CheckedEntities.Remove(entity);
                        }
                    }
                }
            }

            CheckedItemsChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Toggle all or none checked in the main list view
        /// </summary>
        /// <param name="checkAll"></param>
        private void CheckAllNone(bool checkAll)
        {
            _performingBulkSelection = true;

            listViewEntities.SuspendLayout();

            if (checkAll)
            {
                foreach (ListViewItem item in listViewEntities.Items) {
                    item.Checked = true;
                }
            }
            else
            {
                foreach (ListViewItem item in listViewEntities.CheckedItems) {
                    item.Checked = false;
                }
            }
            listViewEntities.ResumeLayout();

            _performingBulkSelection = false;

            // now that we have an updated list view, udpate the list of selected items
            UpdateSelectedItemsList();
        }

        /// <summary>
        /// Filter the entities list against the provided string 
        /// </summary>
        public void FilterEntitiesList(string filterText)
        {
            toolStripTextFilter.Text = filterText;
        }

        /// <summary>
        /// Filter the entities list using the text in the text box.
        /// </summary>
        private void FilterEntitiesList()
        {
            string filterText = toolStripTextFilter.Text.ToLower();

            _performingBulkSelection = true;

            listViewEntities.SuspendLayout();

            // 
            if (filterText.Length > 0)
            {
                // filter the master list and bind it to the list view
                var filteredList = _entitiesListViewItemsColl
                    .Where(i => i.Text.ToLower().Contains(filterText) ||
                                i.SubItems["Name"].Text.ToLower().Contains(filterText)
                    );
                // for some reason, on filter, the group gets lost
                listViewEntities.Items.Clear();
                listViewEntities.Items.AddRange(filteredList.ToArray<ListViewItem>());
            }
            else
            {
                // clear filter, add all items back
                listViewEntities.Items.Clear();
                listViewEntities.Items.AddRange(_entitiesListViewItemsColl.ToArray<ListViewItem>());
            }

            // for some reason, on filter, the group gets lost
            ResetGroups(_entitiesListViewItemsColl);

            _performingBulkSelection = false;

            // now that we have an updated list view, udpate the list of selected items
            UpdateSelectedItemsList();

            listViewEntities.ResumeLayout();

            FilterEntitiesListComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Reset the groups on the list view control
        /// </summary>
        /// <param name="items"></param>
        private void ResetGroups(List<ListViewItem> items)
        {
            // for some reason, on filter, the group gets lost
            foreach (ListViewItem item in items)
            {
                var entity = item.Tag as EntityMetadata;
                var entityType = (entity.IsCustomEntity.Value) ? "Custom" : "System";
                item.Group = listViewEntities.Groups[entityType];
            }
        }
        #endregion
    }
}
