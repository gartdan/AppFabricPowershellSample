using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.ApplicationServer.Caching;
using Microsoft.ApplicationServer.Caching.AdminApi;

using AppFabricPowershellLib;

namespace AppFabricPowershellSample
{
    class Program
    {
        static void Main(string[] args)
        {
            SampleNewCacheAndRemove();
            SampleGetCacheInfo();
            Console.ReadLine();
        }

        static void SampleNewCacheAndRemove()
        {
            var commandRunner = new AppFabricPowershellCommandRunner();
            var cmd2 = new Command("New-Cache");
            var cacheName = "MyTestCache123";
            cmd2.Parameters.Add("CacheName", cacheName);
            try {
                Console.WriteLine("Attempting to create a new cache: " + cacheName);
                commandRunner.Run(new Command("Use-CacheCluster"), cmd2);
            }
            catch (CmdletInvocationException ex) {
                Console.WriteLine(ex.ErrorRecord.ErrorDetails + " Error ID: " + ex.ErrorRecord.FullyQualifiedErrorId);
                var removeCmd = new Command("Remove-Cache");
                removeCmd.Parameters.Add("CacheName", cacheName);
                commandRunner.Run(new Command("Use-CacheCluster"), removeCmd);
            }
        }

        static void SampleGetCacheInfo()
        {
            var commandRunner = new AppFabricPowershellCommandRunner();
            var output = commandRunner.Run(new Command("Use-CacheCluster"), new Command("Get-Cache"));
            Console.WriteLine("Caches in cluster:");
            output.ToList().ForEach(o =>
            {
                CacheInfo ci = (CacheInfo)o.BaseObject;
                Console.WriteLine(ci.CacheName);
            });


            var cmd = new Command("Get-CacheHost");
            cmd.Parameters.Add(new CommandParameter("HostName", "localhost"));
            cmd.Parameters.Add(new CommandParameter("CachePort", 22233));
            var hostOutput = commandRunner.Run(new Command("Use-CacheCluster"), cmd);
            hostOutput.ToList().ForEach(h =>
            {
                HostInfo hi = (HostInfo)h.BaseObject;
                Console.WriteLine(hi.HostName + " | " + hi.PortNo + " | " + hi.ServiceName + " | " + hi.Status + " | " + hi.VersionInfo);
            });
        }

        static void RunGetCachePSCommand()
        {
            var state = InitialSessionState.CreateDefault();
            state.ImportPSModule(new string[] { "DistributedCacheAdministration", "DistributedCacheConfiguration" });
            state.ThrowOnRunspaceOpenError = true;

            var runspace = RunspaceFactory.CreateRunspace(state);
            runspace.Open();

            var pipe = runspace.CreatePipeline();
            pipe.Commands.Add(new Command("Use-CacheCluster"));
            pipe.Commands.Add(new Command("Get-Cache"));

            Collection<PSObject> output = pipe.Invoke();
            foreach (var o in output) {
                CacheInfo ci = (CacheInfo)o.BaseObject;
                Console.WriteLine(ci.CacheName);
            }

        }
    }
}
