using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;

namespace Futurez.XrmToolbox.Controls
{
    /// <summary>
    /// Helper class that will display the Open File Dialog in the Property Grid
    /// thanks to: https://stackoverflow.com/questions/1858960/how-can-i-get-a-propertygrid-to-show-a-savefiledialog
    /// </summary>
    public class OpenFileNameEditor : UITypeEditor
    {
        /// <summary>
        /// Specify the Edit Style for the Property 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Override for the EditValue method for the Property control
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || context.Instance == null || provider == null) {
                return base.EditValue(context, provider, value);
            }

            using (var openDlg = new OpenFileDialog() {
                Title = context.PropertyDescriptor.DisplayName,
                Filter = Constants.GetFileOpenFilter(),
                CheckFileExists = true
            })
            {
                if (value != null) {

                    openDlg.FileName = value.ToString();
                    openDlg.InitialDirectory = Path.GetDirectoryName(value.ToString());
                }
                if (openDlg.ShowDialog() == DialogResult.OK) {
                    value = openDlg.FileName;
                }
            }

            return value;
        }
    }
}
