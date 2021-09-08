namespace DateTimeExcept.Tests
{
	using System.Collections.Generic;

	using NUnit.Framework;

	public class DedupeTests
	{
		[Test]
		public void Dedupe_範圍不重覆()
		{
			var f0103 = DateTimeDurationFactory.Create(8, 1, 3);
			var f0506 = DateTimeDurationFactory.Create(8, 5, 6);

			IEnumerable<DateTimeDuration> items = new DateTimeDuration[] {f0103, f0506};

			var actual = items.Dedupe();

			Assert.AreEqual(items, actual);
		}

		[Test]
		public void Dedupe_範圍前後連接_合併成一個區段()
		{
			var f0105 = DateTimeDurationFactory.Create(8, 1, 5);
			var f0508 = DateTimeDurationFactory.Create(8, 5, 8);
			
			var expected = new DateTimeDuration[] { DateTimeDurationFactory.Create(8,1,8)};

			IEnumerable<DateTimeDuration> items = new DateTimeDuration[] { f0105, f0508 };

			var actual = items.Dedupe();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Dedupe_範圍部份重疊_合併成一個區段()
		{
			var f0106 = DateTimeDurationFactory.Create(8, 1, 6);
			var f0508 = DateTimeDurationFactory.Create(8, 5, 8);

			var expected = new DateTimeDuration[] { DateTimeDurationFactory.Create(8, 1, 8) };

			IEnumerable<DateTimeDuration> items = new DateTimeDuration[] { f0106, f0508 };

			var actual = items.Dedupe();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Dedupe_範圍完全包住後者_合併成一個區段()
		{
			var f0116 = DateTimeDurationFactory.Create(8, 1, 16);
			var f0508 = DateTimeDurationFactory.Create(8, 5, 8);

			var expected = new DateTimeDuration[] { f0116 };

			IEnumerable<DateTimeDuration> items = new DateTimeDuration[] { f0116, f0508 };

			var actual = items.Dedupe();

			Assert.AreEqual(expected, actual);
		}
	}
}