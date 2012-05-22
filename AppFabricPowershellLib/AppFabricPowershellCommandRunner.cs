using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.ApplicationServer.Caching;
using Microsoft.ApplicationServer.Caching.AdminApi;

namespace AppFabricPowershellLib
{
    public class AppFabricPowershellCommandRunner
    {
        private InitialSessionState _state;
        private Runspace _runspace;

        public AppFabricPowershellCommandRunner()
        {
            _state = InitialSessionState.CreateDefault();
            _state.ImportPSModule(new string[] { "DistributedCacheAdministration", "DistributedCacheConfiguration" });
            _state.ThrowOnRunspaceOpenError = true;
            _runspace = RunspaceFactory.CreateRunspace(_state);
            _runspace.Open();
        }

        public Collection<PSObject> Run(params Command[] commands)
        {
            var pipe = _runspace.CreatePipeline();
            foreach (var command in commands) {
                pipe.Commands.Add(command);
            }
            return pipe.Invoke();
        }

    }
}
