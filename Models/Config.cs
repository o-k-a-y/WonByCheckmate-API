using System;

namespace API.Models {
    public class Config : IEquatable<Config> {
        public string Rules { get; private set; }
        public string TimeClass { get; private set; }
        public string TimeControl { get; private set; }

        public Config(string rules, string timeClass, string timeControl) => (Rules, TimeClass, TimeControl) = (rules, timeClass, timeControl);

        // Instead of implementing interface, this is possible, but probably better to use the interface to enforce the contract
        //  public override bool Equals(object obj)
        //     => Equals(obj as Config);

        public bool Equals(Config other) {
            return other != null && 
                Rules.Equals(other.Rules) && 
                TimeClass.Equals(other.TimeClass) && 
                TimeControl.Equals(other.TimeControl);
        }


        // Try to parse the string equivalent of a config object such as "chess:bullet:30" to a valid Config object
        public static bool TryParse(string configStr, out Config config) {
            string[] configArr = configStr.Split(':');
            if (configArr.Length != 3) {
                config = null;
                return false;
            }

            config = new Config(configArr[0], configArr[1], configArr[2]);

            if (ValidGameConfigurations.Contains(config)) {
                return true;
            } else {
                config = null;
                return false;
            }
        }

        public static bool TryParse(string rules, string timeClass, string timeControl, out Config config) {
            config = new Config(rules, timeClass, timeControl);

            if (ValidGameConfigurations.Contains(config)) {
                return true;
            } else {
                config = null;
                return false;
            }
        }
        public override int GetHashCode() {
            return HashCode.Combine(Rules, TimeClass, TimeControl);
        }

        public override string ToString() {
            return $"{Rules}:{TimeClass}:{TimeControl}";
        }
    }
}