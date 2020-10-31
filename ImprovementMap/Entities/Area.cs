

using System;
using System.Collections.Generic;

namespace ImprovementMap.Entities
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public string Polygon { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public string Okrug { get; set; }
        public string Basis { get; set; }
        public string Program { get; set; }
        public string Category { get; set; }
        public virtual List<InfrastructureObject> Objects { get; set; }
        public virtual List<Point> Polygon { get; set; }
        public AreaStatus Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
