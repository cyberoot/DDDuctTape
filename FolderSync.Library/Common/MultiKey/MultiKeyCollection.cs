using System;
using System.Collections.Generic;
using System.Linq;
using FolderSync.Library.Common.Reflection;

namespace FolderSync.Library.Common.MultiKey
{
    public class MultiKeyCollection<T>
        where T : ReflectionObject
    {
        protected string[] _Properties = new string[]{};
        protected string _Delimiter = "#";
        protected Dictionary<string, T> _Keys = new Dictionary<string, T>();

        public MultiKeyCollection(string[] properties)
        {
            _Properties = properties;
        }

        #region Properties
        public IList<string> Keys
        {
            get { return _Keys.Keys.ToList<string>(); }
        }

        public IList<T> Objects
        {
            get { return _Keys.Values.ToList<T>(); }
        }
        #endregion

        public string GetKey(T obj)
        {
            string key = string.Empty;
            foreach (string property in _Properties)
            {
                string propValue = obj.GetProperty(property).ToString();
                if (propValue.IndexOf(_Delimiter) >= 0) throw new Exception(string.Format("Cannot create key from this object, property value of this object '{0}' contains '#'.", propValue));
                key += propValue + _Delimiter;
            }
            if (key.EndsWith(_Delimiter) && key.Length != 1) key = key.Substring(0, key.Length - 1);
            return key;
        }        

        public T GetAddObject(T obj)
        {
            string key = GetKey(obj);
            if (_Keys.ContainsKey(key))
                return _Keys[key];
            _Keys.Add(key, obj);
            return obj;
        }

        public T GetObjectByKey(string key)
        {
            if (_Keys.ContainsKey(key))
                return _Keys[key];
            return null;
        }
    }
}
