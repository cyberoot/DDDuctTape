using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FolderSync.Library.Common;

namespace FolderSync.Library
{
    public class MultiFileComparer<T>
    {
        protected IComponentLibrary<T> _ComponentList = null;

        public MultiFileComparer(IComponentLibrary<T> _componentList)
        {
            _ComponentList = _componentList;
        }
        
        public int Compare(FileInfo file1, FileInfo file2)
        {
            int result = 0;
            if (_ComponentList == null || _ComponentList.Components.Count <= 0) return result;
            foreach (IFileComparer _comparer in _ComponentList.Components)
            {
                result = result + _comparer.Compare(file1, file2);
            }
            return result;
        }
    }
}
