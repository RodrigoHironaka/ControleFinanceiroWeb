using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFinWeb.Repositorio.Servicos
{
    public class ZipHelper
    {
        public static byte[] GetZipFileBytes(byte[] bytes, string sqlFilename, int compressionLevel)
        {
            MemoryStream stream = new MemoryStream(bytes);
            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);
            zipStream.SetLevel(compressionLevel); //0-9, 9 being the highest level of compression

            ZipEntry sqlFileZipEntry = new ZipEntry(sqlFilename);
            sqlFileZipEntry.DateTime = DateTime.Now;
            zipStream.PutNextEntry(sqlFileZipEntry);

            StreamUtils.Copy(stream, zipStream, new byte[4096]);
            zipStream.CloseEntry();
            zipStream.IsStreamOwner = false;
            zipStream.Close();

            outputMemStream.Position = 0;
            byte[] byteArray = outputMemStream.ToArray();
            return byteArray;
        }
    }
}
