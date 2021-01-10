﻿using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace Sweep.Core.Processing
{
    public static class FileExtensionParser
    {
        public static bool HasExtension(string filePath, string extension)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");
            if(extension == null)
                throw new ArgumentNullException("extension");
            Contract.EndContractBlock();

            extension = extension.Trim(' ','*');
            var fileExtension = Path.GetExtension(filePath);
            return extension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsMp3(string filePath)
        {
            return HasExtension(filePath, ".mp3");
        }

        public static bool IsChromeDownload(string filePath)
        {
            return HasExtension(filePath, ".crdownload");
        }
    }
}
