using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GithubReleaseDownloader
{
    internal class IntegerFunc
    {
        internal static byte[] Normalize(ReadOnlyMemory<byte> value, int expectedLength = -1)
        {
            byte[] bytes = value.ToArray();

            // Strip leading zero if present
            if (bytes.Length > 1 && bytes[0] == 0x00)
            {
                bytes = bytes.Skip(1).ToArray();
            }

            // If expected length is known, pad to that length
            if (expectedLength > 0 && bytes.Length < expectedLength)
            {
                byte[] padded = new byte[expectedLength];
                Buffer.BlockCopy(bytes, 0, padded, expectedLength - bytes.Length, bytes.Length);
                return padded;
            }

            return bytes;
        }

    }
}
