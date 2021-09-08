using NUnit.Framework;

namespace DateTimeExcept.Tests
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;

	public class DateTimeDurationFactory
	{
		public static DateTimeDuration Create(int month, int beginDay, int endDay)
		{
			int year = DateTime.Now.Year;
			var begin = new DateTime(year, month, beginDay);
			var end = new DateTime(year, month, endDay);

			return new DateTimeDuration(begin, end);
		}
	}
	public class DateTimeDurationExtsTests
	{
		private DateTimeDuration F01To20, 
		                         F02To04, F01To02, F04To20,
								 F01To03,F03To20,
								 F05To07, F04To05,F07To20;


		[SetUp]
		public void Setup()
		{
			this.F01To20 = DateTimeDurationFactory.Create(8, 1, 20); // 8/1 ~ 8/20
			this.F02To04 = DateTimeDurationFactory.Create(8, 2, 4); // 8/2 ~ 8/4
			F01To02 = DateTimeDurationFactory.Create(8, 1, 2);
			F04To20 = DateTimeDurationFactory.Create(8, 4, 20);

			F01To03 = DateTimeDurationFactory.Create(8, 1, 3);
			F03To20 = DateTimeDurationFactory.Create(8, 3, 20);

			F05To07 = DateTimeDurationFactory.Create(8, 5,7); 
			F04To05 = DateTimeDurationFactory.Create(8, 4,5); 
			F07To20 = DateTimeDurationFactory.Create(8, 7, 20);
		}

		[Test]
		public void Except_沒有排除區段_傳回本身()
		{
			var source = this.F01To20;
			DateTimeDuration[] excludeItems =null;

			var expected = new DateTimeDuration[] { source };

			var actual = source.Except(excludeItems).ToArray();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Except_有一個區段_在source中央_傳回二段()
		{
			var source = this.F01To20;
			var excludeItems = new DateTimeDuration[] {this.F02To04};

			var expected = new DateTimeDuration[] {this.F01To02,this.F04To20};

			var actual = source.Except(excludeItems).ToArray();

			Assert.AreEqual(2, actual.Length);
			Assert.AreEqual(expected, actual);

		}

		[Test]
		public void Except_有一個區段_開頭相等_傳回一段()
		{
			var source = this.F01To20;
			var excludeItems = new DateTimeDuration[] { this.F01To03 };

			var expected = new DateTimeDuration[] { this.F03To20};

			var actual = source.Except(excludeItems).ToArray();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Except_有一個區段_結尾相等_傳回一段()
		{
			var source = this.F01To20;
			var excludeItems = new DateTimeDuration[] { this.F03To20 };

			var expected = new DateTimeDuration[] { this.F01To03 };

			var actual = source.Except(excludeItems).ToArray();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Except_有一個區段_頭尾相等_傳回空集合()
		{
			var source = this.F01To20;
			var excludeItems = new DateTimeDuration[] { source };

			var actual = source.Except(excludeItems).ToArray();

			Assert.AreEqual(0, actual.Length);
		}

		[Test]
		public void Except_有2個區段_在source中央_傳回3段()
		{
			var source = this.F01To20;
			var excludeItems = new DateTimeDuration[] { this.F02To04, this.F05To07 };

			var expected = new DateTimeDuration[] { this.F01To02, this.F04To05,this.F07To20};

			var actual = source.Except(excludeItems).ToArray();

			Assert.AreEqual(expected, actual);

		}
	}
}