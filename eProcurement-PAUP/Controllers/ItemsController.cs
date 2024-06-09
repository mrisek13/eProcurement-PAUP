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
using PagedList;

namespace eProcurement_PAUP.Controllers
{
    public class ItemsController : Controller
    {
        private BazaDbContext db = new BazaDbContext();

        public ActionResult Index(string searchString, ItemCategory? categoryFilter, int? page, string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.SupplierSortParm = sortOrder == "Supplier" ? "supplier_desc" : "Supplier";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";
            ViewBag.QuantitySortParm = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewBag.MinimumQuantitySortParm = sortOrder == "MinimumQuantity" ? "minquantity_desc" : "MinimumQuantity";
            ViewBag.UnitSortParm = sortOrder == "Unit" ? "unit_desc" : "Unit";
            ViewBag.CategorySortParm = sortOrder == "Category" ? "category_desc" : "Category";

            var items = db.Items.AsQueryable();

            // Pretraga po nazivu artikla
            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(i => i.Name.Contains(searchString));
            }

            // Filtriranje po kategoriji artikla
            if (categoryFilter != null)
            {
                items = items.Where(i => i.Category == categoryFilter);
            }

            // Sortiranje po potrebi
            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(i => i.Name);
                    break;
                case "Supplier":
                    items = items.OrderBy(i => i.Supplier.Name);
                    break;
                case "supplier_desc":
                    items = items.OrderByDescending(i => i.Supplier.Name);
                    break;
                case "Price":
                    items = items.OrderBy(i => i.Price);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(i => i.Price);
                    break;
                case "Quantity":
                    items = items.OrderBy(i => i.Quantity);
                    break;
                case "quantity_desc":
                    items = items.OrderByDescending(i => i.Quantity);
                    break;
                case "MinimumQuantity":
                    items = items.OrderBy(i => i.MinimumQuantity);
                    break;
                case "minquantity_desc":
                    items = items.OrderByDescending(i => i.MinimumQuantity);
                    break;
                case "Unit":
                    items = items.OrderBy(i => i.Unit);
                    break;
                case "unit_desc":
                    items = items.OrderByDescending(i => i.Unit);
                    break;
                case "Category":
                    items = items.OrderBy(i => i.Category);
                    break;
                case "category_desc":
                    items = items.OrderByDescending(i => i.Category);
                    break;
                default:
                    items = items.OrderBy(i => i.Name);
                    break;
            }

            // Paginacija rezultata
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.SearchString = searchString;
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.CurrentSort = sortOrder;
            return View(items.ToPagedList(pageNumber, pageSize));
        }



        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            ViewBag.Suppliers = db.Suppliers.ToList();
            return View(new Item());
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Item item, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Ako je dostupna slika
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Pretvaranje slike u niz bajtova
                    using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    {
                        item.Image = binaryReader.ReadBytes(imageFile.ContentLength);
                    }
                }
                db.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Ako model nije valjan, ponovno učitamo dobavljače u ViewBag
            ViewBag.Suppliers = db.Suppliers.ToList();
            return View(item);
        }

        // GET: Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.SupplierID = new SelectList(db.Suppliers, "ID", "Name", item.SupplierID);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,MinimumQuantity,Unit,Quantity,Category,Price,SupplierID")] Item item, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Učitajte trenutni objekt iz baze
                var currentItem = db.Items.AsNoTracking().FirstOrDefault(i => i.ID == item.ID);
                if (currentItem == null)
                {
                    return HttpNotFound();
                }

                // Ako je dostupna nova slika
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    {
                        item.Image = binaryReader.ReadBytes(imageFile.ContentLength);
                    }
                }
                else
                {
                    // Ako nije odabrana nova slika, zadržite staru
                    item.Image = currentItem.Image;
                }

                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SupplierID = new SelectList(db.Suppliers, "ID", "Name", item.SupplierID);
            return View(item);
        }

        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
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

        // POST: Items/AddToOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToOrder(int id, int quantity)
        {
            // Pronađi item s odgovarajućim ID-om
            Item item = db.Items.Find(id);

            // Provjeri je li item pronađen
            if (item == null)
            {
                return HttpNotFound();
            }

            // Ažuriraj polje OrderedQuantity za odabrani item
            item.OrderedQuantity += quantity;

            // Spremi promjene u bazu podataka
            db.SaveChanges();

            // Preusmjeri korisnika na pregled narudžbe
            return RedirectToAction("Index");
        }

        public ActionResult ItemReport()
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

                ReportHelper.GenerateReportHeader(document, "Item Report", headerFont);

                // Dodavanje tablice s podacima o artiklima
                PdfPTable table = new PdfPTable(7); // Broj stupaca ovisi o broju svojstava u modelu, isključujući sliku
                table.WidthPercentage = 100;

                // Dodavanje zaglavlja tablice
                table.AddCell("Name");
                table.AddCell("Description");
                table.AddCell("Quantity");
                table.AddCell("Minimum Quantity");
                table.AddCell("Unit");
                table.AddCell("Category");
                table.AddCell("Price");

                // Dodavanje redaka s podacima o artiklima
                foreach (var item in db.Items)
                {
                    table.AddCell(item.Name);
                    table.AddCell(item.Description);
                    table.AddCell(item.Quantity.ToString());
                    table.AddCell(item.MinimumQuantity.ToString());
                    table.AddCell(item.Unit);
                    table.AddCell(item.Category.ToString());
                    table.AddCell(item.Price.ToString("C")); // C format za prikaz cijene u valuti
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
                return File(outputStream, "application/pdf", "Items.pdf");
            }
            catch (Exception ex)
            {
                // U slučaju greške, možemo prikazati poruku o grešci ili je zabilježiti
                return Content("Greška: " + ex.Message);
            }
        }

    }
}
