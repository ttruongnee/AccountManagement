using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Menus
{
    public class MenuItem
    {
        public int Key { get; set; }
        public string Description { get; set; }
        public Action Action { get; set; }
    }
}
