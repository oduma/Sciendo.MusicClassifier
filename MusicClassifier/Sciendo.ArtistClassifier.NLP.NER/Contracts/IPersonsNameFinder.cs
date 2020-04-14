using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.ArtistClassifier.NLP.NER.Contracts
{
    public interface IPersonsNameFinder
    {
        IEnumerable<string> FindPersonNames(string text);
    }
}
