using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFinWeb.Repositorio.Interface
{
    public interface IDbBackupService
    {
        Byte[] GetDbBackupBytes(string connectionString);
        Byte[] GetZipDbBackupBytes(string connectionString, string sqlFilename);
    }
}
