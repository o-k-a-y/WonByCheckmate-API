using System.Collections.Generic;
using API.Data;

namespace API.Models {
    // All known configurations we care about
    // Each config follows the format rules:time class:time control
    // This set is used when a game is being parsed to see if we even want to parse results further
    public static class ValidGameConfigurations {
        private static readonly HashSet<Config> validGameConfigurations = new HashSet<Config> {
            
            // Bullet
            // "chess:10:bullet", // doesn't exist?
            new Config("chess", "bullet", "30"), // 30 seconds
            new Config("chess", "bullet", "60"), // 1 minute
            new Config("chess", "bullet", "60+1"), // 1 minute + 1 second per move
            new Config("chess", "bullet", "120+1"), // 2 minutes + 1 second per move
            // "chess:120:bullet", // doesn't exist?

            // Blitz
            new Config("chess", "blitz", "180"),
            new Config("chess", "blitz", "300"),
            new Config("chess", "blitz", "480"), // this one isn't very common, remove maybe
            new Config("chess", "blitz", "600"), // important to note that blitz 600 was changed to rapid 600, but the old games will still have this configuration
            
            // Rapid
            new Config("chess", "rapid", "600"), // 10 minute game
            new Config("chess", "rapid", "900"), // 15 minute
            new Config("chess", "rapid", "1200"), // 20 minute
            new Config("chess", "rapid", "1800"), // 30 minute
            new Config("chess", "rapid", "3600"), // 1 hour

            // Daily
            new Config("chess", "daily", "1/86400"), // 1 day per move
            new Config("chess", "daily", "1/172800"), // 2 days
            new Config("chess", "daily", "1/259200"), // 3 days
            new Config("chess", "daily", "1/432000"), // 5 days 
            new Config("chess", "daily", "1/604800"), // 7 days
            new Config("chess", "daily", "1/1209600"), // 14 days
        };

        public static bool Contains(Config config) {
            bool res = validGameConfigurations.Contains(config);
            return validGameConfigurations.Contains(config);
        }

        public static IEnumerable<Config> Configs() {
            foreach (Config config in validGameConfigurations) {
                yield return config;
            }
        }
    }
}