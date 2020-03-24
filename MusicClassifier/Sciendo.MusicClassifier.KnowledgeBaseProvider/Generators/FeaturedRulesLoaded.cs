namespace Sciendo.MusicClassifier.KnowledgeBaseProvider.Generators
{
    public class FeaturedRulesLoaded
    {
        public string[] NonTitledInformationFromTitle = new[]
        {
            @"\(.?\)",
            @"\(part \d*\)",
            @"\(\d*\)",
            "(live)",
            "(instrumental)",
            "(original mix)"
        };

        public string FeaturedArtistsInTheTitle = @"\((.*?)\)";

        public string[] WordsSeparatorsGlobal = new[]
        {
            "&",
            "and,",    
            "arr.",
            "by",
            "ch",
            "con",
            "demo",
            "edit",
            "extended",
            "feat",
            "featuring",
            "feat.",
            "ft.",
            "instrumental",
            "main",
            "mix",
            "mixed",
            "original",
            "orch.",
            "part",
            "presents",
            "pres.",
            "prod.",
            "radio",
            "remix",
            "remixed",
            "theme",
            "version",
            "vocal",
            "vocals",
            "vs",
            "vs.",
            "with",
            "x",
            "✖",
        };

    }
}