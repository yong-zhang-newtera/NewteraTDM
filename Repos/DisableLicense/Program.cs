using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DisableLicense.UserInfoWebService;
using DisableLicense.AdminWebService;

namespace DisableLicense
{
    /// <summary>
    /// Disable the installed license and get a transfer ID embedded in the license
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string userName, userPwd;

            Console.Write("Please enter administrator name:");
            userName = Console.ReadLine();

            Console.Write("Please enter administrator password:");
            userPwd = Console.ReadLine();

            // get the user name and password from console
            UserInfoService userInfoService = new UserInfoService();
            if (!userInfoService.AuthenticateSuperUser(userName, userPwd))
            {
                Console.WriteLine("Only administrator is allowed to run this command.");
                return;
            }
            else
            {
                Console.WriteLine("Login as the administrator is successfule.");
            }

            Console.Write("Are you sure that you want to disable the license?(y/n):");
            string answer = Console.ReadLine();
            if (string.IsNullOrEmpty(answer) || answer.ToUpper() != "Y")
            {
                return;
            }

            // disable the license
            try
            {
                AdminService adminService = new AdminService();
                string transferId = adminService.DisableLicense(userName, userPwd);
                Console.WriteLine("The installed license has been disabled.");
                if (transferId != null)
                {
                    Console.WriteLine("The id for transferring the license is " + transferId);
                }
                else
                {
                    Console.WriteLine("The current license doesn't have a transferring id.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Diable the license failed.");
                Console.WriteLine("The error is " + ex.Message);
            }
        }
    }
}
