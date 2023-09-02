﻿using System;

namespace TcUnit.TestAdapter
{
    public static class TestAdapter
    {
        public const string ExecutorUriString = "executor://TcUnitTestExecutor";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        public const string PlcProjFileExtension = ".plcproj";
        public const string TsProjFileExtension = ".tsproj";

        public const string TestResultPath = @"tcunit_testresults.xml"; 

        public const string RunSettingsName = "TcUnit";
    }
}