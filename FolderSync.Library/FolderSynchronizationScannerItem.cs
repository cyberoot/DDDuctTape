using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FolderSync.Library
{
    public class FolderSynchronizationScannerItem
    {
        protected string _Source;
        protected string _Destination;
        protected FolderSynchorizationOption _Option;
        protected bool _Monitor;

        public string Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        public string Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }

        public FolderSynchorizationOption Option
        {
            get { return _Option; }
            set { _Option = value; }
        }

        public bool Monitor
        {
            get { return _Monitor; }
            set { _Monitor = value; }
        }
    }
}
