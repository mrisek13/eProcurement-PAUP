using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eProcurement_PAUP.Models
{
    [Table("permissions")]
    public class Permission
    {
        [Key]
        public string ID { get; set; }
        public string Name { get; set; }
    }
}