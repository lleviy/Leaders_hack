using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImprovementMap.Helpers;
using ImprovementMap.Entities;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace ImprovementMap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreasController : BaseController
    {
        private readonly DataContext _context;

        public AreasController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все области или области, которым требуется ремонт определенного типа (дороги: 1, тротуары: 2, обочины: 3)
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Area>> GetAll(int? typeId)
        {
            if (typeId >= 0 && typeId <= 2)
            {
                return Ok(_context.Areas.Include(o => o.Objects.Where(i => i.Type == (ObjectType)typeId)));
            }
            else
            {
                return Ok(_context.Areas.Include(o => o.Status).Include(o => o.Objects).Include(o => o.Polygon).Where(o => o.Polygon.Count != 0));
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult<Area> GetById(int id)
        {
            var obj = _context.Areas.Include(o => o.Status).Include(o => o.Objects).FirstOrDefault(o => o.Id == id);
            return Ok(obj);
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        public ActionResult<Area> Create(Area model)
        {
            var obj = model;
            _context.Areas.Add(obj);
            _context.SaveChanges();
            return Ok(obj);
        }

        [Authorize(Role.Admin)]
        [HttpPut("{id:int}")]
        public ActionResult<Area> Update(int id, Area model)
        {
            var obj = _context.Areas.Find(id);
            if (obj == null) throw new KeyNotFoundException("Object not found");
            _context.Areas.Update(model);
            _context.SaveChanges();
            return Ok(obj);
        }

        [Authorize(Role.Admin)]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var obj = _context.Areas.Find(id);
            _context.Areas.Remove(obj);
            _context.SaveChanges();
            return Ok(new { message = "Область ремонта удалена" });
        }

    }
}
