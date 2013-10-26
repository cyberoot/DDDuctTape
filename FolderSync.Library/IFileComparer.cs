using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FolderSync.Library
{
    public interface IFileComparer
    {
        int Compare(FileInfo info1, FileInfo info2);
    }
}
