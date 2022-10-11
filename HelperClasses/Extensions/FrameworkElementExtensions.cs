using System.Collections.Generic;
using System.Windows;

namespace CoreUtilities.HelperClasses.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="FrameworkElement"/>s.
    /// </summary>
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Gets a <see cref="List{T}"/> of all <see cref="FrameworkElement"/>s belonging to a parent
        /// <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="FrameworkElement"/> to get the children of.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="FrameworkElement"/>s.</returns>
        public static List<FrameworkElement> GetLogicalElements(this object parent)
        {
            var list = new List<FrameworkElement>();
            if (parent == null)
                return list;

            if (parent.GetType().IsSubclassOf(typeof(FrameworkElement)))
                list.Add((FrameworkElement)parent);

            var doParent = parent as DependencyObject;
            if (doParent == null)
                return list;

            foreach (object child in LogicalTreeHelper.GetChildren(doParent))
            {
                list.AddRange(child.GetLogicalElements());
            }

            return list;
        }
    }
}
