using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    public class SshConnectionInfo
    {
        public string IdentityFile { get; set; }
        public string Pass { get; set; }
        public string Host { get; set; }
        public string User { get; set; }
    }
}
