using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImprovementMap.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
