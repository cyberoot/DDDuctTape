using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FolderSync.Library
{
    public enum FolderSynchronizationItemFolderOption
    {
        CreateFolder, NoOperation
    }

    public class FolderSynchronizationItemFolder : FileOperation
    {
        protected string _SourceFileName = string.Empty;
        protected string _DestinationFileName = string.Empty;
        protected FolderSynchronizationItemFolderOption _Option = FolderSynchronizationItemFolderOption.NoOperation;

        #region Properties
        public string SourceFileName
        {
            get { return _SourceFileName; }
            set { _SourceFileName = value; }
        }

        public string DestinationFileName
        {
            get { return _DestinationFileName; }
            set { _DestinationFileName = value; }
        }

        public FolderSynchronizationItemFolderOption Option
        {
            get { return _Option; }
            set { _Option = value; }
        }
        #endregion

        public FolderSynchronizationItemFolder(string foldername, FolderSynchronizationItemFolderOption option)
        {
            _SourceFileName = foldername;
            _Option = option;
        }

        public override void DoOperation()
        {
            if (_Option == FolderSynchronizationItemFolderOption.CreateFolder)
            {
                if (Directory.Exists(_SourceFileName) == false)
                { Directory.CreateDirectory(_SourceFileName); }
            }
        }
    }
}
