using System;
using System.ComponentModel;
using Microsoft.Xrm.Sdk;

using XrmToolBox.Extensibility;

namespace Futurez.XrmToolbox.Controls
{
    /// <summary>
    /// General interface for XrmToolbox helper controls
    /// </summary>
    interface IXrmToolboxControl
    {
        event EventHandler InitializeComplete;
        event EventHandler LoadDataComplete;
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        event EventHandler ClearDataComplete;
        event EventHandler CloseComplete;

        void Initialize(PluginControlBase parent, IOrganizationService Service);

        void LoadData();

        void ClearData();

        void Close();

        void UpdateConnection(IOrganizationService newService);
    }
}