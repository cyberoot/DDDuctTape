using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void DoSync()
        {
            var ioc = new UnityContainer();

            var sync = ioc.Resolve<SyncMachine>();
            sync.CopyMissingFilesNoOverWrite(@"d:\temp\Ы1111ПыЩПыЩ", @"d:\temp\Result");
        }
    }
}
