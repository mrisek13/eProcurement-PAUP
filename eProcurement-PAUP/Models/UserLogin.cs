using System.ComponentModel.DataAnnotations;

namespace eProcurement_PAUP.Models
{
    public class UserLogin
    {
        [Display(Name = "Korisničko ime")]
        [Required]
        public string Username { get; set; }

        [Display(Name = "Lozinka")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}