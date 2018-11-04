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

            // handle the enabled changed event
            EnabledChanged += EntitiesDropdownControl_EnabledChanged;

            ToggleMainControlsEnabled();
        }

        /// <summary>
        /// Handle the Enabled property change to ensure that the child controls are also disabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntitiesDropdownControl_EnabledChanged(object sender, EventArgs e)
        {
            ToggleMainControlsEnabled();
        }

        #region Private properties
        private PluginControlBase _parent = null;
        private IOrganizationService _service = null;

        private bool _autoLoadData = false;

        #endregion

        #region Public Properties
        /// <summary>
        /// Flag indicating whether to automatically load data when the Service connection is set or updated.
        /// </summary>
        [DisplayName("Automatically LoadData")]
        [Description("Flag indicating whether to automatically load data when the Service connection is set or updated.")]
        [Category("XrmToolbox")]
        public bool AutoLoadData
        {
            get { return _autoLoadData; }
            set { _autoLoadData = value; }
        }
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

        [DisplayName("Organization Service")]
        [Description("Refefrence to the IOrganizationService context.")]
        [Category("XrmToolbox")]
        [Browsable(false)]
        public IOrganizationService Service { get => _service; set => UpdateConnection(value); }

        [DisplayName("Parent PluginBase Control")]
        [Description("Refefrence to the parent XrmToolBox Plugin Base Control.")]
        [Category("XrmToolbox")]
        public PluginControlBase ParentBaseControl { get => _parent; set => _parent = value; }

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

        [Category("XrmToolbox")]
        [Description("Event that fires when the Connection Update completes")]
        public event EventHandler UpdateConnectionComplete;

        [Category("XrmToolbox")]
        [Description("Event that fires when an Exception occours")]
        public event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

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

        #region Public methods
        /// <summary>
        /// Clear all loaded data in your control
        /// </summary>
        public void ClearData()
        {
            SelectedEntity = null;
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(null));

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
            AllEntities = new List<EntityMetadata>();

            // set up some vars
            _parent = parent;
            _service = service;

            InitializeComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Load the Entities using the current IOrganizationService.
        /// The call is asynchronous and will leverage the WorkAsync object on the parent XrmToolbox control
        /// </summary>
        public void LoadData()
        {
            LoadData(true);
        }

        /// <summary>
        /// Private method that will rethrow an Exception if specified in the parameter.  
        /// This is meant to allow for external programmatic calls to load vs those from the built in controls
        /// </summary>
        /// <param name="throwException">Flag indicating whether to rethrow a captured exception</param>
        private void LoadData(bool throwException)
        {
            if ((_service == null) || (_parent == null))
            {
                var ex = new InvalidOperationException("Both the Service and Parent references must be set before loading the Entities list");

                // raise the error event and if set, throw error
                ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs(ex.Message, ex));

                if (throwException) {
                    throw ex;
                }
                return;
            }

            ToggleMainControlsEnabled(false);

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
                    catch (System.ServiceModel.FaultException ex)
                    {
                        e.Result = ex;
                    }
                },
                ProgressChanged = e => {
                    ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(e.ProgressPercentage, e.UserState.ToString()));
                },
                PostWorkCallBack = e => {
                    if (e.Result is Exception)
                    {
                        RaiseErrorMessage($"An error occured attetmpting to load the list of Entities", (Exception)e.Result);

                        if (throwException) {
                            throw (Exception)e.Result;
                        }
                    }
                    else
                    {
                        var entities = (List<EntityMetadata>)e.Result;

                        LoadDataComplete?.Invoke(this, new EventArgs());

                        AllEntities = entities;

                        LoadComboItems();
                    }
                }
            });
        }

        /// <summary>
        /// Handle the updated connection 
        /// </summary>
        /// <param name="newService">Reference to the new IOrganizationService</param>
        public void UpdateConnection(IOrganizationService newService)
        {
            // if the service had previously been set, then clear things out
            if (_service != null) {
                ClearData();
            }

            _service = newService;

            // if the auto load is set, now is the time to reload!
            if (_autoLoadData)
            {
                LoadData(true);
            }

            ToggleMainControlsEnabled();

            UpdateConnectionComplete?.Invoke(this, new EventArgs());
        }
        #endregion

        #region Private helper methods

        /// <summary>
        /// Toggle the main display based on the status of the overall Enbabled state or whether the service has been set
        /// </summary>
        private void ToggleMainControlsEnabled() {

            var enabled = (Enabled) ? _service != null : Enabled;

            ToggleMainControlsEnabled(enabled);
        }

        /// <summary>
        /// Toggle main ui elements disabled while we show the results
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleMainControlsEnabled(bool enabled)
        {
            buttonReload.Enabled = enabled;
            comboEntities.Enabled = enabled;
        }

        /// <summary>
        /// Load the list of Entities into the Combo control
        /// </summary>
        private void LoadComboItems()
        {
            comboEntities.SuspendLayout();

            ClearData();

            var items = from ent in AllEntities
                        select new ComboItem(CrmActions.GetLocalizedLabel(ent.DisplayName, ent.SchemaName), ent.SchemaName, ent);

            comboEntities.DataSource = items.OrderBy(e => e.DisplayName).ToList();
            comboEntities.DisplayMember = "DisplayName";
            comboEntities.ValueMember = "Value";

            if (comboEntities.Items.Count > 0)
            {
                comboEntities.SelectedIndex = 0;
            }

            comboEntities.ResumeLayout();

            ToggleMainControlsEnabled(true);
        }


        /// <summary>
        /// Helper method to raise the ErrorOccured event to the client
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        private void RaiseErrorMessage(string message, Exception ex)
        {
            ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs(message, ex));
        }

        #endregion


        #region Control event handlers

        private void ComboEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedEntity = null;

            if (comboEntities.SelectedItem is ComboItem item)
            {
                SelectedEntity = item.EntityMeta;
            }

            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedEntity));
        }

        private void ButtonReload_Click(object sender, EventArgs e)
        {
            LoadData(false);
        }
        #endregion

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

    }
}
