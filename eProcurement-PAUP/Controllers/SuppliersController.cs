using System;
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
    public class SuppliersController : Controller
    {
        private BazaDbContext db = new BazaDbContext();

        public ActionResult Index(string searchString, SupplierStatus? statusFilter, int? page, string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "address_desc" : "Address";
            ViewBag.PhoneSortParm = sortOrder == "Phone" ? "phone_desc" : "Phone";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "status_desc" : "Status";

            var suppliers = db.Suppliers.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(s =>
                    s.Name.Contains(searchString) ||
                    s.Address.Contains(searchString) ||
                    s.Phone.Contains(searchString) ||
                    s.Email.Contains(searchString) ||
                    s.Status.ToString().Contains(searchString)
                );
            }

            if (statusFilter != null)
            {
                suppliers = suppliers.Where(s => s.Status == statusFilter);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    suppliers = suppliers.OrderByDescending(s => s.Name);
                    break;
                case "Address":
                    suppliers = suppliers.OrderBy(s => s.Address);
                    break;
                case "address_desc":
                    suppliers = suppliers.OrderByDescending(s => s.Address);
                    break;
                case "Phone":
                    suppliers = suppliers.OrderBy(s => s.Phone);
                    break;
                case "phone_desc":
                    suppliers = suppliers.OrderByDescending(s => s.Phone);
                    break;
                case "Email":
                    suppliers = suppliers.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    suppliers = suppliers.OrderByDescending(s => s.Email);
                    break;
                case "Status":
                    suppliers = suppliers.OrderBy(s => s.Status);
                    break;
                case "status_desc":
                    suppliers = suppliers.OrderByDescending(s => s.Status);
                    break;
                default:
                    suppliers = suppliers.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.SearchString = searchString;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.CurrentSort = sortOrder;
            return View(suppliers.ToPagedList(pageNumber, pageSize));
        }


        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            return View(new Supplier());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Supplier supplier, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Ako je dostupna slika
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Pretvaranje slike u niz bajtova
                    using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    {
                        supplier.Logo = binaryReader.ReadBytes(imageFile.ContentLength);
                    }
                }

                // Spremanje dobavljača u bazu podataka
                db.Suppliers.Add(supplier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Address,Phone,Email,Status")] Supplier supplier, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Ako je dostupna nova slika
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Pretvaranje slike u niz bajtova
                    using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    {
                        supplier.Logo = binaryReader.ReadBytes(imageFile.ContentLength);
                    }
                }
                else
                {
                    // Ako nema nove slike, zadržati postojeću sliku
                    var existingSupplier = db.Suppliers.AsNoTracking().FirstOrDefault(s => s.ID == supplier.ID);
                    if (existingSupplier != null)
                    {
                        supplier.Logo = existingSupplier.Logo;
                    }
                }

                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }


        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Supplier supplier = db.Suppliers.Find(id);
            db.Suppliers.Remove(supplier);
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

        public ActionResult Report()
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

                ReportHelper.GenerateReportHeader(document, "Suppliers", headerFont);

                // Dodavanje tablice s podacima o dobavljačima
                PdfPTable table = new PdfPTable(5); // Broj stupaca ovisi o broju svojstava u modelu, isključujući logo
                table.WidthPercentage = 100;

                // Dodavanje zaglavlja tablice
                table.AddCell("Name");
                table.AddCell("Address");
                table.AddCell("Phone");
                table.AddCell("Email");
                table.AddCell("Status");

                // Dodavanje redaka s podacima o dobavljačima
                foreach (var supplier in db.Suppliers)
                {
                    table.AddCell(supplier.Name);
                    table.AddCell(supplier.Address);
                    table.AddCell(supplier.Phone);
                    table.AddCell(supplier.Email);
                    table.AddCell(supplier.Status.ToString());
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
                return File(outputStream, "application/pdf", "Suppliers.pdf");
            }
            catch (Exception ex)
            {
                // U slučaju greške, možemo prikazati poruku o grešci ili je zabilježiti
                return Content("Greška: " + ex.Message);
            }
        }
    }
}
