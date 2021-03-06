﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImprovementMap.Entities;
using ImprovementMap.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImprovementMap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : BaseController
    {
        private readonly DataContext _context;

        public StatusesController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Назначить оценку объекту
        /// </summary>
        [Authorize(Role.Admin)]
        [HttpPost]
        public ActionResult<Area> Create(int areaId, AreaStatus model)
        {
            model.AreaId = areaId;
            model.AccountId = Account.Id;
            _context.Statuses.Add(model);
            _context.SaveChanges();
            return Ok(model);
        }

        /// <summary>
        /// Обновить оценку
        /// </summary>
        [Authorize(Role.Admin)]
        [HttpPut("{id:int}")]
        public ActionResult<Area> Update(int id, AreaStatus model)
        {
            var obj = _context.Statuses.Find(id);
            if (obj == null) throw new KeyNotFoundException("Object not found");
            _context.Statuses.Update(model);
            _context.SaveChanges();
            return Ok(obj);
        }

        /// <summary>
        /// Удалить оценку
        /// </summary>
        [Authorize(Role.Admin)]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var obj = _context.Statuses.Find(id);
            _context.Statuses.Remove(obj);
            _context.SaveChanges();
            return Ok(new { message = "Область ремонта удалена" });
        }
    }
}
