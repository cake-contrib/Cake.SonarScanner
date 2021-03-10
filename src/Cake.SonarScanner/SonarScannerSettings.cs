using Cake.Core.Tooling;
using System.Collections.Generic;

namespace Cake.SonarScanner
{
    /// <summary>
    /// Settings for the aliases
    /// </summary>
    public class SonarScannerSettings : ToolSettings
    {
        /// <summary>
        /// If false adds the -X cmdline parameter.
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// If false adds the -X cmdline parameter.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
    }
}