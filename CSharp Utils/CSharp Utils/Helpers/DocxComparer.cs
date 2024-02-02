using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CSharp_Utils.Helpers
{
    public class DocxComparer
    {
        /// <summary>
        /// Compares the content of two Word documents and returns a boolean indicating if they are equal.
        /// </summary>
        /// <param name="pathDoc1">The file path of the first document.</param>
        /// <param name="pathDoc2">The file path of the second document.</param>
        /// <returns>True if the documents are equal, false otherwise.</returns>
        public bool DocumentsAreEqual(string pathDoc1, string pathDoc2)
        {
            if (string.IsNullOrWhiteSpace(pathDoc1) || !File.Exists(pathDoc1)) return false;
            if (string.IsNullOrWhiteSpace(pathDoc2) || !File.Exists(pathDoc2)) return false;

            var doc1 = ReadFileContent(Path.GetFullPath(pathDoc1));
            var doc2 = ReadFileContent(Path.GetFullPath(pathDoc2));

            if (doc1 == null || doc2 == null) return false;
            return CalculateHash(doc1) == CalculateHash(doc2);
        }

        /// <summary>
        /// Calculates the MD5 hash of the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The MD5 hash of the input string.</returns>
        protected virtual string CalculateHash(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = MD5.HashData(inputBytes);

            StringBuilder sb = new();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Reads the content of a file specified by the file path and returns it as a string.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The content of the file as a string.</returns>
        protected virtual string ReadFileContent(string filePath)
        {
            string content = File.ReadAllText(filePath);
            return content;
        }
    }
}
