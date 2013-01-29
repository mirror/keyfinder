using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace PIDConvert
{
	class Program
	{
		static void Main(string[] args)
		{
			// More details here: http://www.licenturion.com/xp/fully-licensed-wpa.txt

			byte[] binaryKey = GetDigitalProductID();
			if (binaryKey == null)
			{
				Console.WriteLine("Unable to retrieve DigitalProductID registry value");
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

			Console.WriteLine(sb.ToString());
		}

		static byte[] GetDigitalProductID()
		{
			byte[] productID = null;

			RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\100\DTS\Setup");
			if (key != null)
			{
				byte[] data = (byte[]) key.GetValue("DigitalProductID");

				int dataOffset = 52;
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
}
