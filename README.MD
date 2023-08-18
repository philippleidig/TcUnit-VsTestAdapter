# HEAVILY UNDER CONSTRUCTION :construction:

--------------------------------------------------------

# TcUnit-VsTestAdapter - TcUnit integration into Microsoft Visual Studio Test Plattform

TcUnit-VsTestAdapter makes it possible to execute [TcUnit](https://github.com/tcunit/TcUnit)
(TwinCAT unit tests) with the Microsoft [Visual Studio Test Plattform](https://github.com/microsoft/vstest)

For more details see [vstest test adapter extensibility](https://github.com/microsoft/vstest/blob/main/docs/RFCs/0004-Adapter-Extensibility.md)

## vstest.console.exe

### Nuget package - [Microsoft.TestPlatform](https://www.nuget.org/packages/Microsoft.TestPlatform) 
### or [Microsoft.TestPlatform.Portable](https://www.nuget.org/packages/Microsoft.TestPlatform.Portable)

### VS2019

Prerequisites

- Copy *.dll files to "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\Extensions\TestPlatform"


Execute

```
    cd "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\Extensions\TestPlatform"
    vstest.console.exe "path/to/xae.tsproj"
```

![vs-test-console.png](/assets/images/vs-test-console.png)

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
vstest.console.exe /Settings:path\to\TcUnit.runsettings
```

TcUnit.runsettings sample
```
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
    <TcUnit>
        <Target>127.0.0.1.1.1</Target> 
    </TcUnit>
</RunSettings>
```

### VS2017

```
    cd "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\Extensions\TestPlatform"
    vstest.console.exe "path/to/xae.tsproj"
```