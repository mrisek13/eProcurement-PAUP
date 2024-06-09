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
    public class StatusController : Controller
    {
        private BazaDbContext db = new BazaDbContext();

        // GET: Status
        public ActionResult Index()
        {
            // Dohvaćanje svih narudžbi iz baze podataka
            var orders = db.Orders.ToList();

            // Kreiranje liste koja će sadržavati narudžbe s zamijenjenim ID-jevima
            var ordersWithNames = new List<OrderViewModel>();

            // Iteriranje kroz svaku narudžbu
            foreach (var order in orders)
            {
                // Dohvaćanje pravog naziva kupca i dobavljača na temelju ID-jeva
                //var customerName = db.Customers.FirstOrDefault(c => c.ID == order.CustomerID)?.Name;
                var supplierName = db.Suppliers.FirstOrDefault(s => s.ID == order.SupplierID)?.Name;

                // Dodavanje narudžbe s pravim nazivima u listu
                ordersWithNames.Add(new OrderViewModel
                {
                    ID = order.ID,
                    //CustomerName = customerName,
                    SupplierName = supplierName,
                    OrderDate = order.OrderDate,
                    Status = order.Status
                });
            }

            // Vraćanje pogleda s podacima narudžbi s pravim nazivima
            return View(ordersWithNames);
        }


        // GET: Status/Details/5
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

        // GET: Status/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Status/Create
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

        // GET: Status/Edit/5
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

        // POST: Status/Edit/5
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

        // GET: Status/Delete/5
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

        // POST: Status/Delete/5
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

        public ActionResult OrderStatusReport()
        {
            // korak 1: kreiranje objekta tipa dokument
            Document document = new Document();
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, false);
            Font headerFont = new Font(bf, 12);
            Font footerFont = new Font(bf, 10);

            try
            {
                // korak 2:
                // kreiranje objekta za zapisivanje u dokument koristeći PDF stream
                MemoryStream memoryStream = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                // korak 3: otvaranje dokumenta
                document.Open();

                ReportHelper.GenerateReportHeader(document, "Order Statuses", headerFont);

                // Dodavanje tablice s podacima o statusima narudžbi
                PdfPTable table = new PdfPTable(4); // Broj stupaca ovisi o broju svojstava u modelu
                table.WidthPercentage = 100;

                // Dodavanje zaglavlja tablice
                table.AddCell(new Phrase("Customer ID", headerFont));
                table.AddCell(new Phrase("Supplier ID", headerFont));
                table.AddCell(new Phrase("Order Date", headerFont));
                table.AddCell(new Phrase("Status", headerFont));
                // Dodavanje redaka s podacima o narudžbama
                foreach (var order in db.Orders)
                {
                    table.AddCell(new Phrase(order.CustomerID.ToString(), headerFont));
                    table.AddCell(new Phrase(order.SupplierID.ToString(), headerFont));
                    table.AddCell(new Phrase(order.OrderDate.ToShortDateString(), headerFont));
                    table.AddCell(new Phrase(order.Status.ToString(), headerFont));
                }

                document.Add(table);

                ReportHelper.GenerateReportFooter(document, writer, footerFont);

                // korak 5: zatvaranje dokumenta
                document.Close();

                // Nakon što je dokument zatvoren, kreiramo novi MemoryStream
                MemoryStream outputStream = new MemoryStream(memoryStream.ToArray());

                // Resetiranje položaja na početak memorije
                outputStream.Position = 0;

                // Slanje PDF-a korisniku
                return File(outputStream, "application/pdf", "OrderStatuses.pdf");
            }
            catch (Exception ex)
            {
                // U slučaju greške, možemo prikazati poruku o grešci ili je zabilježiti
                return Content("Greška: " + ex.Message);
            }
        }

    }
}
