//using Open.Memory;
using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Open.Compression
{
	public static class Gzip
	{
		private const string TooShortMessage = "Too short.";

		/// <summary>
		/// Returns the compressed version of provided data.
		/// </summary>
		/// <exception cref="ArgumentNullException">The source or destination are null.</exception>
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
		/// <exception cref="ArgumentNullException">The source stream is null.</exception>
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
		/// <exception cref="ArgumentNullException">The source or destination are null.</exception>
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
		/// <exception cref="ArgumentNullException">The source data is null.</exception>
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
		/// <exception cref="ArgumentNullException">The source data is null.</exception>
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
		/// <exception cref="ArgumentNullException">The source data is null.</exception>
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
		/// <exception cref="ArgumentNullException">The source data is null.</exception>
		public static string CompressToString(byte[] data, Encoding? encoding = null)
		{
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			Contract.EndContractBlock();

			return (encoding ?? Encoding.UTF8).GetString(Compress(data));
		}

		/// <summary>
		/// Returns the compressed version of the source text.
		/// </summary>
		/// <exception cref="ArgumentNullException">The source text is null.</exception>
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

			var poolA = ms.Length > 128 && ms.Length <= int.MaxValue ? ArrayPool<byte>.Shared : null;
			var rawData = poolA?.Rent((int)ms.Length) ?? new byte[ms.Length];
			var poolB = rawData.Length > 124 ? ArrayPool<byte>.Shared : null;
			var compressedData = poolB?.Rent(rawData.Length + 4) ?? new byte[rawData.Length];
			try
			{
				ms.Read(rawData, 0, rawData.Length);
				Buffer.BlockCopy(rawData, 0, compressedData, 4, rawData.Length);
				Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, compressedData, 0, 4);
				return Convert.ToBase64String(compressedData, 0, rawData.Length + 4);
			}
			finally
			{
				poolA?.Return(rawData, true);
				poolB?.Return(compressedData, true);
			}
		}

		/// <summary>
		/// Returns the decompressed version of provided data.
		/// </summary>
		/// <exception cref="ArgumentNullException">The source or destination are null.</exception>
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
		/// <exception cref="ArgumentNullException">The source or destination are null.</exception>
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
		/// <exception cref="ArgumentNullException">The source stream is null.</exception>
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
		/// <exception cref="ArgumentNullException">The source data is null.</exception>
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
		/// <exception cref="ArgumentNullException">The source data is null.</exception>
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
		/// <exception cref="ArgumentNullException">The source data is null.</exception>
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
		/// <exception cref="ArgumentNullException">The compressed source text is null.</exception>
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

			var pool = dataLength > 128 ? ArrayPool<byte>.Shared : null;
			var buffer = pool?.Rent(dataLength) ?? new byte[dataLength];
			try
			{
				ms.Position = 0;
				using (var stream = new GZipStream(ms, CompressionMode.Decompress))
					dataLength = stream.Read(buffer, 0, buffer.Length);

				return (encoding ?? Encoding.UTF8).GetString(buffer, 0, dataLength);
			}
			finally
			{
				pool?.Return(buffer, true);
			}
		}

	}
}
