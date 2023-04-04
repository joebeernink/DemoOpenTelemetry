namespace DemoOpenTelemetry
{
    public class CustomerInstanceConfigurationOptions
    {
        public string EnvironmentShortName { get; set; } = string.Empty;

        public string RegionShortName { get; set; } = string.Empty;

        public string StampName { get; set; } = string.Empty;

        public string CustomerId { get; set; } = string.Empty;

        public string CustomerInstanceName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string DnsSubDomain { get; set; } = string.Empty;

        public IEnumerable<KeyValuePair<string, object>> Values
        {
            get
            {
                var kvps = new List<KeyValuePair<string, object>> {
                            new("EnvironmentShortName", EnvironmentShortName),
                            new("RegionShortName", RegionShortName),
                            new("StampName", StampName),
                            new("CustomerId", CustomerId.ToString()),
                            new("CustomerInstanceName", CustomerInstanceName),
                            new("Name", Name),
                            new("DnsSubDomain", DnsSubDomain)
                        };
                return kvps;
            }
        }
    }
}
