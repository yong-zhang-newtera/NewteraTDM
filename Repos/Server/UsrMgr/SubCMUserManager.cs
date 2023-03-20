using System;

using Newtera.Server.UsrMgr;

namespace Newtera.Server.UsrMgr
{
    public class SubCMUserManager : CMUserManager
    {
        public override bool Authenticate(string userName, string password)
        {
            return base.Authenticate(userName, password);
        }
    }
}
