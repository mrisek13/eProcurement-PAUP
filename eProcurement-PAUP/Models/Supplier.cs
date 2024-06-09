using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProcurement_PAUP.Models
{
    public enum SupplierStatus
    {
        Aktivan = 1,
        Neaktivan = 2,
        Odbijen = 3
    }

    [Table("supplier")]
    public class Supplier
    {
        [Required]
        [Key]
        public int ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} je obavezan podatak")]
        [Display(Name = "Ime dobavljača")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} je obavezan podatak")]
        [Display(Name = "Adresa dobavljača")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Address { get; set; }

        [Required(ErrorMessage = "{0} je obvezno polje")]
        [RegularExpression(@"^\d{9,15}$", ErrorMessage = "{0} mora imati između 9 i 15 znamenki")]
        [Display(Name = "Kontakt telefon")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "{0} je obvezno polje")]
        [EmailAddress(ErrorMessage = "{0} je u neispravnom formatu")]
        [Display(Name = "Email adresa")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} je obavezan podatak")]
        [Display(Name = "Status dobavljača")]
        public SupplierStatus Status { get; set; }

        [Display(Name = "Logotip")]
        public byte[] Logo { get; set; }
    }
}