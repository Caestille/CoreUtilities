﻿using System.Collections.Generic;
using System.Windows;

namespace CoreUtilities.HelperClasses
{
	public static class FrameworkElementExtensions
    {
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
				list.AddRange(GetLogicalElements(child));
			}

			return list;
		}
	}
}
