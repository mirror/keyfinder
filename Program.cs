using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace PIDConvert
{
	class Program
	{
		static List<Key> keys = new List<Key>();

		static void Main(string[] args)
		{
			// More details here: http://www.licenturion.com/xp/fully-licensed-wpa.txt

			// keys to read
			keys.Add(new Key(@"SOFTWARE\Microsoft\Microsoft SQL Server\100\DTS\Setup", "DigitalProductID", 52));
			keys.Add(new Key(@"SOFTWARE\Microsoft\Microsoft SQL Server\100\Tools\Setup", "DigitalProductID", 52));
			keys.Add(new Key(@"SOFTWARE\Microsoft\Microsoft SQL Server\110\DTS\Setup", "DigitalProductID", 0));
			keys.Add(new Key(@"SOFTWARE\Microsoft\Microsoft SQL Server\110\Tools\Setup", "DigitalProductID", 0));

			for (int i = 0; i < keys.Count; i++)
				PrintKey(i);
		}

		static void PrintKey(int key) {
			byte[] binaryKey = GetDigitalProductID(key);
			if (binaryKey == null)
			{
				Console.WriteLine("Unable to read {0}/{1}", keys[key].key, keys[key].value);
				return;
			}

			char[] digits = { 'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'M', 'P', 'Q', 'R', 'T', 'V', 'W', 'X', 'Y', '2', '3', '4', '6', '7', '8', '9' };
			char[] decodedKey = new char[25];

			for (int i = decodedKey.Length - 1; i >= 0; i--)
			{
				 int k = 0;
				 for (int j = binaryKey.Length - 1; j >= 0; j--)
				 {
					k = (k << 8) + binaryKey[j];
					binaryKey[j] = (byte)(k / 24);
					k = k % 24;
				 }
				 decodedKey[i] = digits[k];
			}

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < decodedKey.Length; ++i)
			{
				if (i > 0 && i % 5 == 0)
					sb.Append("-");
				sb.Append(decodedKey[i]);
			}

			Console.WriteLine(@"{0} {1}\{2}", sb.ToString(), keys[key].key, keys[key].value);
		}

		static byte[] GetDigitalProductID(int k)
		{
			byte[] productID = null;
			Key pk = keys[k];

			RegistryKey key = Registry.LocalMachine.OpenSubKey(pk.key);
			if (key != null)
			{
				byte[] data = (byte[]) key.GetValue(pk.value);

				int dataOffset = pk.offset;
				int dataLen = 15;
				if (data != null && data.Length >= dataOffset + dataLen)
				{
					productID = new byte[dataLen];
					Array.Copy(data, dataOffset, productID, 0, productID.Length);
				}

				key.Close();
			}

			return productID;
		}
	}

	public struct Key {
		public String key, value;
		public int offset;

		public Key(String k, String v, int o) {
			key = k;
			value = v;
			offset = o;
		}
	}
}
