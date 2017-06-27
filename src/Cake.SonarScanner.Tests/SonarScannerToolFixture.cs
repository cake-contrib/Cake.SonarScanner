using Cake.Testing.Fixtures;

namespace Cake.SonarScanner.Tests
{
    public class SonarScannerToolFixture : ToolFixture<SonarScannerSettings>
    {
        public SonarScannerToolFixture(SonarScannerSettings settings) : base("sonar-scanner")
        {
            Settings = settings;
        }

        protected override void RunTool()
        {
            var tool = new SonarScannerTool(FileSystem, Environment, ProcessRunner, Tools);
            tool.RunSonarScanner(Settings);
        }
    }
}