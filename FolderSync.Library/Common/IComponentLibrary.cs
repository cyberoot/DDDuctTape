using System.Collections.Generic;

namespace FolderSync.Library.Common
{
    public interface IComponentLibrary<T>
    {
        IList<T> Components { get; }
    }
}
