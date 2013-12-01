using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDuctTape.App.Core
{
    public class MaintenanceProgress
    {
        public int Progress { get; set; }
        public IList<string> Errors { get; set; }
        public IList<string> Bads { get; set; }
        public IList<string> Messages { get; set; }
        public int QueueLength { get; set; }
        public int QueueComplete { get; set; }
    }
}
