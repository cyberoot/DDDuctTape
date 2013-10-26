using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace FolderSync.Library.Common
{
    public class FixedComponentLibrary<T> : IComponentLibrary<T>
    {
        #region Implementation of IComponentLibrary<T>

        private IList<T> _components = new List<T>();

        public IList<T> Components
        {
            get { return _components; }
            private set { _components = value; }
        }

        #endregion

        public FixedComponentLibrary()
        {
        }

        [InjectionConstructor]
        public FixedComponentLibrary(T component)
        {
            Add(component);
        }
        
        public FixedComponentLibrary(IList<T> components)
        {
            Components = components;
        }

        public void Add(T component)
        {
            _components.Add(component);
        }

    }
}
