using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace CoreUtilities.Services
{
	/// <summary>
	/// Service which invokes an <see cref="Action"/> once after a refresh time if not kept alive.
	/// </summary>
	public class KeepAliveTriggerService
	{
		private bool hasKeepAliveBeenRefreshed;
		private bool block;
		bool run = true;
		string creator;

		/// <summary>
		/// Constructor for <see cref="KeepAliveTriggerService"/>. Sets the action to invoke and the refresh time.
		/// </summary>
		/// <param name="callback">The <see cref="Action"/> to invoke.</param>
		/// <param name="refreshTimeMs">The refresh time after which the action is invoked if not kept alive (in ms).
		/// </param>
		public KeepAliveTriggerService(Action callback, int refreshTimeMs, [CallerFilePath]string creator = "")
		{
			Thread thread = new Thread(new ThreadStart(() =>
			{
				while (run)
				{
					if (!hasKeepAliveBeenRefreshed && !block)
					{
						callback();
						block = true;
					}

					if (hasKeepAliveBeenRefreshed)
					{
						hasKeepAliveBeenRefreshed = false;
						block = false;
					}

					Thread.Sleep(refreshTimeMs);
				}
			}));
			thread.Start();
			this.creator = creator;
		}

		/// <summary>
		/// Refreshes the service keepAlive, preventing the action from being invoked for the refresh time period once
		/// more.
		/// </summary>
		public void Refresh()
		{
			hasKeepAliveBeenRefreshed = true;
		}

		/// <summary>
		/// Stops the service, killing the thread checking for the keep alive.
		/// </summary>
		public void Stop()
		{
			run = false;
		}
	}
}
