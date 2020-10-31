using System;

namespace ImprovementMap.Entities
{
    public class AreaStatus
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}
