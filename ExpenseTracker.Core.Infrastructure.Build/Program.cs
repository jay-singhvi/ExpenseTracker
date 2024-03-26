// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV3s;
using System.Collections.Generic;
using System.IO;

namespace ExpenseTracker.Core.Infrastructure.Build
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var adotNetClient = new ADotNetClient();

            var githubPipeline = new GithubPipeline
            {
                Name = "Expense Tracker Build",

                OnEvents = new Events
                {
                    PullRequest = new PullRequestEvent
                    {
                        Branches = new string[] { "master" }
                    },

                    Push = new PushEvent
                    {
                        Branches = new string[] { "master" }
                    }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "build",
                        new Job
                        {
                            RunsOn = BuildMachines.WindowsLatest,

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV3
                                {
                                    Name = "Checking Out Code"
                                },

                                new SetupDotNetTaskV3
                                {
                                    Name = "Installing .NET",
                                    With = new TargetDotNetVersionV3
                                    {
                                        DotNetVersion = "8.0.203"
                                    }
                                },

                                new RestoreTask
                                {
                                    Name = "Restoring NuGet Packages"
                                },

                                new DotNetBuildTask
                                {
                                    Name = "Building Project"
                                },

                                new TestTask
                                {
                                    Name = "Running Tests"
                                }
                            }
                        }
                    }
                }
            };

            string buildScriptPath = "../../../../.github/workflows/dotnet.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            adotNetClient.SerializeAndWriteToFile(
                adoPipeline: githubPipeline,
                path: buildScriptPath);
        }
    }
}
