using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FolderSync.Library
{
    public enum FolderSynchronizationItemFileOption
    {
        SourceCreate, SourceOverwrite, DestinationCreate, DestinationOverwrite, NoOperation
    }

    public class FolderSynchronizationItemFile : FileOperation
    {
        protected string _SourceFileName;
        protected string _DestinationFileName;
        protected FolderSynchronizationItemFileOption _Option = FolderSynchronizationItemFileOption.NoOperation;

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

        public FolderSynchronizationItemFileOption Option
        {
            get { return _Option; }
            set { _Option = value; }
        }
        #endregion

        public FolderSynchronizationItemFile(string source, string destination, FolderSynchronizationItemFileOption option)
        {
            _SourceFileName = source;
            _DestinationFileName = destination;
            _Option = option;
        }

        public override void DoOperation()
        {
            if (_Option == FolderSynchronizationItemFileOption.DestinationCreate)
            {
                if (File.Exists(_DestinationFileName) == false)
                { File.Copy(_SourceFileName, _DestinationFileName); }
            }
            else if (_Option == FolderSynchronizationItemFileOption.DestinationOverwrite)
            {
                if (File.Exists(_DestinationFileName) == true)
                { File.Copy(_SourceFileName, _DestinationFileName, true); }
            }
            else if (_Option == FolderSynchronizationItemFileOption.SourceCreate)
            {
                if (File.Exists(_SourceFileName) == false)
                { File.Copy(_DestinationFileName, _SourceFileName); }
            }
            else if (_Option == FolderSynchronizationItemFileOption.SourceOverwrite)
            {
                if (File.Exists(_SourceFileName) == true)
                { File.Copy(_DestinationFileName, _SourceFileName, true); }
            }
        }
    }
}
