
![TcUnit logo](assets/images/TcUnit.png)
[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/philippleidig/TcUnit-VsTestAdapter/blob/main/LICENSE)
[![Open Source? Yes!](https://badgen.net/badge/Open%20Source%20%3F/Yes%21/blue?icon=github)](https://github.com/philippleidig/TcUnit-VsTestAdapter)

# TcUnit-VsTestAdapter

TcUnit-VsTestAdapter makes it possible to execute [TcUnit](https://github.com/tcunit/TcUnit)
(TwinCAT unit tests) with the Microsoft [Visual Studio Test Plattform](https://github.com/microsoft/vstest)

For more details see [vstest test adapter extensibility](https://github.com/microsoft/vstest/blob/main/docs/RFCs/0004-Adapter-Extensibility.md)

## Table of Contents

- [Usage](#usage)
    - [vstest.console.exe](#vstest.console.exe)
    - [Azure Pipelines](#azure-pipelines)
- [Best Practice](#best-practice)
- [RunSettings](#runsettings)

## Usage

This section describes how to execute TcUnit tests in Azure DevOps pipelines or locally with Microsoft.TestPlatform.

### vstest.console.exe

It is also possible to run TcUnit tests on a VM that has [vstest.console.exe](https://docs.microsoft.com/en-us/visualstudio/test/vstest-console-options?view=vs-2019) installed.

#### Setup Microsoft.TestPlatform on a VM

Let's assume that the working folder is `C:\Tools`.

1. Download and install [NuGet](https://www.nuget.org/downloads).
2. Install NuGet package [Microsoft.TestPlatform](https://www.nuget.org/packages/Microsoft.TestPlatform)

    ```
    nuget install Microsoft.TestPlatform
    ```
Find `vstest.console.exe` in `C:\Tools\Microsoft.TestPlatform.16.7.1\tools\net451\Common7\IDE\Extensions\TestPlatform`. 

3. Install [TcUnit.TestAdapter](https://www.nuget.org/packages/TcUnit.TestAdapter)

    ```
    nuget install TcUnit.TestAdapter
    ```
    
4. List all available tests (optional)

    ```
    vstest.console.exe /ListTests /TestAdapterPath:C:\Tools\TcUnit.TestAdapter.1.0.0\lib\netstandard2.0 /Settings:path\to\TcUnit.runsettings *\*.tsproj
    ```

5. Start test run and execute available tests

    ```
    vstest.console.exe /TestAdapterPath:C:\Tools\TcUnit.TestAdapter.1.0.0\lib\netstandard2.0 /Settings:path\to\TcUnit.runsettings *\*.tsproj
    ```

6. You may now create a *.cmd / *.ps1 file, put it inside your TcUnit tests project and run it after compile

    ![twincat-plc-deployment.png](/assets/images/twincat-plc-deployment.png)

    See [TwinCAT PLC Deployment](https://infosys.beckhoff.com/english.php?content=../content/1033/tc3_plc_intro/3260050187.html&id=1675861973834834662) for more information

### Azure Pipelines

- [Setup](https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/visual-studio-test-platform-installer-v1?view=azure-pipelines&viewFallbackFrom=azure-devops)
- [Execution](https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/vstest-v2?view=azure-pipelines&viewFallbackFrom=azure-devops)


1. Add [NuGet tool installer](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/tool/nuget?view=azure-devops) task to install NuGet.exe.
    
    Example:
    
    ```yaml
    steps:
    - task: NuGetToolInstaller@0
      displayName: 'Use NuGet'
      inputs:
        versionSpec: 4.4.1
    ```
3. Add [NuGet](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/package/nuget?view=azure-devops) task. Set **command** to `custom` and specify the command line:

    ```
    install TcUnit.TestAdapter -Version $(TcUnitTestAdapterVersion)
    ```
    
    Example
    
    ```yaml
    steps:
    - task: NuGetCommand@2
      displayName: 'NuGet Install TcUnit.TestAdapter'
      inputs:
        command: custom
        arguments: 'install TcUnit.TestAdapter -Version $(TcUnitTestAdapterVersion)'    
    ```
    
    In the pipeline settings set **TcUnitTestAdapterVersion** variable to the TcUnit.TestAdapter version you want to install (e.g. 1.0.0). The task will install TcUnit.TestAdapter into 
    
    ```
    $(Build.Repository.LocalPath)\TcUnit.TestAdapter.$(TcUnitTestAdapterVersion)\lib\netstandard2.0
    ```
4. If you plan to run tests on a self-hosted Windows Agent that does not have Visual Studio installed you need to add [Visual Studio Test Platform Installer](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/tool/vstest-platform-tool-installer?view=azure-devops) task.

    Example:
    
    ```yaml
    steps:
    - task: VisualStudioTestPlatformInstaller@1
      displayName: 'Visual Studio Test Platform Installer'
      inputs:
        versionSelector: latestStable
    ```

5. To run tests you need [Visual Studio Test](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/test/vstest?view=azure-devops) task.

    Example:
    
    ```yaml
    steps:
    - task: VSTest@2
      displayName: 'VsTest - Run TcUnit Tests'
      inputs:
        testAssemblyVer2: |
         $(Build.Repository.LocalPath)\SolutionFolder\ProjectFolder\*.tsproj
        runSettingsFile: '$(System.DefaultWorkingDirectory)\.ci\TcUnit.runsettings'
        pathtoCustomTestAdapters: '$(Build.Repository.LocalPath)\TcUnit.TestAdapter.$(TcUnitTestAdapterVersion)\lib\netstandard2.0'
        platform: '$(BuildPlatform)'
        configuration: '$(BuildConfiguration)'
    ```

    Make sure that path to custom test set adapters is set to
    
    ```
    $(Build.Repository.LocalPath)\TcUnit.TestAdapter.$(TcUnitTestAdapterVersion)\lib\netstandard2.0
    ```
    
6. To publish test results (for later review and downloading) use [Publish Build Artifacts](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/utility/publish-build-artifacts?view=azure-devops) task. Execution results are copied to `$(Agent.TempDirectory)\TestResults`.

    Example:
    
    ```yaml
    steps:
    - task: PublishBuildArtifacts@1
      displayName: 'Publish Artifact: TestResults'
      inputs:
        PathtoPublish: '$(Agent.TempDirectory)\TestResults'
        ArtifactName: TestResults
    ```

## Best Practice




## Filter  

TcUnit.TestAdapter supports [filter](https://github.com/Microsoft/vstest-docs/blob/master/docs/filter.md) 

e.g. only executes the specific test
```
vstest.console.exe /TestAdapterPath:C:\Tools\TcUnit.TestAdapter.1.0.0\lib\netstandard2.0 /Settings:path\to\TcUnit.runsettings *\*.tsproj /TestCaseFilter:"FullyQualifiedName=UnitTests.AxisGeneric_Tests.Should_Fail_When_AxisIdIsZero"
```

## RunSettings

Use custom run settings
```
vstest.console.exe /Settings:path\to\TcUnit.runsettings
```

[.runsettings file](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file) is used to pass parameters to the test run.

see [docs](https://learn.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2022) for detailed information


```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
    <RunConfiguration>
        <!-- Specify timeout in milliseconds. A valid value should be greater than 0 -->
        <TestSessionTimeout>20000</TestSessionTimeout>
    </RunConfiguration>
    <TcUnit>
        <Target>127.0.0.1.1.1</Target> 
        <CleanUpAfterTestRun>true</CleanUpAfterTestRun>
    </TcUnit>
</RunSettings>
```


