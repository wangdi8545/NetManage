using System;

namespace Nimo.Tools
{
	public class CheckFrequencyFilter
	{
		private long lastCheckTime, interval, count, MaxCount;


		public CheckFrequencyFilter (long _interval, int _MaxCount)
		{
			MaxCount = _MaxCount;
			count = 0;
			interval = _interval;
			lastCheckTime = System.DateTime.Now.Ticks / 10000;
		}

		public void ReSetLastCheckTime (long t)
		{
			lastCheckTime = t;
		}

		public void check ()
		{
			long nowTime = System.DateTime.Now.Ticks / 10000;
			if (nowTime - lastCheckTime > interval) {
				if (count > MaxCount) {
					// Form last 1s, check for more then 10 times
					Debuger.Log("check Frequency more then 10 times");
				}

				lastCheckTime = nowTime;
				count = 0;
			} else {
				count++;
			}
		}
	}
}

