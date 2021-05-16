# Cake.SonarScanner

Cake aliases for SonarScanner. To be used in conjunction with the sonar-scanner tool.

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Pre requisites

Ensure [sonar-scanner](https://docs.sonarqube.org/display/SCAN/Analyzing+with+SonarQube+Scanner) is on your path

Suggestion: use chocolatey. https://chocolatey.org/packages/sonarqube-scanner.portable

## Usage
```cs
#addin "nuget:?package=Cake.SonarScanner"
    
// Assuming a sonar-scanner.properties on the current directory
SonarScanner(new SonarScannerSettings {
    Properties = new Dictionary<string, string> {
        {"sonar.login", EnvironmentVariable("sonar_scanner_token")}
    }
});
```

## Important
.net projects should use the MSBuild scanner for SonarQube.
See https://github.com/AgileArchitect/Cake.Sonar

[![Build status](https://ci.appveyor.com/api/projects/status/l00o9jw5cxh68255?svg=true)](https://ci.appveyor.com/project/pitermarx/cake-sonarscanner)
[![NuGet](https://img.shields.io/nuget/v/Cake.SonarScanner.svg)](https://www.nuget.org/packages/Cake.SonarScanner/)
[![Coverage Status](https://coveralls.io/repos/github/pitermarx/Cake.SonarScanner/badge.svg?branch=master)](https://coveralls.io/github/pitermarx/Cake.SonarScanner?branch=master)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=pitermarx%3ACake.SonarScanner&metric=alert_status)](https://sonarcloud.io/dashboard/index/pitermarx:Cake.SonarScanner)

## Discussion

For questions and to discuss ideas & feature requests, use the [GitHub discussions on the Cake GitHub repository](https://github.com/cake-build/cake/discussions), under the [Extension Q&A](https://github.com/cake-build/cake/discussions/categories/extension-q-a) category.

[![Join in the discussion on the Cake repository](https://img.shields.io/badge/GitHub-Discussions-green?logo=github)](https://github.com/cake-build/cake/discussions)

## Release History

Click on the [Releases](https://github.com/cake-contrib/Cake.SonarScanner/releases) tab on GitHub.

---

_Copyright &copy; 2017-2021 Cake Contributors - Provided under the [MIT License](LICENSE)._
