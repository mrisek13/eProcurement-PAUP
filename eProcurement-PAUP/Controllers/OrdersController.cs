using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eProcurement_PAUP.Models;
using eProcurement_PAUP.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace eProcurement_PAUP.Controllers
{
    public class OrdersController : Controller
    {
        private BazaDbContext db = new BazaDbContext();

        public ActionResult Index()
        {
            var itemsGroupedBySupplier = db.Items
                                            .Where(i => i.OrderedQuantity > 0)
                                            .GroupBy(i => i.SupplierID)
                                            .ToList();

            var orderViewModels = new List<OrderViewModel>();

            foreach (var group in itemsGroupedBySupplier)
            {
                var supplierID = group.Key;
                var supplierName = db.Suppliers.FirstOrDefault(s => s.ID == supplierID)?.Name;

                var orderViewModel = new OrderViewModel
                {
                    SupplierID = supplierID,
                    SupplierName = supplierName,
                    Items = group.ToList()
                };

                orderViewModels.Add(orderViewModel);
            }

            return View(orderViewModels);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View(new Order());
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CustomerID,SupplierID,OrderDate,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CustomerID,SupplierID,OrderDate,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GeneratePDF()
        {
            MemoryStream memoryStream = new MemoryStream();
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, false);
            Font headerFont = new Font(bf, 12);
            Font footerFont = new Font(bf, 10);

            ReportHelper.GenerateReportHeader(document, "Orders", headerFont);

            PdfPTable table = new PdfPTable(4); // Broj stupaca ovisi o broju atributa koje želite prikazati u PDF-u

            // Dodajte zaglavlje tablice
            table.AddCell("Name");
            table.AddCell("Description");
            table.AddCell("Ordered Quantity");
            table.AddCell("Price");

            decimal totalPrice = 0;

            // Dodajte redove s podacima
            foreach (var item in db.Items.Where(i => i.OrderedQuantity > 0))
            {
                table.AddCell(item.Name);
                table.AddCell(item.Description);
                table.AddCell(item.OrderedQuantity + " " + item.Unit);
                decimal itemPrice = item.OrderedQuantity * item.Price;
                totalPrice += itemPrice;
                table.AddCell(itemPrice.ToString() + " €");
            }

            // Dodajte ukupnu cijenu dolje desno
            PdfPCell totalCell = new PdfPCell(new Phrase("Total: " + totalPrice.ToString() + " €"));
            totalCell.Colspan = 4;
            totalCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            table.AddCell(totalCell);

            // Dodajte tablicu u dokument
            document.Add(table);

            ReportHelper.GenerateReportFooter(document, writer, footerFont);

            document.Close();
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();

            // Vratite PDF kao FileResult
            return File(bytes, "application/pdf", "OrdersReport.pdf");
        }

        public ActionResult Confirmation()
        {
            var items = db.Items.Where(i => i.OrderedQuantity > 0).ToList();
            foreach (var item in items)
            {
                item.OrderedQuantity = 0;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();

            return View();
        }

        [HttpPost]
        public ActionResult ConfirmAllOrders()
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var itemsToOrder = db.Items.Where(i => i.OrderedQuantity > 0).ToList();

                    foreach (var group in itemsToOrder.GroupBy(i => i.SupplierID))
                    {
                        // Kreirajte novu narudžbu
                        Order order = new Order
                        {
                            SupplierID = group.Key, // Postavite SupplierID na ID dobavljača iz grupe
                            OrderDate = DateTime.Now,
                            Status = OrderStatus.Nepotvrđeno // Postavite status narudžbe
                        };

                        // Dodajte novu narudžbu u bazu podataka
                        db.Orders.Add(order);
                        db.SaveChanges(); // SaveChanges se mora pozvati ovdje kako bi se generirao OrderID

                        // Iterirajte kroz svaku stavku u grupi narudžbi
                        foreach (var item in group)
                        {
                            // Kreirajte novi OrderItem
                            OrderItem orderItem = new OrderItem
                            {
                                OrderID = order.ID, // Postavite ID novostvorene narudžbe
                                ItemID = item.ID,
                                Quantity = item.OrderedQuantity,
                                PricePerUnit = item.Price
                            };

                            // Dodajte novi OrderItem u bazu podataka
                            db.OrderItems.Add(orderItem);
                        }

                        // SaveChanges se mora pozvati ovdje kako bi se spremili OrderItems
                        db.SaveChanges();
                    }

                    // Commitajte transakciju ako su sve promjene uspješno spremljene
                    dbContextTransaction.Commit();

                    // Preusmjerite korisnika na odgovarajuću stranicu (npr. potvrda narudžbe)
                    return RedirectToAction("Confirmation", "Orders");
                }
                catch (Exception)
                {
                    // Ako dođe do greške, poništite transakciju
                    dbContextTransaction.Rollback();
                    throw;
                }
            }
        }
    }
}
