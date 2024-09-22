using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace SafeFactory.Projects
{
    public class Project
    {
        public ObservableCollection<ViolationReport> Violations { get; } = [];

        public ProjectInfo Info { get; }

        public bool Processed { get; private set; }

        public Project(ProjectInfo info)
        {
            Info = info;
        }

        public async Task ProcessAsync()
        {
            Violations.Clear();
            throw new NotImplementedException();
            Processed = true;
        }

        public async Task ReadReportsAsync()
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(Info.ProjectPath, "Reports"), "*.json"))
            {
                string t = File.ReadAllText(file);
                var violation = JsonConvert.DeserializeObject<ViolationReport>(t);
                // TODO: add image clip.
                Violations.Add(violation);
            }
        }

        internal async Task RestoreAsync()
        {
            if (Processed)
                return;
            if (Directory.Exists(Path.Combine(Info.ProjectPath, "Reports")))
            {
                await ReadReportsAsync();
            }
            else
            {
                await ProcessAsync();
            }
        }
    }
}
