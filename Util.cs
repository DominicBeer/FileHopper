using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Grasshopper.Kernel.Special;

namespace FileHopper
{
    internal static class Util
    {
        private static string WildCardToRegex(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }
        public static bool WildCardMatch(this string text, string pattern)
        {
            return Regex.IsMatch(text, WildCardToRegex(pattern));
        }
        public static HashSet<string> GetFolders(List<DirectoryInfo> paths)
        {
            return paths.Where(dir => dir.Exists)
                        .Select(dir => dir.GetDirectories())
                        .SelectMany(subDirs => subDirs)
                        .Select(subDir => subDir.Name)
                        .ToHashSet();
        }
        public static HashSet<string> GetFiles(List<DirectoryInfo> paths)
        {
            return paths.Where(dir => dir.Exists)
                        .Select(dir => dir.GetFiles())
                        .SelectMany(files => files)
                        .Select(file => file.Name)
                        .ToHashSet();
        }

        public static HashSet<string> GetValueListNames(GH_ValueList valueList)
        {
            return valueList.ListItems.Select(item => item.Name).ToHashSet();
        }

        public static void RebuildValueList(GH_ValueList valueList, IEnumerable<(string Key, string Value)> keyValuePairs)
        {
            var selectedNames = valueList.SelectedItems.Select(item => item.Name);

            valueList.ListItems.Clear();
            valueList.SelectedItems.Clear();

            valueList.ListItems.AddRange(keyValuePairs.Select(pair => new GH_ValueListItem(pair.Key, pair.Value)));

            var names = valueList.ListItems.Select(item => item.Name).ToList();
            foreach (var selectedName in selectedNames)
            {
                if (names.Contains(selectedName))
                {
                    var index = names.IndexOf(selectedName);
                    valueList.SelectItem(index);
                }
            }
            if (valueList.SelectedItems.Count == 0)
            {
                valueList.SelectItem(0);
            }

            valueList.Refresh();
        }

        public static string ToValueListString(this string text) => "\"" + text + "\"";

        static void Refresh(this GH_ValueList valueList)
        {
            valueList.Attributes.ExpireLayout();
            valueList.OnAttributesChanged();
            valueList.ExpireSolution(true);
            valueList.OnDisplayExpired(true);
        }
    }
}
