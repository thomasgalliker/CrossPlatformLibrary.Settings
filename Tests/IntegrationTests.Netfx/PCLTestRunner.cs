﻿using System;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Runners;

namespace IntegrationTests.Netfx
{
    [Collection("SettingsService")]
    public class PCLTestRunner
    {
        private static readonly ManualResetEvent finished = new ManualResetEvent(false);

        private readonly ITestOutputHelper testOutputHelper;
        private bool isTestFailed = false;

        public PCLTestRunner(ITestOutputHelper testOutputHelper)
        {
            finished.Reset();
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void RunAllPCLTests()
        {
            var testAssembly = this.AssemblyLocation(Assembly.Load("CrossPlatformLibrary.Settings.IntegrationTests"));

            using (var runner = AssemblyRunner.WithAppDomain(testAssembly))
            {
                runner.OnDiscoveryComplete = this.OnDiscoveryComplete;
                runner.OnExecutionComplete = this.OnExecutionComplete;
                runner.OnTestFailed = this.OnTestFailed;
                runner.OnTestSkipped = this.OnTestSkipped;

                runner.Start(null, null, null, null, false, null);

                finished.WaitOne();
                finished.Dispose();

                this.isTestFailed.Should().BeFalse("at least one test failed!");
            }
        }

        private void OnTestSkipped(TestSkippedInfo info)
        {
            this.testOutputHelper.WriteLine("[SKIP] {0}: {1}", info.TestDisplayName, info.SkipReason);
        }

        private void OnTestFailed(TestFailedInfo info)
        {
            this.testOutputHelper.WriteLine("[FAIL] {0}: {1}", info.TestDisplayName, info.ExceptionMessage);
        }

        private void OnExecutionComplete(ExecutionCompleteInfo info)
        {
            this.testOutputHelper.WriteLine($"Finished: {info.TotalTests} tests in {Math.Round(info.ExecutionTime, 3)}s ({info.TestsFailed} failed, {info.TestsSkipped} skipped)");
            if (info.TestsFailed > 0)
            {
                this.isTestFailed = true;
            }
            else
            {
                this.isTestFailed = false;
            }
            finished.Set();
        }

        private void OnDiscoveryComplete(DiscoveryCompleteInfo info)
        {
            this.testOutputHelper.WriteLine($"Running {info.TestCasesToRun} test cases...");
        }

        private string AssemblyLocation(Assembly testAssembly)
        {
            var codebase = new Uri(testAssembly.CodeBase);
            var path = codebase.LocalPath;
            return path;
        }
    }
}
