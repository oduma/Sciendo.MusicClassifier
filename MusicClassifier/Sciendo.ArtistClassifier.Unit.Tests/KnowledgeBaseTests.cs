using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sciendo.MusicClassifier.KnowledgeBaseProvider;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.ArtistClassifier.Unit.Tests
{
    [TestClass]
    public class KnowledgeBaseTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void KnowledgeBaseFactoryDoesNotReceiveAFile()
        {
            KnowledgeBaseFactory knowledgeBaseFactory = new KnowledgeBaseFactory();
            knowledgeBaseFactory.GetKnowledgeBase("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void KnowledgeBaseFactoryReceivesANonExistingFile()
        {
            KnowledgeBaseFactory knowledgeBaseFactory = new KnowledgeBaseFactory();
            knowledgeBaseFactory.GetKnowledgeBase("abc.json");
        }
        [TestMethod]
        public void KnowledgeBaseFactoryReceivesAGoodFile()
        {
            KnowledgeBaseFactory knowledgeBaseFactory = new KnowledgeBaseFactory();
            var knowledgeBase = knowledgeBaseFactory.GetKnowledgeBase("newknowledgebase.json");
            Assert.IsNotNull(knowledgeBase);
            Assert.IsNotNull(knowledgeBase.Excludes);
            Assert.IsNotNull(knowledgeBase.FeaturedRules);
            Assert.IsNotNull(knowledgeBase.Rules);
            Assert.IsNotNull(knowledgeBase.Spliters);
            Assert.IsNotNull(knowledgeBase.Transforms);
        }
    }
}
