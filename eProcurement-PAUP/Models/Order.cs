using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProcurement_PAUP.Models
{
    public enum OrderStatus
    {
        Nepotvrđeno,
        Naručeno,
        Tranzit,
        Delay,
        Odbijeno,
        Isporučeno,
    }

    [Table("order")]
    public class Order
    {
        [Required]
        [Key]
        public int ID { get; set; }

        public int CustomerID { get; set; }

        [Required(ErrorMessage = "{0} je obavezan podatak")]
        [Display(Name = "Dobavljač")]
        public int SupplierID { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "{0} je obavezan podatak")]
        [Display(Name = "Datum narudžbe")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "{0} je obavezan podatak")]
        [Display(Name = "Status narudžbe")]
        public OrderStatus Status { get; set; }
    }
}