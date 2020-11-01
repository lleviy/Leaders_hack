using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImprovementMap.Entities;
using ImprovementMap.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task = ImprovementMap.Entities.Task;

namespace ImprovementMap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly DataContext _context;

        public TasksController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все задачи
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Task>> GetAll(int? typeId)
        {
            return Ok(_context.Tasks);
        }

        /// <summary>
        /// Назначить задачу инспектору
        /// </summary>
        //[Authorize(Role.Manager)]
        [HttpPost]
        public ActionResult<Task> Create(Task model)
        {
            _context.Tasks.Add(model);
            _context.SaveChanges();
            return Ok(model);
        }

        /// <summary>
        /// Обновить созданную задачу
        /// </summary>
        [Authorize(Role.Manager)]
        [HttpPut("{id:int}")]
        public ActionResult<Task> Update(int id, Task model)
        {
            var obj = _context.Statuses.Find(id);
            if (obj == null) throw new KeyNotFoundException("Object not found");
            _context.Tasks.Update(model);
            _context.SaveChanges();
            return Ok(obj);
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
        [Authorize(Role.Manager)]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var obj = _context.Tasks.Find(id);
            _context.Tasks.Remove(obj);
            _context.SaveChanges();
            return Ok(new { message = "Задача удалена" });
        }
    }
}
