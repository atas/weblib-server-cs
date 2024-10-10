using System;
using System.Security.Cryptography;
using System.Text;
using MoreLinq;

namespace WebAppShared.Utils;

public static class PasswordUtils
{
	/// <summary>
	/// Gets a random salt
	/// </summary>
	/// <param name="maximumSaltLength"></param>
	/// <returns></returns>
	public static string GetRandomSalt(int maximumSaltLength = 16)
	{
		return KeyGenerator.GetUniqueKey(maximumSaltLength);

		// byte[] salt;
		// using (var random = new RNGCryptoServiceProvider())
		// {
		// 	random.GetNonZeroBytes(salt = new byte[maximumSaltLength]);
		// }
		//
		// return Convert.ToBase64String(salt);
	}

	/// <summary>
	/// Gets the hash of a password with a salt
	/// </summary>
	/// <param name="password"></param>
	/// <param name="salt"></param>
	/// <returns></returns>
	public static string GetHash(string password, string salt)
	{
		byte[] saltBytes = Convert.FromBase64String(salt);

		var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000);
		byte[] hash = pbkdf2.GetBytes(32);

		return Convert.ToBase64String(hash);
	}

	public static string Sha256Hash(string rawData)
	{
		// Create a SHA256   
		using SHA256 sha256Hash = SHA256.Create();
		// ComputeHash - returns byte array  
		byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

		// Convert byte array to a string   
		StringBuilder builder = new StringBuilder();
		bytes.ForEach(t => builder.Append(t.ToString("x2")));
		return builder.ToString();
	}


}
