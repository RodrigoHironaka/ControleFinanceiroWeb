using ControlFinWeb.Repositorio.Interface;
using MySql.Data.MySqlClient;
using MySQLBackupNetCore;
using System;
using System.IO;

namespace ControlFinWeb.Repositorio.Servicos
{
    public class DbBackupService : IDbBackupService
    {
        private static MemoryStream GetMemoryStream(string connectionString)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
            using (MySqlCommand mySqlCommand = new MySqlCommand())
            using (MySqlBackup mb = new MySqlBackup(mySqlCommand))
            {
                mySqlCommand.Connection = mySqlConnection;
                mySqlConnection.Open();
                mb.ExportToMemoryStream(memoryStream);
                mySqlConnection.Close();
            }
            return memoryStream;
        }
        public byte[] GetDbBackupBytes(string connectionString)
        {
            MemoryStream memoryStream = GetMemoryStream(connectionString);
            return memoryStream.ToArray();
        }

        public byte[] GetZipDbBackupBytes(string connectionString, string sqlFilename)
        {
            byte[] dbBackupBytes = GetDbBackupBytes(connectionString);
            byte[] zipFileBytes = ZipHelper.GetZipFileBytes(dbBackupBytes, sqlFilename, 3);
            return zipFileBytes;
        }
    }
}
