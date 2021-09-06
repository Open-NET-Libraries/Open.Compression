using Xunit;

namespace Open.Compression.Tests
{
	public class StringSmokeTests
	{
		[Theory]
		[InlineData("Hello World.")]
		[InlineData("The quick brown fox jumped over the lazy dog.")]
		[InlineData("!@#$%^&*((***&&&*((())){}|}")]
		public void ToStringTest(string value)
		{
			var compressed = Gzip.CompressToString(value);
			var uncompressed = Gzip.DecompressToString(compressed);
			Assert.Equal(value, uncompressed);
		}

		[Theory]
		[InlineData("Hello World.")]
		[InlineData("The quick brown fox jumped over the lazy dog.")]
		[InlineData("!@#$%^&*((***&&&*((())){}|}")]
		public void BasicByteArrayTest(string value)
		{
			var compressed = Gzip.Compress(value);
			var uncompressed = Gzip.DecompressToString(compressed);
			Assert.Equal(value, uncompressed);
		}
	}
}
