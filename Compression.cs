using Open.Memory;
using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Open.Formatting
{

	public static class Gzip
	{
		private const string TooShortMessage = "Too short.";

		/// <summary>
		/// Returns the compressed version of provided data.
		/// </summary>
		public static TStream Compress<TStream>(Stream source, TStream destination)
			where TStream : Stream
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (destination is null)
				throw new ArgumentNullException(nameof(destination));
			Contract.EndContractBlock();

			using var gs = new GZipStream(destination, CompressionMode.Compress);
			source.CopyTo(gs);
			return destination;
		}

		/// <summary>
		/// Returns the compressed version of provided data.
		/// </summary>
		public static MemoryStream Compress(Stream source)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			Contract.EndContractBlock();

			return Compress(source, new MemoryStream());
		}

		/// <summary>
		/// Returns the compressed version of provided data.
		/// </summary>
		/// <returns>The compressed data stream.</returns>
		public static async ValueTask<TStream> CompressAsync<TStream>(Stream source, TStream destination)
			where TStream : Stream
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (destination is null)
				throw new ArgumentNullException(nameof(destination));
			Contract.EndContractBlock();

			using var gs = new GZipStream(destination, CompressionMode.Compress);
			await source.CopyToAsync(gs).ConfigureAwait(false);
			return destination;
		}

		/// <summary>
		/// Returns the compressed version of provided data.
		/// </summary>
		/// <returns>The compressed data stream.</returns>
		public static ValueTask<MemoryStream> CompressAsync(Stream source)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			Contract.EndContractBlock();

			return CompressAsync(source, new MemoryStream());
		}

		/// <summary>
		/// Returns the compressed version of provided data.
		/// </summary>
		public static byte[] Compress(byte[] data)
		{
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			Contract.EndContractBlock();

			using var msi = new MemoryStream(data);
			using var mso = new MemoryStream();
			return Compress(msi, mso).ToArray();
		}

		/// <summary>
		/// Returns the compressed byte array of provided string.
		/// </summary>
		public static byte[] Compress(string data, Encoding? encoding = null)
		{
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			Contract.EndContractBlock();

			return Compress((encoding ?? Encoding.UTF8).GetBytes(data));
		}

		/// <summary>
		/// Returns the compressed string of provided byte array.
		/// </summary>
		public static string CompressToString(byte[] data, Encoding? encoding = null)
		{
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			Contract.EndContractBlock();

			return (encoding ?? Encoding.UTF8).GetString(Compress(data));
		}

		public static string CompressToString(string text, Encoding? encoding = null)
		{
			if (text is null)
				throw new ArgumentNullException(nameof(text));
			Contract.EndContractBlock();

			var buffer = (encoding ?? Encoding.UTF8).GetBytes(text);
			using var ms = new MemoryStream();
			using (var stream = new GZipStream(ms, CompressionMode.Compress, true))
				stream.Write(buffer, 0, buffer.Length);

			ms.Position = 0;

			var pool = ArrayPool<byte>.Shared;
			using var rawData = pool.RentDisposable(ms.Length, true);
			using var compressedData = pool.RentDisposable(rawData.Length + 4, true);
			ms.Read(rawData.Array, 0, rawData.Length);
			Buffer.BlockCopy(rawData.Array, 0, compressedData.Array, 4, rawData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, compressedData.Array, 0, 4);
			return Convert.ToBase64String(compressedData.Array, 0, rawData.Length + 4);
		}

		/// <summary>
		/// Returns the decompressed version of provided data.
		/// </summary>
		public static TStream Decompress<TStream>(Stream source, TStream destination)
			where TStream : Stream
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (destination is null)
				throw new ArgumentNullException(nameof(destination));
			Contract.EndContractBlock();

			using var gs = new GZipStream(source, CompressionMode.Decompress);
			gs.CopyTo(destination);
			return destination;
		}

		/// <summary>
		/// Returns the decompressed version of provided data.
		/// </summary>
		public static MemoryStream Decompress(Stream source)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			Contract.EndContractBlock();

			return Decompress(source, new MemoryStream());
		}

		/// <summary>
		/// Returns the decompressed version of provided data.
		/// </summary>
		/// <returns>The decompressed data stream.</returns>
		public static async ValueTask<TStream> DecompressAsync<TStream>(Stream source, TStream destination)
			where TStream : Stream
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (destination is null)
				throw new ArgumentNullException(nameof(destination));
			Contract.EndContractBlock();

			using var gs = new GZipStream(source, CompressionMode.Decompress);
			await gs.CopyToAsync(destination).ConfigureAwait(false);
			return destination;
		}

		/// <summary>
		/// Returns the decompressed version of provided data.
		/// </summary>
		/// <returns>The decompressed data stream.</returns>
		public static ValueTask<MemoryStream> DecompressAsync(Stream source)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			Contract.EndContractBlock();

			return DecompressAsync(source, new MemoryStream());
		}

		/// <summary>
		/// Returns the decompressed version of provided data.
		/// </summary>
		public static byte[] Decompress(byte[] data)
		{
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			Contract.EndContractBlock();

			using var msi = new MemoryStream(data);
			using var mso = new MemoryStream();
			return Decompress(msi, mso).ToArray();
		}

		/// <summary>
		/// Returns the decompressed byte array of provided string.
		/// </summary>
		public static byte[] Decompress(string data, Encoding? encoding = null)
		{
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			Contract.EndContractBlock();

			return Decompress((encoding ?? Encoding.UTF8).GetBytes(data));
		}

		/// <summary>
		/// Returns the decompressed string of provided byte array.
		/// </summary>
		public static string DecompressToString(byte[] data, Encoding? encoding = null)
		{
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			Contract.EndContractBlock();

			return (encoding ?? Encoding.UTF8).GetString(Decompress(data));
		}

		/// <summary>
		/// Returns the decompressed string of the compressed text.
		/// </summary>
		public static string DecompressToString(string compressedText, Encoding? encoding = null)
		{
			if (compressedText is null)
				throw new ArgumentNullException(nameof(compressedText));
			Contract.EndContractBlock();

			if (compressedText.Length == 0)
				return string.Empty;

			var compressedData = Convert.FromBase64String(compressedText);
			if (compressedData.Length < 4)
				throw new ArgumentException(TooShortMessage, nameof(compressedText));

			using var ms = new MemoryStream();
			var dataLength = BitConverter.ToInt32(compressedData, 0);
			if (dataLength < 4)
				throw new ArgumentException(TooShortMessage, nameof(compressedText));

			ms.Write(compressedData, 4, compressedData.Length - 4);

			using var B = ArrayPool<byte>.Shared.RentDisposable(dataLength, true);
			var buffer = B.Array;
			ms.Position = 0;
			using (var stream = new GZipStream(ms, CompressionMode.Decompress))
				dataLength = stream.Read(buffer, 0, buffer.Length);

			return (encoding ?? Encoding.UTF8).GetString(buffer, 0, dataLength);
		}

	}
}
