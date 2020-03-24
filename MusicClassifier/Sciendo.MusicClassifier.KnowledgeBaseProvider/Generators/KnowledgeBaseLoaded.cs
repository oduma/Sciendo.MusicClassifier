namespace Sciendo.MusicClassifier.KnowledgeBaseProvider.Generators
{
    public class KnowledgeBaseLoaded
    {
        public KnowledgeBaseLoaded()
        {
            Excludes = new ExcludesLoaded();
            Spliters = new SplitersLoaded();
            Transforms = new TransformsLoaded();
            Rules = new RulesLoaded();
            FeaturedRules = new FeaturedRulesLoaded();
        }
        public ExcludesLoaded Excludes { get; set; }

        public SplitersLoaded Spliters { get; set; }

        public TransformsLoaded Transforms { get; set; }

        public RulesLoaded Rules { get; set; }

        public FeaturedRulesLoaded FeaturedRules { get; set; }

    }
}