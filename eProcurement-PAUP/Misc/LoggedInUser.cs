using eProcurement_PAUP.Models;
using System.Security.Principal;

namespace eProcurement_PAUP.Misc
{
    public class LoggedInUser : IPrincipal
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Permission { get; set; }

        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            if (Permission == role) return true;
            return false;
        }

        public LoggedInUser(User user)
        {
            this.Identity = new GenericIdentity(user.Email);
            this.UserName = user.FirstName + "." + user.LastName;
            this.FullName = user.FullName;
            this.Permission = user.PermissionID;
        }

        public LoggedInUser(string username)
        {
            this.Identity = new GenericIdentity(username);
            this.UserName = username;
        }
    }

}