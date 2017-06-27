# Cake.SonarScanner

## Pre requisites
Either use [Cake.Chocolatey.Module](https://github.com/gep13/Cake.Chocolatey.Module) or ensure [sonar-scanner](https://docs.sonarqube.org/display/SCAN/Analyzing+with+SonarQube+Scanner) is on your path

## Usage
```cs
#tool choco:?package=sonarcube-scanner&version=3.0.3.778
#addin nuget:?package=Cake.SonarScanner
    
// Assuming a sonar-scanner.properties on the current directory
SonarScanner();
```

[![Build status](https://ci.appveyor.com/api/projects/status/l00o9jw5cxh68255?svg=true)](https://ci.appveyor.com/project/pitermarx/cake-sonarscanner)
[![NuGet](https://img.shields.io/nuget/v/Cake.SonarScanner.svg)](https://www.nuget.org/packages/Cake.SonarScanner/)
