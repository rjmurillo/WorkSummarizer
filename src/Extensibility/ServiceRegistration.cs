using System;

namespace Extensibility
{
    public class ServiceRegistration
    {
        private readonly string m_family;
        private readonly string m_id;
        private readonly string m_name;
     
        public ServiceRegistration(string id, string family, string name)
        {
            m_id = id;
            m_family = family;
            m_name = name;
        }

        public bool IsConfigurable
        {
            get;
            set;
        }

        public string Family
        {
            get { return m_family; }
        }

        public string Id
        {
            get { return m_id; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public bool InvokeOnShellDispatcher { get; set; }

        public string HelpText { get; set; }
        
        public override bool Equals(object obj)
        {
            var otherServiceRegistration = obj as ServiceRegistration;
            if (otherServiceRegistration == null)
            {
                return false;
            }

            return m_id.Equals(otherServiceRegistration.Id);
        }

        public override int GetHashCode()
        {
            return m_id.GetHashCode();
        }
    }
}
