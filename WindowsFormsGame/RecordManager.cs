using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ColorJump
{
    public static class RecordManager
    {
        private static readonly string recordFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "record.dat");

        public static void AddToRecord(int score)
        {
            int current = LoadRecord();
            int newScore = current + score;

            using (var writer = new StreamWriter(recordFilePath))
            {
                string encryptedScore = EncryptScore(newScore);
                writer.WriteLine(encryptedScore);
            }
        }

        public static int LoadRecord()
        {
            if (!File.Exists(recordFilePath))
            {
                AddToRecord(0);
                return 0;
            }

            using (var reader = new StreamReader(recordFilePath))
            {
                string encryptedScore = reader.ReadLine();
                return DecryptScore(encryptedScore);
            }
        }

        private static string EncryptScore(int score)
        {
            string rawData = score.ToString();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData + "0705"));
                return Convert.ToBase64String(hash);
            }
        }

        private static int DecryptScore(string encryptedScore)
        {
            for (int i = 0; i < 1000000; i++)
            {
                if (EncryptScore(i) == encryptedScore)
                    return i;
            }
            return 0;
        }
    }
}