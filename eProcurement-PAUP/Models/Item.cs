using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProcurement_PAUP.Models
{
    public enum ItemCategory
    {
        Sirovine = 1,
        Alati = 2,
        Usluge = 3,
        Elektronika = 4,
        Tekstil = 5,
        Hrana = 6,
        Knjige = 7,
        Sport = 8,
        Kozmetika = 9,
        Ostalo = 10
    }

    [Table("item")]
    public class Item
    {
        [Required]
        [Key]
        public int ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} je obavezan podatak")]
        [Display(Name = "Naziv artikla")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} je obavezan podatak")]
        [Display(Name = "Opis artikla")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }

        [Required(ErrorMessage = "{0} je obavezna")]
        [Display(Name = "Količina na skladištu")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "{0} je obavezna")]
        [Display(Name = "Minimalna količina")]
        public int MinimumQuantity { get; set; }

        [Required(ErrorMessage = "{0} je obavezna")]
        [Display(Name = "Naručena količina")]
        public int OrderedQuantity { get; set; }

        [Required(ErrorMessage = "{0} je obavezna")]
        [Display(Name = "Kategorija")]
        public ItemCategory Category { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} je obavezan podatak")]
        [Display(Name = "Jedinica mjere")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Unit { get; set; }

        [Display(Name = "Slika")]
        public byte[] Image { get; set; }

        [Required(ErrorMessage = "{0} je obvezno polje")]
        [Range(0.01, double.MaxValue, ErrorMessage = "{0} mora biti veća od 0")]
        [Display(Name = "Cijena")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        [ForeignKey("Supplier")]
        public int SupplierID { get; set; }

        [Display(Name = "Dobavljač")]
        public virtual Supplier Supplier { get; set; }
    }
}