using Microsoft.Extensions.Logging;
using Sciendo.MusicClassifier.Api.Configuration;
using Sciendo.MusicClassifier.Api.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Services
{
    public class Sets : ISets
    {
        private readonly ILogger<Sets> logger;
        private readonly SetsConfiguration setsConfiguration;

        public Sets(ILogger<Sets> logger, SetsConfiguration setsConfiguration)
        {
            this.logger = logger;
            this.setsConfiguration = setsConfiguration;
        }

        public Dictionary<SetFileType, string> GetAllFilesInSet(string setName)
        {
            var filesInSet = new Dictionary<SetFileType, string>();

            foreach(var fileType in setsConfiguration.SetFiles.Keys)
            {
                var path = Path.Combine(setsConfiguration.SetsLocation, setName, $"{setsConfiguration.SetFiles[fileType]}.csv");
                if (File.Exists(path))
                    filesInSet.Add(fileType, path);
            }
            return filesInSet;
        }

        public IEnumerable<string> GetAllSets()
        {
            return Directory.GetDirectories(setsConfiguration.SetsLocation).Select(d=>
            {
                var p = d.Split(Path.DirectorySeparatorChar);
                return p[p.Length - 1];
            });
        }
    }
}
