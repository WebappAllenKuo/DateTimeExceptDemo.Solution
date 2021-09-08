namespace DateTimeExcept.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	using NUnit.Framework.Constraints;

	public struct DateTimeDuration
	{
		public DateTime Begin { get; set; }

		public DateTime End { get; set; }

		public DateTimeDuration(DateTime begin, DateTime end)
		{
			this.Begin = begin;
			this.End = end;
		}

		public override int GetHashCode() => HashCode.Combine(Begin, End);

		public override bool Equals(object? obj)
		{
			DateTimeDuration x = this;
			DateTimeDuration y = (DateTimeDuration)obj;
			return x.Begin == y.Begin && x.End == y.End;
		}

		public override string ToString()
		{
			return $"{Begin:MM/dd} - {End:MM/dd}";
		}
	}

	public static class DateTimeDurationExts
	{
		/// <summary>
		/// 將 source根據 excludeItems 切割成多段,並排除 excludeItems 部份
		/// </summary>
		/// <param name="source">原始區段</param>
		/// <param name="excludeItems">處理過,完全不會有重疊的區段,如果範圍超過source的起迄,也請自行先刪除</param>
		/// <returns></returns>
		public static IEnumerable<DateTimeDuration> Except(
			this DateTimeDuration source,
			IEnumerable<DateTimeDuration> excludeItems)
		{
			if (excludeItems == null) return new List<DateTimeDuration> {source};

			// 確保每一個excludeItems都在 source區段內
			var excludeItems2 = excludeItems.ToArray();
			for (int i = 0; i < excludeItems2.Length; i++)
			{
				excludeItems2[i] = excludeItems2[i].EnsureInDuration(source);
			}

			// 將excludeItems重疊的合併
			var excludeItems3 = excludeItems2.Dedupe();


			// 開始進行區段裁切
			var result = new List<DateTimeDuration>();
			foreach (var duration in excludeItems3.OrderBy(x => x.Begin))
			{
				if (source.Begin < duration.Begin)
				{ // 切出左側一段
					var newItem = new DateTimeDuration(source.Begin, duration.Begin);
					result.Add(newItem);
				}
				
				// 將source 根據duration切掉
				source = new DateTimeDuration(duration.End, source.End);
				
				if(source.Begin>=source.End)break; // 切完了,離開迴圈
			}

			if (source.Begin < source.End) result.Add(source);

			return result;
		}

		/// <summary>
		/// 確保source的前後範圍不會超過 limit
		/// </summary>
		/// <param name="source"></param>
		/// <param name="limit"></param>
		/// <returns></returns>
		public static DateTimeDuration EnsureInDuration(this DateTimeDuration source, DateTimeDuration limit)
		{
			var begin = source.Begin <= limit.Begin ? limit.Begin : source.Begin;
			var end = source.End >= limit.End ? limit.End : source.End;

			return new DateTimeDuration(begin, end);
		}

		/// <summary>
		/// 將重覆的區段合併成一段
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<DateTimeDuration> Dedupe(this IEnumerable<DateTimeDuration> source)
		{
			if (source == null) return Enumerable.Empty<DateTimeDuration>();
			if (source.Count() <= 1) return source;

			var result = new List<DateTimeDuration>();
			DateTimeDuration? currentItem = null;
			foreach (var duration in source.OrderBy(x=>x.Begin))
			{
				if (currentItem == null)
				{
					currentItem = duration;
					continue;
				}
				//duration的begin在currentItem的範圍外,就將currentItem加進result
				if (duration.Begin > currentItem.Value.End)
				{
					result.Add(currentItem.Value);
					currentItem = duration;
					continue;
				}

				// duration的begin落在 currentItem範圍內,就合併
				if (duration.Begin <= currentItem.Value.End)
				{
					currentItem = currentItem.Value.Merge(duration);
					continue;
				}
			}
			result.Add(currentItem.Value);
			return result;
		}

		private static DateTimeDuration Merge(this DateTimeDuration source, DateTimeDuration others)
		{
			var begin = source.Begin;
			var end = others.End <= source.End ? source.End : others.End;
			return new DateTimeDuration(begin, end);
		}
	}
}