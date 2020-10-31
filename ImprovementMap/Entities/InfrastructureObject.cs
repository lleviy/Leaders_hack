using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImprovementMap.Entities
{
    public class InfrastructureObject
    {
        public int Id { get; set; }
        public ObjectType Type { get; set; }
        public double Square { get; set; }
        public virtual List<Photo> Photos { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; }
    }
}
