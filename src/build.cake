#load "./BuildSetup.cake"

Parameters.ProjectName = "Cake.SonarScanner";
Parameters.NugetSettings = new NuGetPackSettings
{
    Authors      = new[] { "pitermarx", "augustoproiete" },
    Description  = "Cake aliases for SonarScanner. To be used in conjunction with the sonar-scanner tool.",
    ProjectUrl   = new Uri("https://github.com/cake-contrib/Cake.SonarScanner"),
    License      = new NuSpecLicense { Type = "expression", Value = "MIT" },
    Tags         = new [] { "cake", "sonar", "sonar-scanner", "cake-addin", "cake-build", "cake-contrib", "addin", "script", "build" },
    IconUrl      = new Uri("https://cdn.rawgit.com/cake-contrib/graphics/a5cf0f881c390650144b2243ae551d5b9f836196/png/cake-contrib-medium.png"),
    Files        = new [] {
        new NuSpecContent { Source = "Cake.SonarScanner/bin/Release/netstandard2.0/Cake.SonarScanner.dll", Target = "lib\\netstandard2.0" },
        new NuSpecContent { Source = "Cake.SonarScanner/bin/Release/netstandard2.0/Cake.SonarScanner.xml", Target = "lib\\netstandard2.0" },
        new NuSpecContent { Source = "Cake.SonarScanner/bin/Release/netstandard2.0/Cake.SonarScanner.pdb", Target = "lib\\netstandard2.0" }
    },
    Dependencies = new [] {
        new NuSpecDependency { Id = "Cake.Core", Version = "1.0.0" }
    }
};

Task("Default")
    .IsDependentOn("Publish");

RunTarget(Parameters.Target);
