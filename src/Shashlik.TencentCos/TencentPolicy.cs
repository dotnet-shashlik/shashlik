using System;
using System.Collections.Generic;
using System.Text;

namespace TencentCos.Sdk
{
    public class TencentPolicy
    {
        public Effect Effect { get; set; }

        public List<string> Action { get; set; }

        public string AllowPath { get; set; }
    }

    public enum Effect
    {
        Allow, Deny
    }
}
