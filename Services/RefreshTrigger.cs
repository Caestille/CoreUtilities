﻿using System;
using System.Threading;

namespace CoreUtilities.Services
{
	/// <summary>
	/// Service which invokes an <see cref="Action"/> once after a refresh time if not kept alive.
	/// </summary>
	public class RefreshTrigger
	{
		private bool refreshed;
		private bool block;
		private bool run = true;
		private Action refreshAction;

		/// <summary>
		/// Initializes a new instance of the <see cref="RefreshTrigger"/> class.
		/// Sets the action to invoke and the refresh time.
		/// </summary>
		/// <param name="callback">The <see cref="Action"/> to invoke.</param>
		/// <param name="refreshTimeMs">The refresh time (ms) after which the action is invoked if not refreshed.
		/// </param>
		public RefreshTrigger(Action callback, int refreshTimeMs)
		{
			if (refreshTimeMs <= 0)
			{
				refreshAction = callback;
				return;
			}

			refreshAction = () => refreshed = true;

			Thread thread = new Thread(new ThreadStart(() =>
			{
				while (run)
				{
					if (!refreshed && !block)
					{
						callback();
						block = true;
					}

					if (refreshed)
					{
						refreshed = false;
						block = false;
					}

					Thread.Sleep(refreshTimeMs);
				}
			}));
			thread.Start();
		}

		/// <summary>
		/// Refreshes the trigger, preventing the action from being invoked for the refresh time period once more.
		/// </summary>
		public void Refresh()
		{
			refreshAction();
		}

		/// <summary>
		/// Stops the service, killing the thread checking for the refresh.
		/// </summary>
		public void Stop()
		{
			run = false;
		}
	}
}