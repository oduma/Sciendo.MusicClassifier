using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sciendo.ArtistClassifier.Logic;
using Sciendo.MusicClassifier.KnowledgeBaseProvider;
using System;

namespace Sciendo.ArtistClassifier.Unit.Tests
{
    [TestClass]
    public class ArtistProcessorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetAristsWithNoInput()
        {
            var artistProcessor = new ArtistProcessor(new KnowledgeBaseFactory(), "newknowledgebase.json");
            var artists = artistProcessor.GetArtists(null, true, true);
        }
    }
}
