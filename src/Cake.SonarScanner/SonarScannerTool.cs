using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.SonarScanner
{
    internal class SonarScannerTool : Tool<SonarScannerSettings>
    {
        protected internal SonarScannerTool(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IProcessRunner processRunner,
            IToolLocator tools)
            : base(fileSystem, environment, processRunner, tools)
        {
        }

        protected sealed override string GetToolName()
        {
            return "sonar-scanner";
        }

        protected sealed override IEnumerable<string> GetToolExecutableNames()
        {
            return new[] { "sonar-scanner.bat", "sonar-scanner.exe", "sonar-scanner" };
        }

        protected internal void RunSonarScanner(SonarScannerSettings settings)
        {
            var sb = new ProcessArgumentBuilder();
            if (settings.Debug)
            {
                sb.Append("-X");
            }

            if (settings.Properties != null)
            {
                foreach (var settingsProperty in settings.Properties)
                {
                    sb.Append("-D" + settingsProperty.Key + "=" + settingsProperty.Value);
                }
            }

            Run(settings, sb);
        }
    }
}