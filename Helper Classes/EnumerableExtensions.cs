﻿using System.Collections.Generic;

namespace CoreUtilities.HelperClasses
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Clone<T>(this IEnumerable<T> toCopy)
		{
			return new List<T>(toCopy);
		}
	}
}