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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ImprovementMap.Controllers
{
    [Route("api/[controller]")]
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

        //[Authorize(Role.Admin, Role.Inspector)]
        //[HttpGet]
        //public async Task<IActionResult> Export()
        //{
        //    string sWebRootFolder = _environment.WebRootPath;
        //    string sFileName = @"ObjectsStatuses.xlsx";
        //    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
        //    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    var memory = new MemoryStream();
        //    using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
        //    {
        //        IWorkbook workbook = new XSSFWorkbook();
        //        ISheet excelSheet = workbook.CreateSheet("ObjectsStatuses");

        //        IRow row = excelSheet.CreateRow(0);
        //        row.CreateCell(0).SetCellValue("Latitude");

        //        var objects = _context.Areas.ToList();
        //        for (int i = 0; i < objects.Count; i++)
        //        {
        //            row = excelSheet.CreateRow(i);
        //            row.CreateCell(0).SetCellValue(objects[i].Latitude);
        //        }
        //        workbook.Write(fs);
        //    }
        //    using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        //}

    }
}
