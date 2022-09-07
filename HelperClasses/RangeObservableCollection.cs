using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CoreUtilities.HelperClasses
{
	/// <summary>
	/// Extenesion of <see cref="ObservableCollection{T}"/>. Raises <see cref="INotifyCollectionChanged"/> when the
	/// collection is modified, as well as allowing addition of multiple objects.
	/// </summary>
	/// <typeparam name="T">The data type to be stored.</typeparam>
	public class RangeObservableCollection<T> : ObservableCollection<T>
	{
		private bool suppressNotification;

		/// <summary>
		/// Constructor, creates and empty collection.
		/// </summary>
		public RangeObservableCollection() { }

		/// <summary>
		/// Constructor, creates a collection with the data from a given list.
		/// </summary>
		/// <param name="list">The list to initialise with.</param>
		public RangeObservableCollection(IEnumerable<T> list)
		{
			AddRange(list);
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (!suppressNotification)
				base.OnCollectionChanged(e);
		}

		/// <summary>
		/// Adds a range of items to the underlying collection.
		/// </summary>
		/// <param name="list">The <see cref="IEnumerable{T}"/> of items to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if the given list is null.</exception>
		public void AddRange(IEnumerable<T> list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));

			suppressNotification = true;

			foreach (T item in list)
				Add(item);

			suppressNotification = false;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		/// <summary>
		/// Removes a range of items from the underlying collection.
		/// </summary>
		/// <param name="list">The <see cref="IEnumerable{T}"/> of items to remove.</param>
		/// <exception cref="ArgumentNullException">Thrown if the given list is null.</exception>
		public void RemoveRange(IEnumerable<T> list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));

			suppressNotification = true;

			foreach (T item in list)
				Remove(item);

			suppressNotification = false;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		/// <summary>
		/// Forcibly raises <see cref="INotifyCollectionChanged"/> even if nothing has changed. Useful in some WPF
		/// contexts.
		/// </summary>
		public void ForceRaiseCollectionChanged()
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}
}