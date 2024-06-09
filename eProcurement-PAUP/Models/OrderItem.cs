using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProcurement_PAUP.Models
{
    [Table("order_item")]
    public class OrderItem

    {
        [Required]
        [Key]
        public int ID { get; set; }

        public int OrderID { get; set; }

        public int ItemID { get; set; }

        [Required(ErrorMessage = "{0} je obvezan podatak")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} mora biti veći od 0")]
        [Display(Name = "Količina")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "{0} je obvezan podatak")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} mora biti veća od 0")]
        [Display(Name = "Jedinična cijena")]
        public decimal PricePerUnit { get; set; }
    }
}