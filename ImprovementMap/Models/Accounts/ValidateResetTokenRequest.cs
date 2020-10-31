using System.ComponentModel.DataAnnotations;

namespace ImprovementMap.Models.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}