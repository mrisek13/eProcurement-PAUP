using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProcurement_PAUP.Misc
{
    public class LoggedInUserSerializeModel
    {
        public string KorisnickoIme { get; set; }
        public string PrezimeIme { get; set; }
        public string Ovlast { get; set; }

        internal void CopyFromUser(LoggedInUser user)
        {
            this.KorisnickoIme = user.UserName;
            this.PrezimeIme = user.FullName;
            this.Ovlast = user.Permission;
        }
    }
}