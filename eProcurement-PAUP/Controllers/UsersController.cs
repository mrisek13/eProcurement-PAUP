using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using eProcurement_PAUP.Misc;
using eProcurement_PAUP.Models;

namespace eProcurement_PAUP.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private BazaDbContext db = new BazaDbContext();

        // GET: Users
        [Authorize(Roles = UserPermissions.Administrator)]
        public ActionResult Index()
        {
            var usersList = db.Users.OrderBy(x => x.PermissionID).ThenBy(x => x.LastName).ToList();
            return View(usersList);
        }

        [HttpGet]
        [Authorize(Roles = UserPermissions.Administrator)]
        public ActionResult Create()
        {
            PopulatePermissionsDropDownList();
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserPermissions.Administrator)]
        public ActionResult Create(User user, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Hash the password
                user.Password = PasswordHelper.CalculateHash(user.Password);

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    {
                        user.Avatar = binaryReader.ReadBytes(imageFile.ContentLength);
                    }
                }

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulatePermissionsDropDownList(user.PermissionID);
            return View(user);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            UserLogin model = new UserLogin();
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(UserLogin model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var korisnikBaza = db.Users.FirstOrDefault(x => x.UserName == model.Username);
                if (korisnikBaza != null)
                {
                    var passwordOK = korisnikBaza.Password == PasswordHelper.CalculateHash(model.Password);

                    if (passwordOK)
                    {
                        LoggedInUser prijavljeniKorisnik = new LoggedInUser(korisnikBaza);
                        LoggedInUserSerializeModel serializeModel = new LoggedInUserSerializeModel();
                        serializeModel.CopyFromUser(prijavljeniKorisnik);
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        string korisnickiPodaci = serializer.Serialize(serializeModel);

                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1,
                            prijavljeniKorisnik.Identity.Name,
                            DateTime.Now,
                            DateTime.Now.AddDays(1),
                            false,
                            korisnickiPodaci);

                        string ticketEncrypted = FormsAuthentication.Encrypt(authTicket);

                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEncrypted);
                        Response.Cookies.Add(cookie);

                        if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError("", "Neispravno korisničko ime ili lozinka");
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = UserPermissions.Administrator)]
        public ActionResult Edit(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            PopulatePermissionsDropDownList(user.PermissionID);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserPermissions.Administrator)]
        public ActionResult Edit(User user, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var userInDb = db.Users.Find(user.ID);
                if (userInDb == null)
                {
                    return HttpNotFound();
                }

                userInDb.UserName = user.UserName;
                userInDb.FirstName = user.FirstName;
                userInDb.LastName = user.LastName;
                userInDb.Email = user.Email;
                userInDb.PermissionID = user.PermissionID;

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    {
                        userInDb.Avatar = binaryReader.ReadBytes(imageFile.ContentLength);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulatePermissionsDropDownList(user.PermissionID);
            return View(user);
        }

        [HttpGet]
        [Authorize(Roles = UserPermissions.Administrator)]
        public ActionResult Delete(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserPermissions.Administrator)]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulatePermissionsDropDownList(object selectedPermission = null)
        {
            var permissionsQuery = from p in db.Permissions
                                   orderby p.Name
                                   select p;
            ViewBag.PermissionID = new SelectList(permissionsQuery, "ID", "Name", selectedPermission);
        }

        public ActionResult Details(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
    }
}
