using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PairProgram
{
    public class UserService
    {
        public List<User> Users { get; } = new();

        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public bool Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            if (Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return false;

            Users.Add(new User { Username = username, PasswordHash = HashPassword(password) });
            return true;
        }

        public bool Login(string username, string password)
        {
            var hash = HashPassword(password);
            return Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                               && u.PasswordHash == hash);
        }

        public int ImportFromCsvLines(IEnumerable<string> lines)
        {
            int addedCount = 0;
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length < 2) continue;

                var username = parts[0].Trim();
                var hash = parts[1].Trim();

                if (!Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    Users.Add(new User { Username = username, PasswordHash = hash });
                    addedCount++;
                }
            }
            return addedCount;
        }
    }
}
