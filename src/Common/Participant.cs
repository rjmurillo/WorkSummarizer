using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class IdentityUtility
    {
        public static string Sanitize(string input)
        {
            if (input.IndexOf("\\", StringComparison.Ordinal) > 0)
            {
                return input.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last();
            }

            return input;
        }
    }

    public class Participant : IEquatable<Participant>
    {
        public Participant(string alias)
        {
            Alias = IdentityUtility.Sanitize(alias);
        }

        public string Alias { get; private set; }

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
