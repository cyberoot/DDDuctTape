using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;

namespace DDDuctTape.ViewModel
{
    public class HomeViewModel : NotifyPropertyChanged
    {
        public RelayCommand BrowseCommand { get; private set; }

        public string CdAppsPath { get; set; }
        public string InitData { get; set; }
        public string DdApps { get; set; }
        public string SiteData { get; set; }

        public HomeViewModel()
        {
            BrowseCommand = new RelayCommand(o => DoBrowse(ref o));
            CdAppsPath = "";
            InitData = "";
            DdApps = "";
            SiteData = "";
        }

        private void DoBrowse(ref object o)
        {
            //ModernDialog.ShowMessage("What's up?", "test", MessageBoxButton.OK);
            var dialog = new FolderBrowserDialog {ShowNewFolderButton = false};
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (o is System.Windows.Controls.TextBox)
                {
                    (o as System.Windows.Controls.TextBox).Text = dialog.SelectedPath;
                }
            }
        }

    }
}
