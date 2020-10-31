using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImprovementMap.Entities;
using ImprovementMap.Helpers;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ImprovementMap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : BaseController
    {
        public static IWebHostEnvironment _environment;
        private readonly DataContext _context;
        public PhotosController(IWebHostEnvironment environment, DataContext context)
        {
            _environment = environment;
            _context = context;
        }

        [Authorize(Role.Admin, Role.Inspector)]
        [HttpPost("{objectId:int}")]
        public ActionResult Post(int objectId, [FromForm] IFormFile file)
        {
            var obj = _context.Objects.Find(objectId);
            if (obj == null) throw new KeyNotFoundException("Объекта с таким id не существует");

            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                if (sFileExtension == ".png" || sFileExtension == ".jpg" || sFileExtension == ".jpeg")
                {
                    try
                    {
                        if (!Directory.Exists(_environment.WebRootPath + "\\uploads\\"))
                        {
                            Directory.CreateDirectory(_environment.WebRootPath + "\\uploads\\");
                        }
                        var imageURL = _environment.WebRootPath + "\\uploads\\" + file.FileName;
                        using (FileStream filestream = System.IO.File.Create(imageURL))
                        {
                            file.CopyTo(filestream);
                            filestream.Flush();
                        }

                        Photo photo = new Photo
                        {
                            ImageURL = imageURL,
                            AccountId = Account.Id,
                            InfrastructureObjectId = objectId
                        };
                        _context.Photos.Add(photo);
                        _context.SaveChanges();

                        return Ok(new { message = "Загрузка прошла успешно" });
                    }
                    catch (Exception ex)
                    {
                        return Ok(new { message = ex.ToString() });
                    }
                }
                return Ok(new { message = "Недопустимый формат файла" });
            }
            else
            {
                return Ok(new { message = "Загрузка фото не удалась" });
            }
        }

        [Authorize(Role.Admin, Role.Inspector)]
        [HttpDelete("{photoId:int}")]
        public IActionResult Delete(int photoId)
        {
            var photo = _context.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo.AccountId != Account.Id && Account.Role != Role.Admin)
                return Unauthorized(new { message = "Вы не можете удалять чужие фото" });
            _context.Photos.Remove(photo);
            _context.SaveChangesAsync();
            return Ok(new { message = "Фото успешно удалено" });
        }
    }
}
