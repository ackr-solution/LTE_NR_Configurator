using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SendLogMessageEventArgs : EventArgs
    {
        public SendLogMessageEventArgs(string _msg)
        {
            msg = _msg;
        }

        public string msg { get; set; }
    }
}
