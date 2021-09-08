namespace DateTimeExcept.Tests
{
	using System.ComponentModel.DataAnnotations;

	using NUnit.Framework;

	public class EnsureInDurationTests
	{
		[Test]
		public void EnsureInDuration_區段在limit之間_不必變動()
		{
			var source = DateTimeDurationFactory.Create(8, 2, 5);
			var limit = DateTimeDurationFactory.Create(8, 1, 20);

			var actual = source.EnsureInDuration(limit);
			Assert.AreEqual(source, actual);
		}

		[Test]
		public void EnsureInDuration_區段begin比limit小_裁切前段()
		{
			var source = DateTimeDurationFactory.Create(8, 1, 5);
			var limit = DateTimeDurationFactory.Create(8, 3, 20);
			var excepted = DateTimeDurationFactory.Create(8, 3, 5);

			var actual = source.EnsureInDuration(limit);
			Assert.AreEqual(excepted, actual);
		}

		[Test]
		public void EnsureInDuration_區段end比limit大_裁切後段()
		{
			var source = DateTimeDurationFactory.Create(8, 15, 25);
			var limit = DateTimeDurationFactory.Create(8, 1, 20);
			
			var excepted = DateTimeDurationFactory.Create(8, 15, 20);

			var actual = source.EnsureInDuration(limit);
			Assert.AreEqual(excepted, actual);
		}
	}
}