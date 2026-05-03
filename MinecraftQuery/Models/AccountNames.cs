using System;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftQuery.Models
{
    public class AccountNames : Dictionary<DateTime, string>
    {
        public AccountNames(IDictionary<DateTime, string> dictionary) : base(dictionary)
        {
        }

        public override string ToString() 
            => string.Join(", ", this.Select(kv => kv.Key == DateTime.MinValue ? $"First name: {kv.Value}" : $"since {kv.Key:dd.MM.yyyy}: {kv.Value}"));
    }
}