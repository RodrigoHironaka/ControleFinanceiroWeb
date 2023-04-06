using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.Repositorio.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace ControlFinWeb.App.Controllers
{
    public class DbBackupController : Controller
    {
        private const string FileNameFixedPart = "db_backup";

        private readonly IDbBackupService _dbBackupService;
        private readonly IConfiguration _configuration;
        public DbBackupController(IDbBackupService dbBackupService, IConfiguration configuration)
        {
            _dbBackupService = dbBackupService;
            _configuration = configuration;
        }

        private static string GetFileNameWithoutExtension()
        {
            string chrono = DateTime.Now.ToString("yyyyMMdd HHmmss");
            string fileNameWithoutExtension = $"{chrono} - {FileNameFixedPart}";
            return fileNameWithoutExtension;
        }

        [HttpPost]
        public ActionResult Sql(string connectionString)
        {
            Byte[] stream = _dbBackupService.GetDbBackupBytes(connectionString);
            ActionResult actionResult = File(stream, System.Net.Mime.MediaTypeNames.Text.Plain);
            return actionResult;
        }

        public ActionResult DownloadSql(string connectionString)
        {
            Byte[] stream = _dbBackupService.GetDbBackupBytes(connectionString);
            string fileNameWithoutExtension = GetFileNameWithoutExtension();
            string fileName = $"{fileNameWithoutExtension}.sql";
            FileContentResult fileContentResult = File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            return fileContentResult;
        }

        public ActionResult DownloadZip()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string fileNameWithoutExtension = GetFileNameWithoutExtension();
            string sqlFilename = $"{fileNameWithoutExtension}.sql";
            Byte[] zipDbBackupBytes = _dbBackupService.GetZipDbBackupBytes(connectionString, sqlFilename);
            string zipFilename = $"{fileNameWithoutExtension}.zip";
            FileContentResult fileContentResult = File(zipDbBackupBytes, System.Net.Mime.MediaTypeNames.Application.Zip, zipFilename);
            return fileContentResult;
        }
    }
}
