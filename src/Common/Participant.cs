using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Participant : IEquatable<Participant>
    {
        public string Alias { get; set; }

        public override string ToString()
        {
            return Alias;
        }

        public override int GetHashCode()
        {
            return (string.IsNullOrEmpty(Alias) ? 0 : Alias.GetHashCode());
        }

        public bool Equals(Participant other)
        {
            return other != null && Alias.Equals(other.Alias, StringComparison.OrdinalIgnoreCase);
        }
    }
}
