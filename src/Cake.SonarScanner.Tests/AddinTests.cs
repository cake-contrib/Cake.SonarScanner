using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cake.SonarScanner.Tests
{
    public class AddinTests
    {
        [Fact]
        public void SettingsShouldBeDefined()
        {
            // arrange
            var fixture = new SonarScannerToolFixture(null);
            //act
            var result = Record.Exception(() => fixture.Run());
            //assert
            Assert.IsType<NullReferenceException>(result);
        }

        [Fact]
        public void NoParameters()
        {
            // arrange
            var fixture = new SonarScannerToolFixture(new SonarScannerSettings());

            //act
            fixture.Run();

            //assert
            Assert.Equal(string.Empty, fixture.ProcessRunner.Results.Single().Args);
        }

        [Fact]
        public void Parameters()
        {
            // arrange
            var fixture = new SonarScannerToolFixture(new SonarScannerSettings
            {
                Debug = true,
                Properties = new Dictionary<string, string>
                {
                    {"aa", "bb"}
                }
            });

            //act
            fixture.Run();

            //assert
            Assert.Equal("-X -Daa=bb", fixture.ProcessRunner.Results.Single().Args);
        }
    }
}