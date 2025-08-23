using System;
using System.Collections.Generic;

namespace Mtl.Toolbox
{
    /// <summary>
    /// A thread-safe low-allocating Uid generator for all kinds of uses
    /// </summary>
    public static class UidGenerator
    {
        // If 8 is good enough for git, it's good enough for us
        private const int BufferLength = 8;
        private const string AllowedChars = "abcdefghijklmnopqrstuv0123456789";

        private static readonly Dictionary<int, char[]> BufferCache;
        private static readonly Random Rng;

        static UidGenerator()
        {
            BufferCache = new Dictionary<int, char[]>();
            Rng = new Random();
        }

        public static string GetUid(int length = BufferLength)
        {
            char[] buffer;
            lock (BufferCache)
            {
                if (!BufferCache.TryGetValue(length, out buffer))
                {
                    buffer = new char[length];
                    BufferCache.Add(length, buffer);
                }
            }

            lock (buffer)
            {
                for (int i = 0, iMax = buffer.Length; i < iMax; ++i)
                {
                    buffer[i] = AllowedChars[Rng.Next(AllowedChars.Length)];
                }

                return new string(buffer);
            }
        }
    }
}