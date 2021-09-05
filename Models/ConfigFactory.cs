using System.Collections.Generic;
using System.Linq;
using API.Models;

namespace API.Models {
    public class ConfigFactory : IConfigFactory {
        public IEnumerable<Config> GetConfigs(IEnumerable<string> configStrs) {
            HashSet<Config> configs = new HashSet<Config>();
            foreach (string configStr in configStrs) {
                if (Config.TryParse(configStr, out Config config)) {
                    configs.Add(config);
                }
            }
            return configs.ToList();
        }
    }
}