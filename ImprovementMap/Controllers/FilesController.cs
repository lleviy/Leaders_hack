using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImprovementMap.Entities;
using ImprovementMap.Helpers;
using ImprovementMap.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ImprovementMap.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        private readonly DataContext _context;
        private readonly IExcelService _excelService;
        public FilesController(IWebHostEnvironment environment, DataContext context, IExcelService excelService)
        {
            _environment = environment;
            _context = context;
            _excelService = excelService;
        }

        /// <summary>
        /// Загрузить титульный список
        /// </summary>
        //[Authorize(Role.Admin)]
        [HttpPost]
        public ActionResult Post([FromForm] IFormFile file)
        {
            ISheet sheet = _excelService.Import(file);
            string message = _excelService.ParseExcelToObjects(sheet);
            return Ok(new { message });
        }

        /// <summary>
        /// Скачать список объектов и их оценок
        /// </summary>
        //[Authorize(Role.Admin, Role.Inspector)]
        [HttpGet]
        public async Task<IActionResult> Export()
        {
            string sWebRootFolder = _environment.WebRootPath;
            string sFileName = @"ObjectsStatuses.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("ObjectsStatuses");

                IRow row = excelSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("Name");
                row.CreateCell(0).SetCellValue("StartPoint");
                row.CreateCell(0).SetCellValue("EndPoint");
                row.CreateCell(0).SetCellValue("Status");

                var objects = _context.Areas.Include(o => o.Status).ToList();
                for (int i = 1; i < objects.Count; i++)
                {
                    row = excelSheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue(objects[i].Name);
                    row.CreateCell(0).SetCellValue(objects[i].StartPoint);
                    row.CreateCell(0).SetCellValue(objects[i].EndPoint);
                    row.CreateCell(0).SetCellValue(objects[i].Status.Status);
                }
                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }

        /// <summary>
        /// Скачать список пересечений
        /// </summary>
        [HttpGet]
        public IActionResult ExportIntersects()
        {
            string sWebRootFolder = _environment.WebRootPath;
            string sFileName = @"Intersects.xlsx";
            var memory = new MemoryStream();
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }

    }
}
