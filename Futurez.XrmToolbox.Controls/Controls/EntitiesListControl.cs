using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Metadata;

namespace Futurez.XrmToolbox.Controls
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
        /// Event that fires when <see cref="LoadData"/>() completes
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

        /// <summary>
        /// Event Arguments for the Selected Item Changed event that provides the new Selected EntityMetadata object
        /// </summary>
        public class SelectedItemChangedEventArgs : EventArgs
        {
            /// <summary>
            /// The new Selected EntityMetadata object
            /// </summary>
            public EntityMetadata Entity { get; } = null;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="entity"></param>
            internal SelectedItemChangedEventArgs(EntityMetadata entity)
            {
                Entity = entity;
            }
        }
        #endregion

        #region Public properties

        #region Options
        /// <summary>
        /// Display additional column details or Name and Entity Logical Name only
        /// </summary>
        [DisplayName("Column Display Mode")]
        [Description("Display additional column details or Name and Entity Logical Name only")]
        [Category("XrmToolbox")]
        public ListViewColumnDisplayMode ColumnDisplayMode
        {
            get { return _config.ColumnDisplayMode; }
            set {
                _config.ColumnDisplayMode = value;
                // reset the list view with the new settings
                PopulateListView();
            }
        }

        /// <summary>
        /// Flag indicating whether to display Checkboxes in the ListView control
        /// </summary>
        [DisplayName("Display Checkboxes")]
        [Description("Flag indicating whether to display Checkboxes in the ListView control")]
        [Category("XrmToolbox")]
        public bool Checkboxes
        {
            get { return ListViewEntities.CheckBoxes; }
            set {
                // uncheck everything if no checkboxes
                if (value == false) {
                    CheckAllNone(false);
                }
                _performingBulkSelection = true;
                ListViewEntities.SuspendLayout();

                ListViewEntities.CheckBoxes = value;

                toolLinkCheckAll.Enabled = ListViewEntities.CheckBoxes;
                toolLinkCheckNone.Enabled = ListViewEntities.CheckBoxes;

                ListViewEntities.ResumeLayout();
                _performingBulkSelection = false;
            }
        }

        /// <summary>
        /// Defines which Entity types should be loaded on retrieve.
        /// </summary>
        [DisplayName("Entity Types")]
        [Description("Defines which Entity types should be loaded on retrieve.")]
        [Category("XrmToolbox")]
        public EnumEntityTypes EntityTypes
        {
            get { return _config.EntityTypes; }
            set { _config.EntityTypes = value; }
        }

        /// <summary>
        /// List of Entities to excluded upon retrieval.
        /// </summary>
        [DisplayName("Entity Filters")]
        [Description("List of Entities to excluded upon retrieval.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public List<FilterInfo> EntityFilters
        {
            get { return _config.EntityFilters; }
            internal set { _config.EntityFilters = value; }
        }

        /// <summary>
        /// List of EntityFilters to be applied on the 
        /// </summary>
        [DisplayName("Entity Request Filters")]
        [Description("List of EntityFilters applied to Entity retrieval. This is the EntityFilter structure passed with the RetrieveAllEntitiesRequest")]
        [Category("XrmToolbox")]
        public List<EntityFilters> EntityRequestFilters
        {
            get { return _config.EntityRequestFilters; }
            internal set { _config.EntityRequestFilters = value; }
        }

        /// <summary>
        /// Flag indicating whether to retrieve the metadata that has not been published
        /// </summary>
        [DisplayName("Retrieve As If Published")]
        [Description("Flag indicating whether to retrieve the metadata that has not been published")]
        [Category("XrmToolbox")]
        public bool RetrieveAsIfPublished
        {
            get { return _config.RetrieveAsIfPublished; }
            set { _config.RetrieveAsIfPublished = value; }
        }

        /// <summary>
        /// Flag indicating whether to group the Entities by System or Custom type
        /// </summary>
        [DisplayName("Group By Type")]
        [Description("Flag indicating whether to group the Entities by System or Custom type")]
        [Category("XrmToolbox")]
        public bool GroupByType
        {
            get { return _groupByType; }
            set {
                _groupByType = value;
                // reset the list view with the new settings
                PopulateListView();
            }
        }

        /// <summary>
        /// Toggle the display of the toolbar within the control
        /// </summary>
        [DisplayName("Display Toolbar")]
        [Description("Toggle the display of the toolbar within the control")]
        [Category("XrmToolbox")]
        [Browsable(true)]
        public bool DisplayToolbar
        {
            get { return toolStripMain.Visible; }
            set { toolStripMain.Visible = value; }
        }

        /// <summary>
        /// Filter string for the toolbar 
        /// </summary>
        [DisplayName("List Filter String")]
        [Description("Filter string applied to the main ListView control for the loaded Entities List.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public string ListFilterString
        {
            get {
                var filter = toolStripTextFilter.Text;
                return (filter != null) ? filter.Trim() : filter;
            }
            internal set { toolStripTextFilter.Text = value; }
        }

        /// <summary>
        /// Current SortOrder for the Entity List View
        /// </summary>
        [DisplayName("List Sort Order")]
        [Description("Current SortOrder for the Entity List View.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public SortOrder ListSortOrder
        {
            get {
                return ListViewEntities.Sorting;
            }
            internal set { ListViewEntities.Tag = value; }
        }

        /// <summary>
        /// Current SortColumn index for the Entity List View
        /// </summary>
        [DisplayName("List Sort Column")]
        [Description("Current Sort Column for the Entity List View.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public int ListSortColumn
        {
            get {
                var currCol = 0;
                int.TryParse(ListViewEntities.Tag.ToString(), out currCol);
                return currCol;
            }
            internal set { ListViewEntities.Tag = value; }
        }
        #endregion

        #region Runtime Properties

        /// <summary>
        /// List of all checked EntityMetadata objects in the ListView
        /// </summary>
        [DisplayName("Checked Entities List")]
        [Description("List of all Entities that are checked in the control.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public List<EntityMetadata> CheckedEntities { get; private set; } = null;

        /// <summary>
        /// The currently selected EntityMetadata object in the ListView
        /// </summary>
        [DisplayName("Selected Entity")]
        [Description("The Entity that is currently selected the control.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public EntityMetadata SelectedEntity { get; private set; } = null;

        /// <summary>
        /// List of all loaded EntityMetadata objects for the current connection
        /// </summary>
        [DisplayName("All Entities List")]
        [Description("List of all Entities that have been loaded.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public List<EntityMetadata> AllEntities { get; private set; } = null;
        #endregion
        #endregion

        #region Private items
        private PluginControlBase _parent = null;
        private IOrganizationService _service = null;
        private List<ListViewItem> _entitiesListViewItemsColl = null;
        private bool _performingBulkSelection = false; // let's keep the listview from flickering and crashing

        private bool _groupByType = true;
        private ConfigurationInfo _config = null;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public EntitiesListControl()
        {
            InitializeComponent();

            // set up some default values and uI state
            _config = new ConfigurationInfo();
            ToggleMainControlsEnabled(false);
        }
        #region Public methods


        #region IXrmToolboxControl

        /// <summary>
        /// Initialize the control with the parent XrmToolbox control and the active Organization Service
        /// </summary>
        /// <param name="parent">Reference to the containing XrmToolbox control</param>
        /// <param name="service">reference to the active IOrganizationService</param>
        /// <exception cref="InvalidOperationException">Will be thrown if the Initialize has not been called.</exception>
        public void Initialize(PluginControlBase parent, IOrganizationService service)
        {
            if ((service == null) || (parent == null)) {
                throw new InvalidOperationException("Both parent and service must be provided.");
            }

            // set up some vars
            _parent = parent;
            _service = service;

            ClearData();

            // only enalbe stuff if the connection is made 
            ToggleMainControlsEnabled(_service != null);

            InitializeComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Close the control and release all resources
        /// </summary>
        public void Close()
        {
            _parent = null;
            _service = null;

            ClearData();

            CloseComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Clear out the saved entities list and update the ListView
        /// </summary>
        public void ClearData()
        {
            // clear out list view list, collection of entities, etc.
            _entitiesListViewItemsColl?.Clear();
            CheckedEntities = new List<EntityMetadata>();
            AllEntities = new List<EntityMetadata>();

            ListViewEntities.Items.Clear();

            ClearDataComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Load the Entities using the current IOrganizationService.
        /// The call is asynchronous and will leverage the WorkAsync object on the parent XrmToolbox control
        /// </summary>
        /// <exception cref="InvalidOperationException">Will be thrown if the Initialize has not been called.</exception>
        public void LoadData()
        {
            if ((_service == null) || (_parent == null)) {
                throw new InvalidOperationException("Initialize must be called before loading the list of Entities.");
            }

            ToggleMainControlsEnabled(false);

            // first clear out all data currently loaded
            ClearData();

            _parent.WorkAsync(new WorkAsyncInfo {
                Message = "Retrieving the list of Entities",
                AsyncArgument = _config,
                IsCancelable = true,
                MessageWidth = 340,
                MessageHeight = 150,
                Work = (w, e) => {
                    try {
                        w.ReportProgress(0, "Loading Entities from CRM");

                        var config = e.Argument as ConfigurationInfo;

                        e.Result = CrmActions.RetrieveAllEntities(_service, config);

                        w.ReportProgress(100, "Loading Entities from CRM Complete!");
                    }
                    catch (Exception ex) {
                        MessageBox.Show("An error occured attetmpting to load the list of Entities:\n" + ex.Message);
                    }
                },
                ProgressChanged = e => {
                    ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(e.ProgressPercentage, e.UserState.ToString()));
                },
                PostWorkCallBack = e => {
                    // launch the results window... get off this worker thread so we can work with the dialog correctly
                    BeginInvoke(new LoadEntitiesCompleteDelegate(LoadEntitiesComplete), new object[] { e.Result as List<EntityMetadata> });
                }
            });
        }

        /// <summary>
        /// Update the active connection and IOrganizationService reference for the control
        /// This will clear the current list of EntityMetadata objects in the ListView.
        /// </summary>
        /// <param name="newService">New active instance of the IOrganizationService</param>
        public void UpdateConnection(IOrganizationService newService)
        {
            _service = newService;

            ClearData();

            ToggleMainControlsEnabled(_service != null);
        }

        #endregion

        /// <summary>
        /// Filter the entities list against the provided string 
        /// </summary>
        public void FilterEntitiesList(string filterText)
        {
            toolStripTextFilter.Text = filterText;
        }

        /// <summary>
        /// Check all items in the ListView
        /// </summary>
        public void CheckAll()
        {
            CheckAllNone(true);
        }

        /// <summary>
        /// Check all items in the ListView
        /// </summary>
        public void CheckEntities(List<string> entityLogicalNames)
        {
            if (entityLogicalNames == null)
                return;

            _performingBulkSelection = true;

            ListViewEntities.SuspendLayout();

            // if the item is in the list, check it
            foreach (ListViewItem item in ListViewEntities.Items) 
            {
                var ent = item.Tag as EntityMetadata;
                if (entityLogicalNames.Contains(ent.LogicalName.ToLower()) && Checkboxes)
                {
                    item.Checked = true;
                }
            }
            ListViewEntities.ResumeLayout();

            _performingBulkSelection = false;

            // now that we have an updated list view, udpate the list of selected items
            UpdateSelectedItemsList();
        }

        /// <summary>
        /// Uncheck all items in the ListView
        /// </summary>
        public void CheckNone()
        {
            CheckAllNone(false);
        }
        #endregion

        #region Events
        /// <summary>
        /// Event handler definition for the LoadEntitiesComplete change event
        /// </summary>
        /// <param name="entites">List of all loaded EntityMetadata objects</param>
        private delegate void LoadEntitiesCompleteDelegate(List<EntityMetadata> entites);

        /// <summary>
        /// Method to handle the Load Data Complete event
        /// </summary>
        /// <param name="entities"></param>
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
            ListViewEntities.Items.Clear();
            ListViewEntities.Refresh();

            ListViewEntities.SuspendLayout();

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
                        Group = (GroupByType) ? ListViewEntities.Groups[entityType] : null
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
                ListViewEntities.Items.AddRange(_entitiesListViewItemsColl.ToArray<ListViewItem>());
                ListViewEntities.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }

            ListViewEntities.ResumeLayout();
        }


        /// <summary>
        /// Set up the list view columns based on the ColumnDisplayMode and Groups options
        /// </summary>
        private void SetUpListViewColumns() {

            var cols = ListViewEntities.Columns;
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

            var groups = ListViewEntities.Groups;
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

        /// <summary>
        /// Handle the filter text changed event.  Apply current filter text to the ListView
        /// </summary>
        /// <param name="sender">event sender object</param>
        /// <param name="e">event args object</param>
        private void ToolStripTextFilter_TextChanged(object sender, EventArgs e)
        {
            FilterEntitiesList();
        }

        /// <summary>
        /// Hanlde the LoadEntites tool button click and load all entities from the connected instance
        /// </summary>
        /// <param name="sender">event sender object</param>
        /// <param name="e">event args object</param>
        private void ToolButtonLoadEntities_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// Handle the ListView Item Checked event and set the selected list of items
        /// </summary>
        /// <param name="sender">event sender object</param>
        /// <param name="e">event args object</param>
        private void ListViewEntities_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!_performingBulkSelection)
            {
                UpdateSelectedItemsList();
            }
        }

        /// <summary>
        /// Handle the item selection change for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Handle the column click for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewEntities_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortList(e.Column);
        }

        private void ToolLinkCheckNone_Click(object sender, EventArgs e)
        {
            CheckAllNone(false);
        }

        private void ToolLinkCheckAll_Click(object sender, EventArgs e)
        {
            CheckAllNone(true);
        }

        /// <summary>
        /// Handle keyboard select all / none
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewEntities_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Control) {
                    if (e.KeyCode == Keys.A) {
                        CheckAllNone(true);
                    }
                    else if (e.KeyCode == Keys.D) {
                        CheckAllNone(false);
                    }
                }
            }
        }
        #endregion

        #region Private helper methods

        /// <summary>
        /// Toggle main ui elements disabled while we show the results
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleMainControlsEnabled(bool enabled)
        {
            ListViewEntities.Enabled = enabled;
            toolStripMain.Enabled = enabled;
            toolButtonLoadEntities.Enabled = enabled;
            toolLinkCheckAll.Enabled = enabled;
            toolLinkCheckNone.Enabled = enabled;
            toolStripTextFilter.Enabled = enabled;
        }

        /// <summary>
        /// External method to allow sorting of the Entities List
        /// </summary>
        /// <param name="sortColumn"></param>
        /// <param name="sortorder"></param>
        public void SortEntitiesList(int sortColumn, SortOrder sortorder)
        {
            SortList(sortColumn, sortorder);
        }

        /// <summary>
        /// Sort the current list of Entities in the ListView
        /// </summary>
        private void SortEntitiesList()
        {
            SortList(ListSortColumn);
        }

        /// <summary>
        /// Sort the current list of Entities in the ListView
        /// </summary>
        /// <param name="column">ListView column index to be sorted</param>
        /// <param name="sortOrder"></param>
        private void SortList(int column, SortOrder? sortOrder = null)
        {
            // toggle the sort order if not passed as a param
            if (sortOrder == null) {
                sortOrder = (ListViewEntities.Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }

            _performingBulkSelection = true;
            ListViewEntities.SuspendLayout();

            // update entities list and save the values to properties
            ListViewEntities.Sorting = sortOrder.Value;

            ListSortOrder = ListViewEntities.Sorting;
            ListSortColumn = column;

            // now apply the sorter helper 
            ListViewEntities.ListViewItemSorter = new ListViewItemComparer(ListSortColumn, ListViewEntities.Sorting);

            _performingBulkSelection = false;

            ListViewEntities.ResumeLayout();
        }

        /// <summary>
        /// Update the list of selected items based on the list of Checked items in the ListView
        /// </summary>
        private void UpdateSelectedItemsList()
        {
            if (_performingBulkSelection) {
                return;
            }

            if (CheckedEntities == null) {
                CheckedEntities = new List<EntityMetadata>();
            }

            if (ListViewEntities.CheckedItems.Count == 0) {
                CheckedEntities.Clear();
            }
            else
            {
                foreach (ListViewItem item in ListViewEntities.Items)
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
        /// <param name="checkAll">flag indicating whether to check all items in the ListView</param>
        private void CheckAllNone(bool checkAll)
        {
            _performingBulkSelection = true;

            ListViewEntities.SuspendLayout();

            // if check all and we have checkboxes enabled, then do some work
            if (checkAll && Checkboxes)
            {
                foreach (ListViewItem item in ListViewEntities.Items) {
                    item.Checked = true;
                }
            }
            else
            {
                foreach (ListViewItem item in ListViewEntities.CheckedItems) {
                    item.Checked = false;
                }
            }
            ListViewEntities.ResumeLayout();

            _performingBulkSelection = false;

            // now that we have an updated list view, udpate the list of selected items
            UpdateSelectedItemsList();
        }

        /// <summary>
        /// Filter the entities list using the text in the text box.
        /// </summary>
        private void FilterEntitiesList()
        {
            string filterText = toolStripTextFilter.Text.ToLower();

            _performingBulkSelection = true;

            ListViewEntities.SuspendLayout();

            // 
            if (filterText.Length > 0)
            {
                // filter the master list and bind it to the list view
                var filteredList = _entitiesListViewItemsColl
                    .Where(i => i.Text.ToLower().Contains(filterText) ||
                                i.SubItems["Schema Name"].Text.ToLower().Contains(filterText)
                    );
                // for some reason, on filter, the group gets lost
                ListViewEntities.Items.Clear();
                ListViewEntities.Items.AddRange(filteredList.ToArray<ListViewItem>());
            }
            else
            {
                // clear filter, add all items back
                ListViewEntities.Items.Clear();
                ListViewEntities.Items.AddRange(_entitiesListViewItemsColl.ToArray<ListViewItem>());
            }

            // for some reason, on filter, the group gets lost
            ResetGroups(_entitiesListViewItemsColl);

            _performingBulkSelection = false;

            // now that we have an updated list view, udpate the list of selected items
            UpdateSelectedItemsList();

            ListViewEntities.ResumeLayout();

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
                item.Group = ListViewEntities.Groups[entityType];
            }
        }
        #endregion
    }
}
