using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Common;
using RestSharp;

namespace DataSources.Who
{
    public static class IdentityUtility
    {
        private static IdentityCache m_cache = new IdentityCache();
        private static IEnumerable<string> m_exclusions = new[] { "Rollup Engine" };

        public static string Sanitize(string input)
        {
            if (!m_exclusions.Contains(input, StringComparer.OrdinalIgnoreCase))
            {
                if (input.IndexOf("\\", StringComparison.Ordinal) > 0)
                {
                    return input.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last();
                }


                // Remove name distinguishers, not helpful to lookup service
                var pattern = "\\([^()]*\\)";
                var regex = new Regex(pattern, RegexOptions.Compiled);
                input = regex.Replace(input, string.Empty);

                // If the input still has a space, it's probably a display name and not an alias. Try to look up the alias
                if (input.IndexOf(' ') > 0)
                {
//                var client = new RestClient(String.Format("http://qeid01"));
//                client.DefaultParameters.Clear();
//                var request = new RestRequest(String.Format("api/v1/identity/{0}?fields=alias,display_name,enabled", input), Method.GET);
//                request.AddHeader("Content-type", "text/json");
//                request.AddHeader("Cache-Control", "max-age=86400");
//                
//                var response = client.Execute<IdentityModel>(request);
//
//                if(response.StatusCode == HttpStatusCode.OK && response.Data != null)
//                {
//                    input = response.Data.Alias;
//                }
//                else
//                {
//                    request = new RestRequest(String.Format("api/v1/identity?property=display_name&term={0}&fields=alias,display_name,enabled", input + '*'), Method.GET);
//                    request.AddHeader("Content-type", "text/json");
//                    request.AddHeader("Cache-Control", "max-age=86400");
//
//                    var displayNameResponse = client.Execute<IdentityModelList>(request);
//
//                    if (displayNameResponse.StatusCode != HttpStatusCode.OK || displayNameResponse.Data == null || displayNameResponse.Data.FirstOrDefault() == null)
//                    {
//                        return input;
//                    }
//
//                    input = displayNameResponse.Data.First().Alias;
//                }

                
                    var tp = new TeamProvider();
                    input = tp.ResolveDisplayName(input);
                }
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

        private readonly ClockCache<string, Participant> m_identitiesByInput;
        private readonly ClockCache<string, IEnumerable<Participant>> m_identitiesByAlias; 

        public HashSet<string> GetKeys()
        {
            return m_identitiesByInput.Keys;
        }

        public IdentityCache()
        {
            m_identitiesByInput = new ClockCache<string, Participant>(MaximumSize);
            m_identitiesByAlias = new ClockCache<string, IEnumerable<Participant>>(MaximumSize);
        }

        public Participant Get(string input)
        {
            return Get(input, m_identitiesByInput, f => IdentityUtility.CreateFromString(input));
        }

        public void CacheMultipleIdentities(string key, IEnumerable<string> values)
        {
            throw new NotImplementedException();
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
                m_identitiesByInput.TryAdd(participant.Alias, participant);
                m_identitiesByInput.TryAdd(key, participant);
            }
        }
    }
}