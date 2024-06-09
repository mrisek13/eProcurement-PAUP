using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProcurement_PAUP.Models
{
    public enum UserCategory
    {
        Admin = 1,
        Nabava = 2,
        Uprava = 3,
        Skladište = 4
    }

    [Table("users")]
    public class User
    {
        [Column("UserId")]
        [Key]
        [Display(Name = "User ID")]
        public int ID { get; set; }

        [Column("username")]
        [Display(Name = "Username")]
        [Required(ErrorMessage = "{0} je obavezno polje")]
        public string UserName { get; set; }

        [Column("firstName")]
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "{0} je obavezno polje")]
        public string FirstName { get; set; }

        [Column("lastName")]
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "{0} je obavezno polje")]
        public string LastName { get; set; }

        [Column("email")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} je obavezno polje")]
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + " " + FirstName;
            }
        }

        [Required(ErrorMessage = "{0} je obavezno polje")]
        [Display(Name = "Ovlast")]
        [ForeignKey("Permission")]
        public string PermissionID { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [NotMapped]
        public string PasswordInput { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [NotMapped]
        [Compare("PasswordInput", ErrorMessage = "Passwords must match")]
        public string PasswordInput2 { get; set; }

        public virtual Permission Permission { get; set; }

        [Display(Name = "Avatar")]
        public byte[] Avatar { get; set; }
    }
}