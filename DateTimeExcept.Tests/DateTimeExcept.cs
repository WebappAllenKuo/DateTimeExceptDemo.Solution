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
		/// �N source�ھ� excludeItems ���Φ��h�q,�ñư� excludeItems ����
		/// </summary>
		/// <param name="source">��l�Ϭq</param>
		/// <param name="excludeItems">�B�z�L,�������|�����|���Ϭq,�p�G�d��W�Lsource���_��,�]�Цۦ���R��</param>
		/// <returns></returns>
		public static IEnumerable<DateTimeDuration> Except(
			this DateTimeDuration source,
			IEnumerable<DateTimeDuration> excludeItems)
		{
			if (excludeItems == null) return new List<DateTimeDuration> {source};

			// �T�O�C�@��excludeItems���b source�Ϭq��
			var excludeItems2 = excludeItems.ToArray();
			for (int i = 0; i < excludeItems2.Length; i++)
			{
				excludeItems2[i] = excludeItems2[i].EnsureInDuration(source);
			}

			// �NexcludeItems���|���X��
			var excludeItems3 = excludeItems2.Dedupe();


			// �}�l�i��Ϭq����
			var result = new List<DateTimeDuration>();
			foreach (var duration in excludeItems3.OrderBy(x => x.Begin))
			{
				if (source.Begin < duration.Begin)
				{ // ���X�����@�q
					var newItem = new DateTimeDuration(source.Begin, duration.Begin);
					result.Add(newItem);
				}
				
				// �Nsource �ھ�duration����
				source = new DateTimeDuration(duration.End, source.End);
				
				if(source.Begin>=source.End)break; // �����F,���}�j��
			}

			if (source.Begin < source.End) result.Add(source);

			return result;
		}

		/// <summary>
		/// �T�Osource���e��d�򤣷|�W�L limit
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
		/// �N���Ъ��Ϭq�X�֦��@�q
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
				//duration��begin�bcurrentItem���d��~,�N�NcurrentItem�[�iresult
				if (duration.Begin > currentItem.Value.End)
				{
					result.Add(currentItem.Value);
					currentItem = duration;
					continue;
				}

				// duration��begin���b currentItem�d��,�N�X��
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