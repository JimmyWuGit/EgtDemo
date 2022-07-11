using System;
using System.Collections.Generic;
using System.Text;

namespace EgtDemo.Common.Configuration
{
    public class Appsetting1
    {
        public Logging Logging { get; set; }
        public Appsettings1 AppSettings { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }
        public string MicrosoftHostingLifetime { get; set; }
    }

    public class Appsettings1
    {
        public Rediscaching RedisCaching { get; set; }
        public string SqlServerConnection { get; set; }
        public string ProviderName { get; set; }
    }

    public class Rediscaching
    {
        public bool Enabled { get; set; }
        public string ConnectionString { get; set; }
    }

    public class Connectionstrings
    {
        public string DefaultConnStr { get; set; }
    }

}
