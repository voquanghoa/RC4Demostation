using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
	public class RC4Stream
	{
		private const string key = "awkwzlawlauss2@ww";

		private Stream stream;
		private byte[] buffer = new byte[10 * 1024];
		private RC4Converter rc4Converter;
		private Encoding encoding = Encoding.ASCII;

		public RC4Stream(Stream stream)
		{
			this.stream = stream;
			this.rc4Converter = new RC4Converter(key);
		}

		public Tuple<string, byte[]> Send(string message)
		{
			var byteData = encoding.GetBytes(message);
			var ecryptedData = rc4Converter.Decrypt(byteData);
			stream.Write(ecryptedData, 0, ecryptedData.Length);
			return new Tuple<string, byte[]>(message, ecryptedData);
		}

		public Tuple<byte[], string> Read()
		{
			var length = stream.Read(buffer, 0, buffer.Length);

			if (length > 0)
			{
				var receivedData = buffer.Take(length).ToArray();
				var receivedString = encoding.GetString(receivedData, 0, receivedData.Length);

				var decryptedData = rc4Converter.Decrypt(receivedData);
				var decryptedString = encoding.GetString(decryptedData, 0, decryptedData.Length);

				return new Tuple<byte[], string>(receivedData, decryptedString);
			}

			return null;
		}
	}
}
