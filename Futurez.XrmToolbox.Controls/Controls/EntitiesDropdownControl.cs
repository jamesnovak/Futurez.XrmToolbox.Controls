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
    /// <summary>
    /// Shared XrmToolbox Control that will load a list of entities into a Dropdown control
    /// </summary>
    public partial class EntitiesDropdownControl : UserControl, IXrmToolboxControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EntitiesDropdownControl()
        {
            InitializeComponent();
        }

        #region Private properties
        private PluginControlBase _parent = null;
        private IOrganizationService _service = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// The currently selected EntityMetadata object in the ListView
        /// </summary>
        [DisplayName("Selected Entity")]
        [Description("The Entity that is currently selected in the Dropdown.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public EntityMetadata SelectedEntity { get; private set; } = null;

        /// <summary>
        /// List of all loaded EntityMetadata objects for the current connection
        /// </summary>
        [DisplayName("All Entities List")]
        [Description("List of all Entities that have been loaded into the Dropdown.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public List<EntityMetadata> AllEntities { get; private set; } = null;
        #endregion

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
        [Description("Event fired when Initialize() of the Dropdown completes")]
        public event EventHandler InitializeComplete;

        /// <summary>
        /// Event that fires when <see cref="LoadData"/>() completes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event that fires when LoadData() completes for the Dropdown")]
        public event EventHandler LoadDataComplete;

        /// <summary>
        /// Event that fires when ClearData() completes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event that fires when ClearData() completes for the Dropdown")]
        public event EventHandler ClearDataComplete;

        /// <summary>
        /// Event that fires when Close() completes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event that fires when Close() completes for the Dropdown")]
        public event EventHandler CloseComplete;

        /// <summary>
        /// Event that fires when the Selected Item changes
        /// </summary>
        [Category("XrmToolbox")]
        [Description("Event that fires when the Selected Item in the Dropdown changes")]
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        
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
        #endregion

        /// <summary>
        /// Clear all loaded data in your control
        /// </summary>
        public void ClearData()
        {
            comboEntities.DataSource = null;
            comboEntities.Items.Clear();
            ClearDataComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Close the control and release anything that might be hanging around
        /// </summary>
        public void Close()
        {
            _parent = null;
            _service = null;

            ClearData();

            CloseComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Initialize the control with parent and service references
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="service"></param>
        public void Initialize(PluginControlBase parent, IOrganizationService service)
        {
            if ((service == null) || (parent == null)) {
                throw new InvalidOperationException("Both parent and service must be provided.");
            }

            AllEntities = new List<EntityMetadata>();
            SelectedEntity = null;

            // set up some vars
            _parent = parent;
            _service = service;

            ClearData();

            InitializeComplete?.Invoke(this, new EventArgs());
        }

        private class ComboItem
        {
            internal ComboItem(string name, string value, EntityMetadata entityMeta)
            {
                _name = name;
                _value = value;
                _entityMeta = entityMeta;
                if (name != value) {
                    _displayname = $"{_name} ({_value})";
                }
                else {
                    _displayname = name;
                }
            }
            private string _name;
            private string _displayname;
            private string _value;
            private EntityMetadata _entityMeta;

            public string Name { get => _name; set => _name = value; }
            public string DisplayName { get => _displayname; set => _displayname = value; }
            public string Value { get => _value; set => _value = value; }
            internal EntityMetadata EntityMeta { get => _entityMeta; set => _entityMeta = value; }
        }

        /// <summary>
        /// Load the list of entities
        /// </summary>
        public void LoadData()
        {
            ClearData();

            _parent.WorkAsync(new WorkAsyncInfo {
                Message = "Retrieving the list of Entities",
                AsyncArgument = null,
                IsCancelable = true,
                MessageWidth = 340,
                MessageHeight = 150,
                Work = (w, e) => {
                    try {
                        w.ReportProgress(0, "Loading Entities from CRM");

                        // retrieve all entities and bind to the combo
                        e.Result = CrmActions.RetrieveAllEntities(_service);

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
                    var entities = (List<EntityMetadata>)e.Result;

                    LoadDataComplete?.Invoke(this,new EventArgs());

                    AllEntities = entities;

                    LoadComboItems();
                }
            });
        }

        private void LoadComboItems()
        {
            comboEntities.SuspendLayout();

            ClearData();

            var items = from ent in AllEntities
                        select new ComboItem(CrmActions.GetLocalizedLabel(ent.DisplayName, ent.SchemaName), ent.SchemaName, ent);

            comboEntities.DataSource = items.OrderBy(e=> e.DisplayName).ToList();
            comboEntities.DisplayMember = "DisplayName";
            comboEntities.ValueMember = "Value";

            if (comboEntities.Items.Count > 0) {
                comboEntities.SelectedIndex = 0;
            }

            comboEntities.ResumeLayout();
        }

        /// <summary>
        /// Handle the updated connection 
        /// </summary>
        /// <param name="newService">Reference to the new IOrganizationService</param>
        public void UpdateConnection(IOrganizationService newService)
        {
            _service = newService;

            ClearData();
        }

        private void ComboEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedEntity = null;

            if (comboEntities.SelectedItem is ComboItem item) {
                SelectedEntity = item.EntityMeta;
            }
            
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedEntity));
        }

        private void ButtonReload_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
