/*!
 * @author electricessence / https://github.com/electricessence/
 * Licensing: MIT https://github.com/electricessence/Open/blob/dotnet-core/LICENSE.md
 */

using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Open.Collections;


namespace Open.Formatting
{

	public static class Gzip
	{
		/// <summary>
		/// Returns the compressed version of provided data.
		/// </summary>
		public static byte[] Compress(byte[] data)
		{
			if(data==null)
				throw new ArgumentNullException("data");
			using (var msi = new MemoryStream(data))
			using (var mso = new MemoryStream())
			{
				using (var gs = new GZipStream(mso, CompressionMode.Compress))
					msi.CopyTo(gs);

				return mso.ToArray();
			}
		}

		/// <summary>
		/// Returns the compressed byte array of provided string.
		/// </summary>
		public static byte[] Compress(string data, Encoding encoding = null)
		{
			if(data==null)
				throw new ArgumentNullException("data");
			return Compress(data.ToByteArray(encoding));
		}

		/// <summary>
		/// Returns the compressed string of provided byte array.
		/// </summary>
		public static string CompressToString(byte[] data, Encoding encoding = null)
		{
			if(data==null)
				throw new ArgumentNullException("data");

			return (encoding ?? Encoding.UTF8).GetString(Compress(data));
		}

		public static string CompressToString(string text, Encoding encoding = null)
		{
			if(text==null)
				throw new ArgumentNullException("text");


			byte[] buffer = (encoding??Encoding.UTF8).GetBytes(text);
			var ms = new MemoryStream();
			using (var stream = new GZipStream(ms, CompressionMode.Compress, true))
				stream.Write(buffer, 0, buffer.Length);

			ms.Position = 0;

			var rawData = new byte[ms.Length];
			ms.Read(rawData, 0, rawData.Length);

			var compressedData = new byte[rawData.Length + 4];
			Buffer.BlockCopy(rawData, 0, compressedData, 4, rawData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, compressedData, 0, 4);
			return Convert.ToBase64String(compressedData);
		}


		/// <summary>
		/// Returns the decompressed version of provided data.
		/// </summary>
		public static byte[] Decompress(byte[] data)
		{
			if(data==null)
				throw new ArgumentNullException("data");
			using (var msi = new MemoryStream(data))
			using (var mso = new MemoryStream())
			{
				using (var gs = new GZipStream(msi, CompressionMode.Decompress))
					gs.CopyTo(mso);

				return mso.ToArray();
			}
		}

		/// <summary>
		/// Returns the decompressed string of provided byte array.
		/// </summary>
		public static string DecompressToString(byte[] data, Encoding encoding = null)
		{
			if(data==null)
				throw new ArgumentNullException("data");

			return (encoding ?? Encoding.UTF8).GetString(Decompress(data)) ?? String.Empty;
		}

		public static string DecompressToString(string compressedText, Encoding encoding = null)
		{
			if(compressedText==null)
				throw new ArgumentNullException("compressedText");

			if (compressedText.Length == 0)
				return String.Empty;

			byte[] compressedData = Convert.FromBase64String(compressedText);
			if(compressedData.Length<4)
				throw new Exception("Too short.");			

			using (var ms = new MemoryStream())
			{
				int dataLength = BitConverter.ToInt32(compressedData, 0);
				if(dataLength<4)
					throw new Exception("Too short.");			

				ms.Write(compressedData, 4, compressedData.Length - 4);

				var buffer = new byte[dataLength];

				ms.Position = 0;
				using (var stream = new GZipStream(ms, CompressionMode.Decompress))
					stream.Read(buffer, 0, buffer.Length);

				return (encoding??Encoding.UTF8).GetString(buffer);
			}
		}
	}
}
