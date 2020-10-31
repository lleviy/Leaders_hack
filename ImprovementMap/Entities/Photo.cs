using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImprovementMap.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public int AccountId { get; set; }
        public int InfrastructureObjectId { get; set; }
    }
}
