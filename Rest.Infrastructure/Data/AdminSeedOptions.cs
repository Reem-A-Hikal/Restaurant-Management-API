using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Infrastructure.Data
{
    /// <summary>
    /// Bootstrap admin settings. Read from configuration only
    /// (User Secrets locally, App Settings on Azure) — never hardcoded.
    /// </summary>
    public sealed class AdminSeedOptions
    {
        public const string SectionName = "AdminSeed";

        public string? Email {  get; set; }
        public string? Password { get; set; }
        public string FullName { get; set; } = "System Administrator";
        public string? PhoneNumber { get; set; }
    }
}
