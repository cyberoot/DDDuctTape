using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSync.Library;
using FolderSync.Library.Common;
using FolderSync.Library.Comparer;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace DDDuctTape.App.Tests
{
    [TestFixture]
    public class Sync
    {
        [Test]
        public void DoSync()
        {
            var ioc = new UnityContainer()
                .RegisterType<IFileComparer, NullSyncComparer>()
                .RegisterType(typeof(IComponentLibrary<>), typeof(FixedComponentLibrary<>));

            var sync = ioc.Resolve<SyncMachine>();
            sync.CopyMissingFilesByMask(@"f:\temp\Ы1111ПыЩПыЩ", @"f:\temp\Result", null);

        }
    }
}
