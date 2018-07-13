using System;
using System.ComponentModel;
using Microsoft.Xrm.Sdk;

using XrmToolBox.Extensibility;

namespace Futurez.Xrm.XrmToolbox.Controls
{
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