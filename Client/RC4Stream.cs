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

		public RC4Stream(Stream stream)
		{
			this.stream = stream;
			this.rc4Converter = new RC4Converter(key);
		}

		public void Send(string message)
		{
			var byteData = Encoding.ASCII.GetBytes(message);
			var ecryptedData = rc4Converter.Decrypt(byteData);
			stream.Write(ecryptedData, 0, ecryptedData.Length);
		}

		public string Read()
		{
			var length = stream.Read(buffer, 0, buffer.Length);

			if (length > 0)
			{
				var realData = buffer.Take(length).ToArray();
				var decryptedData = rc4Converter.Decrypt(realData);
				return Encoding.ASCII.GetString(decryptedData, 0, decryptedData.Length);
			}

			return string.Empty;
		}
	}
}
