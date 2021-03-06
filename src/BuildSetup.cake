#tool "nuget:?package=xunit.runner.console&version=2.4.1"
#tool "nuget:?package=GitVersion.CommandLine&version=5.6.6"
#tool "nuget:?package=OpenCover&version=4.7.922"
#tool "nuget:?package=coveralls.io&version=1.4.2"
#addin "nuget:?package=Cake.Coveralls&version=1.0.0"
#addin "nuget:?package=Cake.SonarScanner&version=2.0.0-alpha0005"

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
Parameters.Version = GitVersion().NuGetVersionV2;

if (AppVeyor.IsRunningOnAppVeyor) {
    AppVeyor.UpdateBuildVersion(Parameters.Version);
}

Task("Clean")
    .Does(() =>
{
    CleanDirectory("./nuget");
    CleanDirectory("./TestResults");
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
    if (FileExists("./" + Parameters.ProjectName + ".Tests.dll.xml"))
        DeleteFile("./" + Parameters.ProjectName + ".Tests.dll.xml");
    if (FileExists("./Coverage.xml"))
        DeleteFile("./Coverage.xml");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreBuild(Parameters.ProjectName + ".sln", new DotNetCoreBuildSettings
    {
        Configuration = Parameters.Configuration,
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
     OpenCover(tool =>
        tool.
	       DotNetCoreTest("./Cake.SonarScanner.Tests/Cake.SonarScanner.Tests.csproj", new DotNetCoreTestSettings
           {
                Configuration = "Debug",
                ResultsDirectory = "TestResults",
                NoLogo = true,
                NoRestore = true
           }),
        "TestResults/Coverage.xml",
        new OpenCoverSettings
		{
			OldStyle = true,
			MergeOutput = true,
			ArgumentCustomization = args => args.Append("-returntargetcode"),
            Register = ""
		}.WithFilter("+[*]* -[xunit.*]* -[*.Tests]*")
    );

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        if (!string.IsNullOrEmpty(Parameters.CoverallsToken))
        {
            CoverallsIo("TestResults/Coverage.xml", new CoverallsIoSettings()
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
