using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
	public class RC4Converter
	{
		private Encoding unicode = Encoding.Unicode;
		private string key;

		public RC4Converter(string key)
		{
			this.key = key;
		}

		public byte[] Encrypt(byte[] data)
		{
			return EncryptOutput(data);
		}

		public byte[] Decrypt(byte[] data)
		{
			return EncryptOutput(data);
		}

		private byte[] EncryptOutput(byte[] data)
		{
			var encryptedKey = EncryptInitalize(unicode.GetBytes(key));
			var result = new byte[data.Length];

			for(int index=0, a = 0, b = 0, c=0; index < data.Length; index++)
			{
				a = (a + 1) & 255;
				b = (b + encryptedKey[a]) & 255;
				
				Swap(encryptedKey, a, b);

				c = (encryptedKey[a] + encryptedKey[b]) & 255;

				result[index] = (byte)(data[index] ^ encryptedKey[c]);
			}

			return result;
		}

		private byte[] EncryptInitalize(byte[] key)
		{
			byte[] s = Enumerable.Range(0, 256).Select(x=>(byte)x).ToArray();

			for (int i = 0, j = 0; i < 256; i++)
			{
				j = (j + key[i % key.Length] + s[i]) & 255;

				Swap(s, i, j);
			}

			return s;
		}

		private void Swap(byte[] array, int i, int j)
		{
			byte c = array[i];

			array[i] = array[j];
			array[j] = c;
		}
	}
}
