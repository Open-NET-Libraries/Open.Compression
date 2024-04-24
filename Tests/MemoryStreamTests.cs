using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Open.Compression.Tests;
public static class MemoryStreamTests
{
    [Theory]
    [InlineData("Hello World.")]
    [InlineData("The quick brown fox jumped over the lazy dog.")]
    [InlineData("!@#$%^&*((***&&&*((())){}|}")]
    [InlineData("<xml>data</xml>")]
    public static void MemoryStreamRoundTripTest(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var compressed = Gzip.Compress(bytes);
        var decompressed = Gzip.Decompress(compressed);
        Assert.Equal(bytes, decompressed);        
    }
}
