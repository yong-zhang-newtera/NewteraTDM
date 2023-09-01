using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Newtera.WebApi.Models
{
    public class DirectoryViewModel
    {
        public DirectoryViewModel(string name)
        {
            Name = name;
            Subdirs = new List<DirectoryViewModel>();
        }
        public string Name { get; set; }
        
        public List<DirectoryViewModel> Subdirs { get; set; }

    }
}