using System.Windows;
using System.Windows.Media;

namespace CulturezVous.WP7.Utils
{
    /// <summary>
    /// Help to find a child item from view control
    /// </summary>
    /// <remarks>Author : Josh G. http://stackoverflow.com/questions/665719/wpf-animate-listbox-scrollviewer-horizontaloffset </remarks>
    public class VisualChildFinder
    {
        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            // Search immediate children first (breadth-first)
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;

                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
    }
}
