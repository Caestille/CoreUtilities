using System.Collections.Generic;

namespace CoreUtilities.HelperClasses
{
	/// <summary>
	/// Extensions for <see cref="IEnumerable{T}"/> classes.
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Deep copy clone of a <see cref="IEnumerable{T}"/>.
		/// </summary>
		/// <typeparam name="T">The data type the <see cref="IEnumerable{T}"/> stores.</typeparam>
		/// <param name="toCopy">The <see cref="IEnumerable{T}"/> to copy.</param>
		/// <returns>A deep copy of the given <see cref="IEnumerable{T}"/>.</returns>
		public static IEnumerable<T> Clone<T>(this IEnumerable<T> toCopy)
		{
			return new List<T>(toCopy);
		}
	}
}