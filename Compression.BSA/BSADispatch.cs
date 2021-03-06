﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wabbajack.Common;

namespace Compression.BSA
{
    public static class BSADispatch
    {
        public static async ValueTask<IBSAReader> OpenRead(AbsolutePath filename)
        {
            var fourcc = "";
            using (var file = await filename.OpenRead())
            {
                fourcc = Encoding.ASCII.GetString(new BinaryReader(file).ReadBytes(4));
            }

            if (fourcc == TES3Reader.TES3_MAGIC)
                return await TES3Reader.Load(filename);
            if (fourcc == "BSA\0")
                return await BSAReader.LoadWithRetry(filename);
            if (fourcc == "BTDX")
                return await BA2Reader.Load(filename);
            throw new InvalidDataException("Filename is not a .bsa or .ba2, magic " + fourcc);
        }

        private static HashSet<string> MagicStrings = new HashSet<string> {TES3Reader.TES3_MAGIC, "BSA\0", "BTDX"};
        public static async ValueTask<bool> MightBeBSA(AbsolutePath filename)
        {
            using var file = await filename.OpenRead();
            var fourcc = Encoding.ASCII.GetString(new BinaryReader(file).ReadBytes(4));
            return MagicStrings.Contains(fourcc);
        }
    }
}
