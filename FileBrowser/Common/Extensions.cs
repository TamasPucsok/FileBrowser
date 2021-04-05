using FileSystemModels;
using Kendo.Mvc.UI;
using System.Collections.Generic;

namespace FileBrowser.Common
{
    public static class Extensions
    {
        public static List<TreeViewItemModel> BuildTreeViewModel(this FileSystemItemBase item)
        {
            List<TreeViewItemModel> result = new List<TreeViewItemModel>();

            foreach (var child in item.Children)
            {
                var treeViewItem = new TreeViewItemModel
                {
                    Id = child.Address + ":" + child.GetPath().GetHashCode(),
                    Text = child.Name,
                    Items = child.BuildTreeViewModel()
                };

                result.Add(treeViewItem);
            }

            return result;
        }
    }
}
