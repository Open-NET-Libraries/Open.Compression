using System.Buffers;
using Xunit;

namespace Open.Compression.Tests
{
	public static class ArrayPoolTests
	{
		[Fact]
		public static void ArrayPoolSizeTest()
		{
			var pool = ArrayPool<int>.Shared;
			const int maxSize = 1024 * 1024;

			var a = pool.Rent(maxSize);
			Assert.True(a.Length >= maxSize);
			pool.Return(a);

			var testSize = maxSize + 1;
			var b = pool.Rent(testSize);
			Assert.True(b.Length >= maxSize);
			pool.Return(b);

			testSize = int.MaxValue / 2;
			var c = pool.Rent(testSize);
			Assert.True(c.Length >= testSize);
			pool.Return(c);
		}
	}
}
