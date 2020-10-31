using System.ComponentModel.DataAnnotations;

namespace ImprovementMap.Models.Accounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}