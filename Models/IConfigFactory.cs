using System.Collections.Generic;

namespace API.Models {
    public interface IConfigFactory {
        IEnumerable<Config> GetConfigs(IEnumerable<string> configs);
    }
}