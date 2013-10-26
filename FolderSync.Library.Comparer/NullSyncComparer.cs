using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync.Library.Comparer
{

    public class NullSyncComparer : IFileComparer
    {
        /// <summary>
        /// Assume any files with identical filenames are equal
        /// </summary>
        /// <param name="file1">First file info</param>
        /// <param name="file2">Second file info</param>
        /// <returns></returns>
        public virtual int Compare(FileInfo sourcefile, FileInfo destinationfile)
        {
            return 0;
        }
    }
}
