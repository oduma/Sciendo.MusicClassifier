﻿namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    public class KnowledgeBase
    {
        public KnowledgeBase()
        {
            Excludes = new Excludes();
            Spliters = new Spliters();
            Transforms = new Transforms();
            Rules = new Rules();
            FeaturedRules = new FeaturedRules();
        }
        public Excludes Excludes { get; set; }

        public Spliters Spliters { get; set; }

        public Transforms Transforms { get; set; }

        public Rules Rules { get; set; }

        public FeaturedRules FeaturedRules { get; set; }
    }
}