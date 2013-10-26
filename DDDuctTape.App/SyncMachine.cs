using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSync.Library;
using FolderSync.Library.Common;
using FolderSync.Library.Comparer;
using Microsoft.Practices.Unity;

namespace DDDuctTape.App
{
    public class SyncMachine
    {
        private FolderSynchronization _Sync;

        public FolderSynchronization Queue
        {
            get { return _Sync; }
            private set { _Sync = value; }
        }

        protected IUnityContainer IoC { get; set; }

        public SyncMachine(IUnityContainer ioc)
        {
            IoC = ioc;
            _Sync = ioc.Resolve<FolderSynchronization>();
        }

        public void CopyMissingFilesByMask(string source, string destination, IEnumerable<string> sourceSyncMask = null)
        {
            _Sync.Start();

            var fssi = IoC.Resolve<FolderSynchronizationScannerItem>();

            fssi.Source = source;

            fssi.Destination = destination;

            fssi.Option = FolderSynchorizationOption.Destination;

            fssi.Monitor = true;

            _Sync.AddScan(fssi);
        }

        public void InjectDefaultDependencies()
        {
            IoC.RegisterType<IFileComparer, NullSyncComparer>()
                .RegisterType(typeof(IComponentLibrary<>), typeof(FixedComponentLibrary<>));
        }

    }
}
