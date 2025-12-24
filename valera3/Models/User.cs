using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace valera3.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        public string Role { get; set; } = "User"; // По умолчанию "User"
        
        public List<Valera> Valeras { get; set; } = new List<Valera>();
    }
}
