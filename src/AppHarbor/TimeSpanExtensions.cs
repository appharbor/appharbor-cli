using System;

namespace AppHarbor
{
	public static class TimeSpanExtensions
	{
		public static string GetHumanized(this TimeSpan timeSpan)
		{
			string unit;
			double value;
			if (timeSpan.TotalSeconds < 60)
			{
				unit = "second";
				value = timeSpan.TotalSeconds;
			}
			else if (timeSpan.TotalMinutes < 60)
			{
				unit = "minute";
				value = timeSpan.TotalMinutes;
			}
			else if (timeSpan.TotalHours < 24)
			{
				unit = "hour";
				value = timeSpan.TotalHours;
			}
			else
			{
				unit = "day";
				value = timeSpan.Days;
			}

			var approximateValue = ((int)Math.Round(value / 10.0)) * 10;

			var displayValue = value < 10 ? (int)value : approximateValue;
			unit = displayValue == 1 ? unit : unit + "s";


			return string.Format("{0} {1}", displayValue, unit);
		}
	}
}
