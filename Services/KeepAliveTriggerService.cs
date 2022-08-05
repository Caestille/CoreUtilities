using System;
using System.Threading;

namespace CoreUtilities.Services
{
	public class KeepAliveTriggerService
	{
		private bool hasKeepAliveBeenRefreshed;
		private bool block;
		bool run = true;

		public KeepAliveTriggerService(Action callback, int refreshTimeMs)
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
		}

		public void Refresh()
		{
			hasKeepAliveBeenRefreshed = true;
		}

		public void Stop()
		{
			run = false;
		}
	}
}
