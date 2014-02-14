using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;

namespace DataSources.Who
{
    public static class IdentityUtility
    {
        private static IdentityCache m_cache = new IdentityCache();
        private static IEnumerable<string> m_exclusions = new[] {"Rollup Engine"};

        public static string Sanitize(string input)
        {
            if (!m_exclusions.Contains(input, StringComparer.OrdinalIgnoreCase))
            {
                if (input.IndexOf("\\", StringComparison.Ordinal) > 0)
                {
                    return input.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last();
                }


                var tp = new TeamProvider();
                input = tp.ResolveDisplayName(input);

            }

            return input;
        }

        internal static Participant CreateFromString(string input)
        {
            Trace.WriteLine("Cache miss: " + input);

            var alias = Sanitize(input);

            Trace.WriteLine("Cache sanitized input: " + alias);

            return new Participant(alias);
        }

        public static Participant Create(string input)
        {
            return m_cache.Get(input);
        }

        public static IEnumerable<string> GetIdentityAttributes()
        {
            return m_cache.GetKeys();
        }
    }

    internal sealed class IdentityCache
    {
        private const int MaximumSize = 512;

        private readonly ClockCache<string, Participant> m_identitiesByAlias;

        public HashSet<string> GetKeys()
        {
            return m_identitiesByAlias.Keys;
        }

        public IdentityCache()
        {
            m_identitiesByAlias = new ClockCache<string, Participant>(MaximumSize);
        }

        public Participant Get(string input)
        {
            return Get(input, m_identitiesByAlias, f => IdentityUtility.CreateFromString(input));
        }

        private Participant Get(string key, ClockCache<string, Participant> cache, Func<string, Participant> lookupFunc)
        {
            Participant retval;
            if (!cache.TryGetValue(key, out retval))
            {
                var id = lookupFunc(key);
                if (id != null)
                {
                    retval = id;
                    CacheIdentity(id, key);
                }
            }

            return retval;
        }

        private void CacheIdentity(Participant participant, string key)
        {
            if (participant != null)
            {
                m_identitiesByAlias.TryAdd(participant.Alias, participant);
                m_identitiesByAlias.TryAdd(key, participant);
            }
        }
    }
}