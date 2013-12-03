using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;
using System.Configuration;
using Microsoft.Practices.Unity;
using DDDuctTape.App.Core;
using System.Windows.Threading;

namespace DDDuctTape.ViewModel
{
    public class HomeViewModel : NotifyPropertyChanged
    {
        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand Pedal8Command { get; private set; }
        public RelayCommand Pedal9Command { get; private set; }
        public RelayCommand Pedal10Command { get; private set; }
        public RelayCommand Pedal11Command { get; private set; }
        public RelayCommand Pedal12Command { get; private set; }
        public RelayCommand Pedal13Command { get; private set; }
        public RelayCommand Pedal13bCommand { get; private set; }
        public RelayCommand Pedal14Command { get; private set; }
        public RelayCommand Pedal15Command { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand RepairCommand { get; private set; }


        private SyncMachine Sync { get; set; }

        private UnityContainer ioc;

        private DispatcherTimer dispatcherTimer;

        private bool _reportError;

        #region Paths

        private IDictionary<string, string> workPaths = new Dictionary<string, string>()
        {
            {"CdAppsPath", ""},
            {"InitData", ""},
            {"DdApps", ""},
            {"SiteData", ""},
        };

        public string CdAppsPath
        {
            get
            {
                return workPaths["CdAppsPath"];
            }
            set
            {
                workPaths["CdAppsPath"] = value;
                SavePathIfChecked("CdAppsPath", workPaths["CdAppsPath"]);
                OnPropertyChanged("CdAppsPath");
            }
        }

        public string InitData
        {
            get
            {
                return workPaths["InitData"];
            }
            set
            {
                workPaths["InitData"] = value;
                SavePathIfChecked("InitData", workPaths["InitData"]);
                OnPropertyChanged("InitData");
            }
        }

        public string DdApps
        {
            get
            {
                return workPaths["DdApps"];
            }
            set
            {
                workPaths["DdApps"] = value;
                SavePathIfChecked("DdApps", workPaths["DdApps"]);
                OnPropertyChanged("DdApps");
            }
        }

        public string SiteData
        {
            get
            {
                return workPaths["SiteData"];
            }
            set
            {
                workPaths["SiteData"] = value;
                SavePathIfChecked("SiteData", workPaths["SiteData"]);
                OnPropertyChanged("SiteData");
            }
        }

        #endregion

        #region Status icons

        private IDictionary<string, ImageSource> imgStatus = new Dictionary<string, ImageSource>()
        {
            {"CdAppsPath", GetStatusImage("Critical")},
            {"InitData", GetStatusImage("Critical")},
            {"DdApps", GetStatusImage("Critical")},
            {"SiteData", GetStatusImage("Critical")},
        };

        public ImageSource ImgStatusCdAppsPath
        {
            get
            {
                return imgStatus["CdAppsPath"];
            }
            set
            {
                imgStatus["CdAppsPath"] = value;
                OnPropertyChanged("ImgStatusCdAppsPath");
            }
        }
        public ImageSource ImgStatusInitData
        {
            get
            {
                return imgStatus["InitData"];
            }
            set
            {
                imgStatus["InitData"] = value;
                OnPropertyChanged("ImgStatusInitData");
            }
        }
        public ImageSource ImgStatusDdApps
        {
            get
            {
                return imgStatus["DdApps"];
            }
            set
            {
                imgStatus["DdApps"] = value;
                OnPropertyChanged("ImgStatusDdApps");
            }
        }
        public ImageSource ImgStatusSiteData
        {
            get
            {
                return imgStatus["SiteData"];
            }
            set
            {
                imgStatus["SiteData"] = value;
                OnPropertyChanged("ImgStatusSiteData");
            }
        }

        #endregion

        #region RememeberPaths

        private bool _rememberPaths = false;

        public bool RememeberPaths
        {
            get
            {
                return _rememberPaths;
            }
            set
            {
                _rememberPaths = value;
                SetSetting("RememberPaths", _rememberPaths ? "1" : "0");
                if (_rememberPaths)
                {
                    CdAppsPath = CdAppsPath;
                    InitData = InitData;
                    DdApps = DdApps;
                    SiteData = SiteData;
                }
                OnPropertyChanged("RememeberPaths");
            }
        }

        #endregion

        #region Other bindings

        private string _statusText;

        public string StatusText
        {
            get
            {
                return _statusText;
            }
            set
            {
                _statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        private string _resultsText = "";

        public string ResultsText
        {
            get
            {
                return _resultsText;
            }
            set
            {
                _resultsText = value;
                OnPropertyChanged("ResultsText");
            }
        }

        private string _summaryText = "";

        public string SummaryText
        {
            get
            {
                return _summaryText;
            }
            set
            {
                _summaryText = value;
                OnPropertyChanged("SummaryText");
            }
        }

        private bool _controlButtonsEnabled = false;

        public bool ControlButtonsEnabled
        {
            get
            {
                return _controlButtonsEnabled;
            }
            set
            {
                _controlButtonsEnabled = value;
                OnPropertyChanged("ControlButtonsEnabled");
            }
        }

        private Visibility _stopButtonVisible = Visibility.Collapsed;

        public Visibility StopButtonVisible
        {
            get
            {
                return _stopButtonVisible;
            }
            set
            {
                _stopButtonVisible = value;
                OnPropertyChanged("StopButtonVisible");
            }
        }

        private bool _browseButtonsEnabled = true;

        public bool BrowseButtonsEnabled
        {
            get
            {
                return _browseButtonsEnabled;
            }
            set
            {
                _browseButtonsEnabled = value;
                OnPropertyChanged("BrowseButtonsEnabled");
            }
        }

        private int _currentOperationProgress = 0;

        public int CurrentOperationProgress
        {
            get
            {
                return _currentOperationProgress;
            }
            set
            {
                _currentOperationProgress = value;
                OnPropertyChanged("CurrentOperationProgress");
            }
        }

        private Visibility _repairControlsVisible = Visibility.Collapsed;

        public Visibility RepairControlsVisible
        {
            get
            {
                return _repairControlsVisible;
            }
            set
            {
                _repairControlsVisible = value;
                OnPropertyChanged("RepairControlsVisible");
            }
        }

        #endregion

        #region Other instance vars

        private IDictionary<string, short> checkFlags = new Dictionary<string, short>()
        {
            {"CdAppsPath", 0},
            {"InitData", 0},
            {"DdApps", 0},
            {"SiteData", 0},
        };

        private CancellationTokenSource cancellationToken;

        #endregion

        public HomeViewModel()
        {
            BrowseCommand = new RelayCommand(o => DoBrowse(ref o));
            Pedal8Command = new RelayCommand(o => Pedal8Action());
            Pedal9Command = new RelayCommand(o => Pedal9Action());
            Pedal10Command = new RelayCommand(o => Pedal10Action());
            Pedal11Command = new RelayCommand(o => Pedal11Action());
            Pedal12Command = new RelayCommand(o => Pedal12Action());
            Pedal13Command = new RelayCommand(o => Pedal13Action());
            Pedal13bCommand = new RelayCommand(o => Pedal13bAction());
            Pedal14Command = new RelayCommand(o => Pedal14Action());
            Pedal15Command = new RelayCommand(o => Pedal15Action());
            CancelCommand = new RelayCommand(o => CancelAction());
            RepairCommand = new RelayCommand(o => RepairAction());

            ImgStatusCdAppsPath = GetStatusImage("Critical");
            ImgStatusInitData = GetStatusImage("Critical");
            ImgStatusDdApps = GetStatusImage("Critical");
            ImgStatusSiteData = GetStatusImage("Critical");
            CheckPathsOnStartup();
            ioc = new UnityContainer();
            Sync = ioc.Resolve<SyncMachine>();

            PropertyChanged += HomeViewModel_PropertyChanged;

        }

        void HomeViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (new[] { "CdAppsPath", "DdApps", "InitData", "SiteData" }.Contains(e.PropertyName))
            {
                UpdatePathStatus(e.PropertyName, workPaths[e.PropertyName]);
            }
        }

        #region Pedal actions

        private async void Pedal8Action()
        {
            ControlButtonsEnabled = false;
            StopButtonVisible = Visibility.Visible;
            var progressReporter = new Progress<MaintenanceProgress>(UpdateStats);
            cancellationToken = new CancellationTokenSource();
            StatusText = "Copying missing files from INITDATA to SITEDATA";
            missingToken = "[color=Red][b]Missing:[/b][/color]\n";
            errorToken = "Errors processing files:\n";
            try
            {
                var filesCopied = await Sync.CopyMissingFilesNoOverWrite(InitData, SiteData, progressReporter, cancellationToken.Token);
                StatusText = "Complete";
                ControlButtonsEnabled = true;
                SummaryText = String.Format("Blanks filled:\t\t{0}\n", filesCopied) + SummaryText;
            }
            catch (OperationCanceledException ex)
            {
                StatusText = "Canceled";
                ControlButtonsEnabled = true;
            }
            log(ResultsText + Environment.NewLine + SummaryText);
            StopButtonVisible = Visibility.Collapsed;
        }

        private async void Pedal9Action()
        {
            ControlButtonsEnabled = false;
            StopButtonVisible = Visibility.Visible;
            var progressReporter = new Progress<MaintenanceProgress>(UpdateStats);
            cancellationToken = new CancellationTokenSource();
            StatusText = "Copying *.CDX files from INITDATA to SITEDATA";
            missingToken = "[color=Red][b]Missing:[/b][/color]\n";
            errorToken = "Errors processing files:\n";
            try
            {
                var filesReplaced = await Sync.Maintenance9(InitData, SiteData, progressReporter, cancellationToken.Token);
                StatusText = "Complete";
                ControlButtonsEnabled = true;
                SummaryText = String.Format("CDX files replaced:\t\t{0}\n", filesReplaced) + SummaryText;
            }
            catch (OperationCanceledException ex)
            {
                StatusText = "Canceled";
                ControlButtonsEnabled = true;
            }
            log(ResultsText + Environment.NewLine + SummaryText);
            StopButtonVisible = Visibility.Collapsed;
        }

        private async void Pedal10Action()
        {
            ControlButtonsEnabled = false;
            StopButtonVisible = Visibility.Visible;
            var progressReporter = new Progress<MaintenanceProgress>(UpdateStats);
            cancellationToken = new CancellationTokenSource();
            StatusText = "Removing DDTEMP.* files from SITEDATA";
            missingToken = "[color=Red][b]Deleted:[/b][/color]\n";
            errorToken = "Errors processing files:\n";
            try
            {
                var filesDeleted = await Sync.Maintenance10(SiteData, progressReporter, cancellationToken.Token);
                StatusText = "Complete";
                ControlButtonsEnabled = true;
                SummaryText = String.Format("DDTEMP files deleted:\t\t{0}\n", filesDeleted) + SummaryText;
            }
            catch (OperationCanceledException ex)
            {
                StatusText = "Canceled";
                ControlButtonsEnabled = true;
            }
            log(ResultsText + Environment.NewLine + SummaryText);
            StopButtonVisible = Visibility.Collapsed;
        }

        private async void Pedal11Action()
        {
            ControlButtonsEnabled = false;
            StopButtonVisible = Visibility.Visible;
            var progressReporter = new Progress<MaintenanceProgress>(UpdateStats);
            cancellationToken = new CancellationTokenSource();
            StatusText = "Copying *WK.* files from INITDATA to SITEDATA";
            missingToken = "[color=Red][b]Missing:[/b][/color]\n";
            errorToken = "Errors processing files:\n";
            try
            {
                var filesReplaced = await Sync.Maintenance11(InitData, SiteData, progressReporter, cancellationToken.Token);
                StatusText = "Complete";
                ControlButtonsEnabled = true;
                SummaryText = String.Format("WK files replaced:\t\t{0}\n", filesReplaced) + SummaryText;
            }
            catch (OperationCanceledException ex)
            {
                StatusText = "Canceled";
                ControlButtonsEnabled = true;
            }
            log(ResultsText + Environment.NewLine + SummaryText);
            StopButtonVisible = Visibility.Collapsed;
        }

        private async void Pedal12Action()
        {
            ControlButtonsEnabled = false;
            StopButtonVisible = Visibility.Visible;
            var progressReporter = new Progress<MaintenanceProgress>(UpdateStats);
            cancellationToken = new CancellationTokenSource();
            StatusText = @"Copying *.RPT files from CDROM\Applications to DD\Applications";
            missingToken = "[color=Red][b]Missing:[/b][/color]\n";
            errorToken = "Errors processing files:\n";
            try
            {
                var filesReplaced = await Sync.Maintenance12(InitData, SiteData, progressReporter, cancellationToken.Token);
                StatusText = "Complete";
                ControlButtonsEnabled = true;
                SummaryText = String.Format("RPT files replaced:\t\t{0}\n", filesReplaced) + SummaryText;
            }
            catch (OperationCanceledException ex)
            {
                StatusText = "Canceled";
                ControlButtonsEnabled = true;
            }
            log(ResultsText + Environment.NewLine + SummaryText);
            StopButtonVisible = Visibility.Collapsed;
        }

        private async void Pedal13Action()
        {
            ControlButtonsEnabled = false;
            StopButtonVisible = Visibility.Visible;
            var progressReporter = new Progress<MaintenanceProgress>(UpdateStats);
            cancellationToken = new CancellationTokenSource();
            StatusText = @"Copying Daily Files Templates from INITDATA to SITEDATA";
            missingToken = "[color=Red][b]Copied:[/b][/color]\n";
            errorToken = "Errors processing files:\n";
            try
            {
                var filesCopied = await Sync.Maintenance13(InitData, SiteData, progressReporter, cancellationToken.Token);
                StatusText = "Complete";
                ControlButtonsEnabled = true;
                SummaryText = String.Format("Files copied:\t\t{0}\n", filesCopied) + SummaryText;
            }
            catch (OperationCanceledException ex)
            {
                StatusText = "Canceled";
                ControlButtonsEnabled = true;
            }
            log(ResultsText + Environment.NewLine + SummaryText);
            StopButtonVisible = Visibility.Collapsed;
        }

        private async void Pedal13bAction()
        {
            ControlButtonsEnabled = false;
            StopButtonVisible = Visibility.Visible;
            var progressReporter = new Progress<MaintenanceProgress>(UpdateStats);
            cancellationToken = new CancellationTokenSource();
            StatusText = "Checking for 0 size files";
            missingToken = "[color=Red][b]Files:[/b][/color]\n";
            errorToken = "Errors processing files:\n";
            try
            {
                var filesFound = await Sync.Maintenance13b(SiteData, progressReporter, cancellationToken.Token);
                StatusText = "Complete";
                ControlButtonsEnabled = true;
                SummaryText = String.Format("Files found:\t\t{0}\n", filesFound.Count()) + SummaryText;
            }
            catch (OperationCanceledException ex)
            {
                StatusText = "Canceled";
                ControlButtonsEnabled = true;
            }
            log(ResultsText + Environment.NewLine + SummaryText);
            StopButtonVisible = Visibility.Collapsed;
        }

        private void Pedal14Action()
        {
            ControlButtonsEnabled = false;
            RepairControlsVisible = Visibility.Visible;
            StatusText = "Start required DD services and hit REPAIR";
        }

        private void Pedal15Action()
        {
            Pedal8Action();
            Pedal9Action();
            Pedal10Action();
            Pedal11Action();
            Pedal12Action();
            Pedal13Action();
            Pedal13bAction();
            Pedal14Action();
        }

        private void RepairAction()
        {
            Sync.RepairStart(DdApps);
            Application.Current.Shutdown();
        }

        private void CancelAction()
        {
            cancellationToken.Cancel();
            StatusText = "Canceling, please wait";
        }

        #endregion

        private string missingToken = "[color=Red][b]Missing:[/b][/color]\n";
        private string errorToken = "Errors processing files:\n";

        private void UpdateStats(MaintenanceProgress progress)
        {
            ResultsText = "";
            CurrentOperationProgress = progress.Progress;
            if (progress.Bads.Count > 0)
            {
                foreach (var bad in progress.Bads)
                {
                    SummaryText += String.Format("{0}[color=Red]{1}[/color]\n", missingToken, bad);
                    missingToken = "";
                }
            }
            if (progress.Errors.Count > 0)
            {
                foreach (var error in progress.Errors)
                {
                    ResultsText += String.Format("{0}{1}\n", errorToken, error);
                    errorToken = "";
                }
            }
        }

        private void log(string text)
        {
            File.AppendAllText(String.Format(".\\{0}_log.txt", DateTime.Now.ToString("MMddyyyy_HHmmss")), DateTime.Now.ToString() + text + Environment.NewLine);
        }

        /// <summary>
        /// Retrieve status resource image
        /// </summary>
        /// <param name="statusCode">Critical or Complete_and_ok</param>
        /// <returns></returns>
        private static ImageSource GetStatusImage(string statusCode)
        {
            return (ImageSource)new ImageSourceConverter().ConvertFromString(String.Format("pack://application:,,/Assets/Media/StatusAnnotations_{0}_16xLG_color.png", statusCode));
        }

        private void DoBrowse(ref object o)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog { ShowNewFolderButton = false };
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (o is string)
                {
                    var actionCode = (o as string).Replace("TextBox", "");
                    UpdatePathStatus(actionCode, dialog.SelectedPath);
                    OnPropertyChanged(actionCode);
                }
            }
        }

        private bool SavePathIfChecked(string key, string value)
        {
            if (GetSetting("RememberPaths") == "1")
            {
                return SetSetting(key, value);
            }
            return false;
        }

        private void CheckPathsOnStartup()
        {
            var pathRemembered = GetSetting("RememberPaths");
            if (pathRemembered == "1")
            {
                _rememberPaths = true;
                var keys = new string[]
                {
                    "CdAppsPath",
                    "InitData",
                    "DdApps",
                    "SiteData",
                };
                foreach (var s in keys)
                {
                    var path = GetSetting(s);
                    if (path != null)
                    {
                        workPaths[s] = path;
                        OnPropertyChanged(path);
                        UpdatePathStatus(s, path);
                    }
                }
                OnPropertyChanged("RememeberPaths");
            }
        }

        private void UpdatePathStatus(string actionCode, string path)
        {
            var fileMustExist = new[] { "CdAppsPath", "DdApps" }.Contains(actionCode) ? "DDWIN.EXE" : "MNUITM.DBF";

            var antiCode = (actionCode == "CdAppsPath")
                ? "DdApps"
                : (actionCode == "DdApps") ? "CdAppsPath" : (actionCode == "InitData") ? "SiteData" : "InitData";

            workPaths[actionCode] = path;

            checkFlags[actionCode] = (short)(File.Exists(String.Format(path + @"\{0}", fileMustExist)) ? 1 : 0);

            if (checkFlags[actionCode] == 1 && checkFlags[antiCode] != 1 && File.Exists(String.Format(workPaths[antiCode] + @"\{0}", fileMustExist)))
            {
                checkFlags[antiCode] = 1;
                imgStatus[antiCode] = GetStatusImage("Complete_and_ok");
                OnPropertyChanged("ImgStatus" + antiCode);
            }

            imgStatus[actionCode] = GetStatusImage(checkFlags[actionCode] == 1 ? "Complete_and_ok" : "Critical");

            if (workPaths["CdAppsPath"] == workPaths["DdApps"])
            {
                imgStatus["CdAppsPath"] = GetStatusImage("Critical");
                imgStatus["DdApps"] = GetStatusImage("Critical");
                checkFlags["CdAppsPath"] = 0;
                checkFlags["DdApps"] = 0;
                OnPropertyChanged("ImgStatusCdAppsPath");
                OnPropertyChanged("ImgStatusDdApps");
            }
            if (workPaths["InitData"] == workPaths["SiteData"])
            {
                imgStatus["InitData"] = GetStatusImage("Critical");
                imgStatus["SiteData"] = GetStatusImage("Critical");
                checkFlags["InitData"] = 0;
                checkFlags["SiteData"] = 0;
                OnPropertyChanged("ImgStatusInitData");
                OnPropertyChanged("ImgStatusSiteData");
            }
            OnPropertyChanged("ImgStatus" + actionCode);

            ControlButtonsEnabled = checkFlags.Sum(x => x.Value) > 3;
        }

        internal static bool SetSetting(string key, string value)
        {
            bool result = false;
            try
            {
                Configuration config =
                  ConfigurationManager.OpenExeConfiguration(
                                       ConfigurationUserLevel.None);

                config.AppSettings.Settings.Remove(key);
                var kvElem = new KeyValueConfigurationElement(key, value);
                config.AppSettings.Settings.Add(kvElem);

                config.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("appSettings");

                result = true;
            }
            finally
            { }
            return result;
        }

        internal static string GetSetting(string key)
        {
            string result = null;
            try
            {
                result = ConfigurationManager.AppSettings[key];
            }
            finally
            { }
            return result;
        }

    }
}
