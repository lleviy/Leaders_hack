using ImprovementMap.Entities;
using ImprovementMap.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Z.EntityFramework.Plus;
using Microsoft.EntityFrameworkCore;

namespace ImprovementMap.Services
{
    public interface IExcelService
    {
        ISheet Import(IFormFile file);
        string ParseExcelToObjects(ISheet sheet);
        Area ObjectWithSuchValues(Area obj);
    }

    public class ExcelService : IExcelService
    {
        public static IWebHostEnvironment _environment;
        private readonly DataContext _context;
        public ExcelService(IWebHostEnvironment environment, DataContext context)
        {
            _environment = environment;
            _context = context;
        }
        public ISheet Import(IFormFile file)
        {
            string folderName = "UploadExcel";
            string webRootPath = _environment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            ISheet sheet = null;
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xlsx")
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook
                    }
                    else return null;
                }
            }
            return sheet;
        }


        public string ParseExcelToObjects(ISheet sheet)
        {
            List<Area> intersects = new List<Area>();
            string folderName = "ODH";
            string webRootPath = _environment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string fileName = "output.json";
            string fullPath = Path.Combine(newPath, fileName);
            string textFromFile = "";
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                byte[] array = new byte[stream.Length];
                // считываем данные
                stream.Read(array, 0, array.Length);
                // декодируем байты в строку
                textFromFile = System.Text.Encoding.Default.GetString(array);
            }
            //var odh = JsonConvert.DeserializeObject(textFromFile);
            var js = JToken.Parse(textFromFile);
            Area objInDb = null;
            string result;
            AllObjectsToArchive();
            int i = 0;
            while (sheet.GetRow(i).GetCell(0).ToString() != "1") i++;
            i++;
            int count = 0;
            for (; i <= sheet.LastRowNum - 1; i++) //Read Excel File
            {
                Area area = new Area();
                var row = sheet.GetRow(i);
                int j = row.FirstCellNum;
                area.Name = row.GetCell(++j).ToString();
                foreach (var item in js.Children())
                {
                    var ItemProperties = item.Children<JProperty>();
                    var name = ItemProperties.FirstOrDefault(x => x.Name == "name");
                    var nameValue = name.Value.ToString();
                    if (nameValue == area.Name & count < 5)
                    {
                        count++;
                        var polygon = ItemProperties.FirstOrDefault(x => x.Name == "polygon");
                        var polygonValue = polygon.Value;
                        foreach (var p in polygonValue)
                        {
                            double X, Y;
                            var success = double.TryParse(p[0].ToString(), out X);
                            success = double.TryParse(p[1].ToString(), out Y);
                            if (success == false)
                            {
                                foreach (var kk in p)
                                {
                                    X = double.Parse(kk[0].ToString());
                                    Y = double.Parse(kk[1].ToString());
                                }
                            }
                            Point point = new Point
                            {
                                X = X,
                                Y = Y,
                                Area = area
                            };
                            _context.Points.Add(point);
                        }
                    }
                    //var polygon = item.GetValue("polygon").ToString();
                }
                area.StartPoint = row.GetCell(++j).ToString();
                area.EndPoint = row.GetCell(++j).ToString();
                area.Okrug = row.GetCell(++j).ToString();
                area.Basis = row.GetCell(++j).ToString();
                area.Program = row.GetCell(++j).ToString();
                area.Category = row.GetCell(++j).ToString();
                _context.Areas.Add(area);
                for (int n = 0; n < 3; n++)
                {
                    double square;
                    var success = double.TryParse(row.GetCell(++j).ToString(), out square);
                    if (success == false) square = 0;
                    if (square != 0.00)
                    {
                        InfrastructureObject obj = new InfrastructureObject
                        {
                            Type = (ObjectType)n,
                            Square = square,
                            Area = area
                        };
                        _context.Objects.Add(obj);
                    }
                }
                objInDb = ObjectWithSuchValues(area);
                if (objInDb != null)
                {
                    objInDb.IsActive = true;
                    _context.Areas.Update(objInDb);
                    intersects.Add(objInDb);
                }
                else _context.Areas.Add(area);
            }
            _context.SaveChanges();
            if (intersects.Count != 0)
            {
                result = "Обнаружено пересечение объектов. Старые объекты были перезаписаны новыми. Вы можете скачать excel-файл с пересечениями.";
                FormIntersectsFile(intersects);
            }
            else result = "Загруженные объекты были сохранены в базе данных.";
            return result;
        }

        public Area ObjectWithSuchValues (Area area)
        {
            var areaInDb = _context.Areas.FirstOrDefault(a => a.Name == area.Name & a.StartPoint == area.StartPoint & a.EndPoint == area.EndPoint);
            if (areaInDb != null)
            {
                return areaInDb;
            }
            return null;
        }

        public void FormIntersectsFile(List<Area> intersects)
        {
            string sWebRootFolder = _environment.WebRootPath;
            string sFileName = @"Intersects.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Intersects");

                IRow row = excelSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("Name");
                row.CreateCell(0).SetCellValue("StartPoint");
                row.CreateCell(0).SetCellValue("EndPoint");
                row.CreateCell(0).SetCellValue("Status");

                for (int i = 1; i < intersects.Count; i++)
                {
                    row = excelSheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue(intersects[i].Name);
                    row.CreateCell(0).SetCellValue(intersects[i].StartPoint);
                    row.CreateCell(0).SetCellValue(intersects[i].EndPoint);
                    row.CreateCell(0).SetCellValue(intersects[i].Status.Status);
                }
                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;
        }

        public void AllObjectsToArchive()
        {
            _context.Areas.Update(x => new Area() { IsActive = false });
            _context.BulkSaveChangesAsync();
        }


    }
}
