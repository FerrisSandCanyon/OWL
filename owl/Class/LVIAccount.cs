using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace owl.Class
{
    public class LVIAccount : ListViewItem
    {
        public LVIAccount(string text) : base(text) {}

        public Class.Account AccountClass = null;
    }
}
