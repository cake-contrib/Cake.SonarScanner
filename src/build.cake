#load "./BuildSetup.cake"

Parameters.ProjectName = "Cake.SonarScanner";
Parameters.NugetSettings = new NuGetPackSettings 
{
    Authors      = new[] {"pitermarx"},
    Description  = "Cake aliases for SonarScanner. To be used in conjunction with the SonarScanner chocolatey package.",
    ProjectUrl   = new Uri("https://github.com/pitermarx/Cake.SonarScanner"),
    LicenseUrl   = new Uri("https://github.com/pitermarx/Cake.SonarScanner/blob/master/LICENSE"),
    Tags         = new [] {"cake","sonar","sonar-scanner"},
    IconUrl      = new Uri("https://cdn.rawgit.com/cake-contrib/graphics/a5cf0f881c390650144b2243ae551d5b9f836196/png/cake-contrib-medium.png"),
    Files        = new [] { 
        new NuSpecContent { Source = "Cake.SonarScanner/bin/Release/Cake.SonarScanner.dll", Target = "lib\\net46" },
        new NuSpecContent { Source = "Cake.SonarScanner/bin/Release/Cake.SonarScanner.xml", Target = "lib\\net46" },
        new NuSpecContent { Source = "Cake.SonarScanner/bin/Release/Cake.SonarScanner.pdb", Target = "lib\\net46" }
    },
    Dependencies = new [] {
        new NuSpecDependency { Id = "Cake.Core", Version = "0.22.1" }
    }
};

Task("Default")
    .IsDependentOn("Publish");
    
RunTarget(Parameters.Target);
