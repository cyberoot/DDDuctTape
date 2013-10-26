using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FolderSync.Library
{
    public static class PathExtension
    {
        public static int LastIndexOf(string path, char ch, int lastPathCount)
        {
            path = GetPathNameFix(path);

            int last = path.LastIndexOf(ch);
            if (last == -1 || last == 0) return last;
            
            if (lastPathCount == 0) return last;
            while(lastPathCount != 0)
            {
                path = path.Substring(0, last);
                last = path.LastIndexOf(ch);

                if (last == -1 || last == 0) return last;
                lastPathCount--;
            }
            return last;
        }

        public static string GetLastPathName(this string fullpath, int lastPathCount)
        {
            fullpath = GetPathNameFix(fullpath);

            int uncIndex = LastIndexOf(fullpath, '/', lastPathCount);
            int comIndex = LastIndexOf(fullpath, '\\', lastPathCount);
            int result = uncIndex;
            if (comIndex > uncIndex) result = comIndex;

            if (result == fullpath.Length -1) return string.Empty;

            return fullpath.Substring(result+1);
        }

        public static string GetPathNameFix(string path)
        {
            if (path.EndsWith("\\")) path = path.Substring(0, path.Length - 1);
            else if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
            return path;
        }
    }
}
