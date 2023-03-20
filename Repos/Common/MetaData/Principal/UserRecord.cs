using System;
using System.Collections.Generic;
using System.Text;

namespace Newtera.Common.MetaData.Principal
{
    public class UserRecord
    {
        public string UserName { get; set; }

        public string UserId{ get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string SecurityStamp { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string Picture { get; set; }
        public string Division { get; set; }
        public string Address { get; set; }
        public bool IsConfirmed { get; set; }

        public UserRecord Clone()
        {
            UserRecord cloned = new UserRecord();

            cloned.UserName = this.UserName;
            cloned.UserId = this.UserId;
            cloned.FirstName = this.FirstName;
            cloned.LastName = this.LastName;
            cloned.Password = this.Password;
            cloned.Email = this.Email;
            cloned.PhoneNumber = this.PhoneNumber;
            cloned.IsConfirmed = this.IsConfirmed;
            cloned.Password = this.Password;
            cloned.SecurityStamp = this.SecurityStamp;
            cloned.Picture = this.Picture;
            cloned.Division = this.Division;
            cloned.Address = this.Address;

            return cloned;
        }
    }
}
