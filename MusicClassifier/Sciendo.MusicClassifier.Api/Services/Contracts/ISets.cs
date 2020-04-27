using Sciendo.MusicClassifier.Api.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Services.Contracts
{
    public interface ISets
    {
        IEnumerable<string> GetAllSets();

        Dictionary<SetFileType, string> GetAllFilesInSet(string setName);
    }
}
