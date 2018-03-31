using System;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftQuery.Models
{
    public class AccountNames : Dictionary<DateTime, string>
    {
        public string FirstName { get; set; }

        public override string ToString() 
            => $"First name: {FirstName}, {string.Join(", ", this.Select(kv => $"since {kv.Key:dd.MM.yyyy}: {kv.Value}"))}";
    }
}