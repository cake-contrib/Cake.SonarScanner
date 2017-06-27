#tool "choco:?package=sonarcube-scanner&version=3.0.3.778&include=./**/*.bat"
#tool "nuget:?package=xunit.runner.console&version=2.1.0"
#tool "nuget:?package=GitVersion.CommandLine&version=3.6.2"
#tool "nuget:?package=OpenCover&version=4.6.519"
#tool "nuget:?package=coveralls.io&version=1.3.4"
#addin "nuget:?package=Cake.Coveralls&version=0.4.0"
#addin "nuget:?package=Cake.SonarScanner&version=0.1.0"

public class Parameters
{
    public static string Target;
    public static string Configuration;
    public static string SonarScannerToken;
    public static string NugetApiToken;
    public static string CoverallsToken;
    public static NuGetPackSettings NugetSettings;
    public static string ProjectName;
    public static string Version;
}

Parameters.Target = Argument("target", "Default");
Parameters.Configuration = Argument("configuration", "Release");
Parameters.SonarScannerToken = EnvironmentVariable("sonar_scanner_token");
Parameters.NugetApiToken = EnvironmentVariable("nuget_api_token");
Parameters.CoverallsToken = EnvironmentVariable("coveralls_token");
Parameters.Version = GitVersion(new GitVersionSettings { UpdateAssemblyInfo = true }).NuGetVersionV2;

if (AppVeyor.IsRunningOnAppVeyor) {
    AppVeyor.UpdateBuildVersion(Parameters.Version);
}

Task("Clean")
    .Does(() => 
{
    CleanDirectory("./nuget");
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
    if (FileExists("./" + Parameters.ProjectName + ".Tests.dll.xml"))
        DeleteFile("./" + Parameters.ProjectName + ".Tests.dll.xml");
    if (FileExists("./Coverage.xml"))
        DeleteFile("./Coverage.xml");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(Parameters.ProjectName + ".sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() => 
{
    MSBuild(Parameters.ProjectName + ".sln", configurator =>
        configurator
            .SetConfiguration(Parameters.Configuration)
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
        AppVeyor.UploadTestResults("./" + Parameters.ProjectName + ".Tests.dll.xml", AppVeyorTestResultsType.XUnit);
        if (!string.IsNullOrEmpty(Parameters.CoverallsToken))
        {
            CoverallsIo("Coverage.xml", new CoverallsIoSettings()
            {
                RepoToken = Parameters.CoverallsToken
            });
        }
    }
});

Task("Analyse")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor && !string.IsNullOrEmpty(Parameters.SonarScannerToken))
    .IsDependentOn("Test")
    .Does(() => 
{
     SonarScanner(new SonarScannerSettings {
         Debug = true,
         Properties = new Dictionary<string, string> {
             { "sonar.login", Parameters.SonarScannerToken },
             { "sonar.projectVersion", Parameters.Version }
         }
     });
});

Task("Pack")
    .WithCriteria(() => Parameters.NugetSettings != null)
    .IsDependentOn("Analyse")
    .Does(() =>
{
    Parameters.NugetSettings.Id = Parameters.NugetSettings.Id ?? Parameters.ProjectName;
    Parameters.NugetSettings.BasePath = Parameters.NugetSettings.BasePath ?? "./";
    Parameters.NugetSettings.OutputDirectory = Parameters.NugetSettings.OutputDirectory ?? "./nuget";
    Parameters.NugetSettings.Version = Parameters.Version;
    NuGetPack(Parameters.NugetSettings);
});

Task("Publish")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor && !string.IsNullOrEmpty(Parameters.NugetApiToken))
    .IsDependentOn("Pack")
    .Does(() =>
{
    var file = GetFiles("**/*.nupkg").First();
    AppVeyor.UploadArtifact(file);

    var tagged = AppVeyor.Environment.Repository.Tag.IsTag && 
        !string.IsNullOrWhiteSpace(AppVeyor.Environment.Repository.Tag.Name);

    if (tagged)
    { 
        // Push the package.
        NuGetPush(file, new NuGetPushSettings 
        {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = Parameters.NugetApiToken
        });
    }
});