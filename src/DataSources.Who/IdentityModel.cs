using System.Runtime.Serialization;

namespace DataSources.Who
{
    [DataContract(Name = IdentityResource)]
    public class IdentityModel
    {
        public IdentityModel()
        {       
        }

        public const string IdentityResource = "identity";
        public const string AliasProperty = "alias";
        public const string ChildrenProperty = "children";
        public const string DisplayNameProperty = "display_name";
        public const string DomainNameProperty = "domain_name";
        public const string DnsDomainNameProperty = "dns_domain_name";
        public const string EnabledProperty = "enabled";
        public const string ObjectIdProperty = "object_id";
        public const string UserPrincipalNameProperty = "user_principal_name";
        public const string SecurityIdentifierProperty = "sid";
        public const string MailProperty = "mail";
        public const string IsMailEnabledProperty = "mail_enabled";
        public const string IsExpandableProperty = "expandable";
        public const string IsWindowsSecurityEnabledProperty = "windows_security_enabled";

        [DataMember(Name = AliasProperty)]
        public string Alias { get; set; }

        [DataMember(Name = DisplayNameProperty)]
        public string DisplayName { get; set; }

        [DataMember(Name = DomainNameProperty)]
        public string DomainName { get; set; }

        [DataMember(Name = DnsDomainNameProperty)]
        public string DnsDomainName { get; set; }

        [DataMember(Name = EnabledProperty)]
        public bool Enabled { get; set; }
    }
}