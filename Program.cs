using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace StegaBuilder
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.Title = "The Dummy App!";
			Console.WriteLine("Hi! I'm a dummy application. I could be anything, a game, web browser, whatever." +
				"Instead, I'm just here typing on this ugly black screen." +
				"\n\nBut am I doing only that?\nI wonder..." + Environment.NewLine);

			int mysize = 7168;
			byte[] hostbytes = File.ReadAllBytes (Assembly.GetExecutingAssembly ().Location);
			int currentsize = hostbytes.Length;
			int msgsize = currentsize - mysize;

			//args[0] = mode - enc or dec
			//args[1] = the message
			//args[2] = the password

			if (args.Length > 0) {
				if (args [0] == "enc") {
					string message = args [1];
					string password = args [2];


					byte[] crypted = encryptdata (System.Text.Encoding.UTF8.GetBytes (message), password, "0123456789101112");
					byte[] joined = JoinTwoByteArrays (hostbytes, crypted);

					using (var fileStream = new FileStream (@"C:\temp\steg_new.exe", FileMode.Create, FileAccess.Write, FileShare.None))
					using (var bw = new BinaryWriter (fileStream)) {
						bw.Write (joined);
					}
				} else if (args [0] == "dec") {
					string password = args [1];

					Stream stream = new MemoryStream(hostbytes);
					BinaryReader b = new BinaryReader(stream); 
					b.BaseStream.Seek (mysize, SeekOrigin.Begin);
					byte[] encmsg = b.ReadBytes (msgsize);

					byte[] decrypted = decryptdata (encmsg, password, "0123456789101112");

					MessageBox.Show (System.Text.Encoding.UTF8.GetString (decrypted));

				}
			}

			Console.ReadKey(true);
		}

		private static byte[] JoinTwoByteArrays(byte[] arrayA, byte[] arrayB)
		{
			byte[] outputBytes = new byte[arrayA.Length + arrayB.Length];
			Buffer.BlockCopy(arrayA, 0, outputBytes, 0, arrayA.Length);
			Buffer.BlockCopy(arrayB, 0, outputBytes, arrayA.Length, arrayB.Length);
			return outputBytes;
		}
			
		//Code to encrypt Data (not my code, credits to the unknown author)
		private static byte[] encryptdata(byte[] bytearraytoencrypt, string key, string iv)
		{
			AesCryptoServiceProvider dataencrypt = new AesCryptoServiceProvider();
			//Block size : Gets or sets the block size, in bits, of the cryptographic operation.
			dataencrypt.BlockSize = 128;
			//KeySize: Gets or sets the size, in bits, of the secret key
			dataencrypt.KeySize = 128;
			//Key: Gets or sets the symmetric key that is used for encryption and decryption.
			dataencrypt.Key = System.Text.Encoding.UTF8.GetBytes(key);
			//IV : Gets or sets the initialization vector (IV) for the symmetric algorithm
			dataencrypt.IV = System.Text.Encoding.UTF8.GetBytes(iv);
			//Padding: Gets or sets the padding mode used in the symmetric algorithm
			dataencrypt.Padding = PaddingMode.PKCS7;
			//Mode: Gets or sets the mode for operation of the symmetric algorithm
			dataencrypt.Mode = CipherMode.CBC;
			//Creates a symmetric AES encryptor object using the current key and initialization vector (IV).
			ICryptoTransform crypto1 = dataencrypt.CreateEncryptor(dataencrypt.Key, dataencrypt.IV);
			//TransformFinalBlock is a special function for transforming the last block or a partial block in the stream.
			//It returns a new array that contains the remaining transformed bytes. A new array is returned, because the amount of
			//information returned at the end might be larger than a single block when padding is added.
			byte[] encrypteddata = crypto1.TransformFinalBlock(bytearraytoencrypt, 0, bytearraytoencrypt.Length);
			crypto1.Dispose();
			//return the encrypted data
			return encrypteddata;
		}

		//code to decrypt data (not my code, credits to the unknown author)
		private static byte[] decryptdata(byte[] bytearraytodecrypt, string key, string iv)
		{
			AesCryptoServiceProvider keydecrypt = new AesCryptoServiceProvider();
			keydecrypt.BlockSize = 128;
			keydecrypt.KeySize = 128;
			keydecrypt.Key = System.Text.Encoding.UTF8.GetBytes(key);
			keydecrypt.IV = System.Text.Encoding.UTF8.GetBytes(iv);
			keydecrypt.Padding = PaddingMode.PKCS7;
			keydecrypt.Mode = CipherMode.CBC;
			ICryptoTransform crypto1 = keydecrypt.CreateDecryptor(keydecrypt.Key, keydecrypt.IV);

			byte[] returnbytearray = crypto1.TransformFinalBlock(bytearraytodecrypt, 0, bytearraytodecrypt.Length);
			crypto1.Dispose();
			return returnbytearray;
		}


	}
}
