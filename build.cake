#tool choco:?package=sonarcube-scanner&version=3.0.3.778
#tool "nuget:?package=xunit.runner.console&version=2.1.0"
#tool "nuget:?package=GitVersion.CommandLine&version=3.6.2"
#tool "nuget:?package=OpenCover&version=4.6.519"
#tool "nuget:?package=coveralls.io&version=1.3.4"
#addin "nuget:?package=Cake.Coveralls&version=0.4.0"
#addin "nuget:?package=Cake.SonarScanner&version=0.1.0"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var sonarScannerToken = EnvironmentVariable("sonar_scanner_token");
var nugetApiToken = EnvironmentVariable("nuget_api_token");
var coverallsToken = EnvironmentVariable("coveralls_token");

var version = GitVersion(new GitVersionSettings { UpdateAssemblyInfo = true });
if (AppVeyor.IsRunningOnAppVeyor) {
    AppVeyor.UpdateBuildVersion(version.NuGetVersionV2);
}

Task("Clean")
    .Does(() => 
{
    CleanDirectory("./nuget");
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
    if (FileExists("./Cake.SonarScanner.Tests.dll.xml"))
        DeleteFile("./Cake.SonarScanner.Tests.dll.xml");
    if (FileExists("./Coverage.xml"))
        DeleteFile("./Coverage.xml");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("Cake.SonarScanner.sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() => 
{
    MSBuild("Cake.SonarScanner.sln", configurator =>
        configurator
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal));
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() => 
{
     OpenCover(tool => 
        tool.XUnit2("./**/bin/**/*.Tests.dll", new XUnit2Settings {
            XmlReport = true,
            NoAppDomain = true,
            OutputDirectory = "."
        }),
        "Coverage.xml",
        new OpenCoverSettings()
            .WithFilter("+[*]* -[xunit.*]* -[*.Tests]*")
    );

    if (AppVeyor.IsRunningOnAppVeyor) 
    {
        AppVeyor.UploadTestResults("./Cake.SonarScanner.Tests.dll.xml", AppVeyorTestResultsType.XUnit);
        CoverallsIo("Coverage.xml", new CoverallsIoSettings()
        {
            RepoToken = coverallsToken
        });
    }
});

Task("Analyse")
    .IsDependentOn("Test")
    .Does(() => 
{
     SonarScanner(new SonarScannerSettings {
         Debug = true,
         Properties = new Dictionary<string, string> {
             { "sonar.login", sonarScannerToken },
             { "sonar.projectVersion", version.NuGetVersionV2 }
         }
     });
});

Task("Pack")
    .IsDependentOn("Analyse")
    .Does(() =>
{
    var nuGetPackSettings   = new NuGetPackSettings 
    {
        Id           = "Cake.SonarScanner",
        Version      = version.NuGetVersionV2,
        Authors      = new[] {"pitermarx"},
        Description  = "Cake aliases for SonarScanner. To be used in conjunction with the SonarScanner chocolatey package.",
        ProjectUrl   = new Uri("https://github.com/pitermarx/Cake.SonarScanner"),
        LicenseUrl   = new Uri("https://github.com/pitermarx/Cake.SonarScanner/blob/master/LICENSE"),
        Tags         = new [] {"cake","sonar","sonar-scanner"},
        IconUrl      = new Uri("https://cdn.rawgit.com/cake-contrib/graphics/a5cf0f881c390650144b2243ae551d5b9f836196/png/cake-contrib-medium.png"),
        Files        = new [] { 
            new NuSpecContent { Source = "Cake.SonarScanner/bin/Release/Cake.SonarScanner.dll", Target = "lib\\net45" },
            new NuSpecContent { Source = "Cake.SonarScanner/bin/Release/Cake.SonarScanner.xml", Target = "lib\\net45" },
            new NuSpecContent { Source = "Cake.SonarScanner/bin/Release/Cake.SonarScanner.pdb", Target = "lib\\net45" }
        },
        BasePath        = "./",
        OutputDirectory = "./nuget"
    };

    NuGetPack(nuGetPackSettings);
});

Task("Publish")
    .WithCriteria(AppVeyor.IsRunningOnAppVeyor)
    .IsDependentOn("Pack")
    .Does(() =>
{
    var file = GetFiles("nuget/*.nupkg").First();
    AppVeyor.UploadArtifact(file);

    var tagged = AppVeyor.Environment.Repository.Tag.IsTag && 
        !string.IsNullOrWhiteSpace(AppVeyor.Environment.Repository.Tag.Name);

    if (tagged)
    { 
        // Push the package.
        NuGetPush(file, new NuGetPushSettings 
        {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = nugetApiToken
        });
    }
});

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);