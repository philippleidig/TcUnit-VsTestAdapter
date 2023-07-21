# HEAVILY UNDER CONSTRUCTION :construction:

--------------------------------------------------------

# TcUnit-VsTestAdapter - TcUnit integration into Microsoft Visual Studio Test Plattform

TcUnit-VsTestAdapter makes it possible to execute [TcUnit](https://github.com/tcunit/TcUnit)
(TwinCAT unit tests) with the Microsoft [Visual Studio Test Plattform](https://github.com/microsoft/vstest)

For more details see [vstest test adapter extensibility](https://github.com/microsoft/vstest/blob/main/docs/RFCs/0004-Adapter-Extensibility.md)

## Visual Studio Extension (VSIX)

![vs-test-explorer](/assets/images/vs-test-explorer.png)


## vstest.console.exe

### VS2017

Execute

```
    cd "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\Extensions\TestPlatform"
    vstest.console.exe "path/to/xae.tsproj"
```

Determine if TcUnit test adapter is registered
```
    vstest.console.exe /listdiscoverers
    vstest.console.exe /listexecutors
```

```
vstest.console.exe /TestAdapterPath:"folder\to\testadapter"
```

Use custom run settings
```
vstest.console.exe /Settings:TcUnit.runsettings
```

runsettings sample
```
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
    <RunConfiguration>
        <ResultsDirectory>.\TestResults</ResultsDirectory>
        <TestAdaptersPaths>%SystemDrive%\Temp\foo</TestAdaptersPaths>
    </RunConfiguration>
    <TcUnit>
        <Target>127.0.0.1.1.1</Target>    
    </TcUnit>
</RunSettings>
```

### VS2019

```
    cd "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\Extensions\TestPlatform"
    vstest.console.exe "path/to/xae.tsproj"
```