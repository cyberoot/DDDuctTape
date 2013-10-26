using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FolderSync.Library.Common.Reflection;

namespace FolderSync.Library
{
    public class FileOperation : ReflectionObject, IFileOperation
    {
        public virtual void DoOperation()
        {
        }
    }
}
