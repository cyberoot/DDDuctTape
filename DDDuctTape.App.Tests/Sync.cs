using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DDDuctTape.App.Core;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace DDDuctTape.App.Tests
{
    [TestFixture]
    public class Sync
    {
        [Test]
        public async void DoSync()
        {
            var ioc = new UnityContainer();

            var sync = ioc.Resolve<SyncMachine>();
            await sync.CopyMissingFilesNoOverWrite(@".\folder1", @".\folder2", null, new CancellationToken());
        }
    }
}
