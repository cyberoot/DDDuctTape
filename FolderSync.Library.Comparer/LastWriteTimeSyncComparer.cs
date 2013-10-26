using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FolderSync.Library
{
    public class LastWriteTimeSyncComparer : IFileComparer
    {
        /// <summary>
        /// Compare two fils infos. If the first file has last write date lesser than second file, it will return negative value
        /// If first file has last write date bigger than second file, it will return positive value
        /// If both files last write date is the same, it will return 0
        /// </summary>
        /// <param name="file1">First file info</param>
        /// <param name="file2">Second file info</param>
        /// <returns></returns>
        public virtual int Compare(FileInfo sourcefile, FileInfo destinationfile)
        {
            TimeSpan ts = sourcefile.LastWriteTime - destinationfile.LastWriteTime;
            if (ts.TotalMilliseconds < 0) return -1;
            if (ts.TotalMilliseconds > 0) return 1;
            return 0;
        }
    }
}
