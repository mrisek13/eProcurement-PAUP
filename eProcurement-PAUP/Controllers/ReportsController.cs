using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using eProcurement_PAUP.Models;

namespace eProcurement_PAUP.Controllers
{
    public class ReportsController : Controller
    {
        private readonly BazaDbContext db = new BazaDbContext();

        public ActionResult MonthlyCostsBySupplier(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
            {
                startDate = DateTime.Today;
            }

            if (!endDate.HasValue)
            {
                endDate = DateTime.Today;
            }

            // Dodajemo jedan dan na krajnji datum kako bi uključili sve narudžbe na taj dan
            endDate = endDate.Value.AddDays(1);

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            if (!startDate.HasValue || !endDate.HasValue)
            {
                return View(new List<OrderViewModel>());
            }

            var reportData = db.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Join(db.Suppliers, o => o.SupplierID, s => s.ID, (o, s) => new { o, s })
                .Join(db.OrderItems, os => os.o.ID, oi => oi.OrderID, (os, oi) => new { os.o, os.s, oi })
                .GroupBy(x => x.s.Name)
                .Select(g => new
                {
                    SupplierName = g.Key,
                    TotalCost = g.Sum(x => x.oi.Quantity * x.oi.PricePerUnit)
                })
                .ToList()
                .Select(g => new OrderViewModel
                {
                    SupplierName = g.SupplierName,
                    TotalCost = g.TotalCost
                })
                .ToList();

            return View(reportData);
        }

        public ActionResult TotalCosts(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
            {
                startDate = DateTime.Today;
            }

            if (!endDate.HasValue)
            {
                endDate = DateTime.Today;
            }

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            if (!startDate.HasValue || !endDate.HasValue)
            {
                return View(new List<OrderViewModel>());
            }

            var reportData = db.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Join(db.Suppliers, o => o.SupplierID, s => s.ID, (o, s) => new { o, s })
                .Join(db.OrderItems, os => os.o.ID, oi => oi.OrderID, (os, oi) => new { os.o, os.s, oi })
                .GroupBy(x => new { x.s.Name, x.o.OrderDate.Month, x.o.OrderDate.Year })
                .Select(g => new
                {
                    SupplierName = g.Key.Name,
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    TotalCost = g.Sum(x => x.oi.Quantity * x.oi.PricePerUnit)
                })
                .ToList()
                .Select(g => new OrderViewModel
                {
                    SupplierName = g.SupplierName,
                    OrderDate = new DateTime(g.Year, g.Month, 1),
                    TotalCost = g.TotalCost
                })
                .ToList();

            return View(reportData);
        }


    }
}
