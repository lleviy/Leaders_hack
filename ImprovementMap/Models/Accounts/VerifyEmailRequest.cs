using System.ComponentModel.DataAnnotations;

namespace ImprovementMap.Models.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}