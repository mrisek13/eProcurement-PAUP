using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProcurement_PAUP.Models
{
    public class OrderViewModel
    {
        public int ID { get; set; }
        public int SupplierID { get; set; }
        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public List<Item> Items { get; set; }
        public decimal TotalCost { get; set; }
        public int Month { get { return OrderDate.Month; } }
        public int Year { get { return OrderDate.Year; } }
    }
}