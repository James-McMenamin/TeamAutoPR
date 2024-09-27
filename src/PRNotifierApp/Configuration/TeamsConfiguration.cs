using System.Collections.Generic;

namespace PRNotifierApp.Configuration
{
    public class TeamsConfiguration
    {
        public string DefaultChannelId { get; set; } = string.Empty;
        public Dictionary<string, string> RepositoryChannelMappings { get; set; } = new Dictionary<string, string>();
    }
}