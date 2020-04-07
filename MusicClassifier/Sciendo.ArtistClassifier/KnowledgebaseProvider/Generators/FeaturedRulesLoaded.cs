namespace Sciendo.MusicClassifier.KnowledgeBaseProvider.Generators
{
    public class FeaturedRulesLoaded
    {
        public char[] FeatureMarkers = new[]
        {
            '(',
            ')'
        };

        public string[] NonTitledInformationFromTitle = new[]
        {
            @"\(.?\)",
            @"\(part \d*\)",
            @"\(\d*\)",
            "(live)",
            "(instrumental)",
            "(original mix)"
        };

        public string[] PossibleAreasForFeaturedArtistsMarkers = new[]
        {
            @"\((.*?)\)",
            @"\[(.*?)\]"
        };

        public string[] FeatureMustContainWords = new[]
        {
            @"(?:^|\W)arr\.(?:$|\W)",
            @"(?:^|\W)feat(?:$|\W)",
            @"(?:^|\W)featuring(?:$|\W)",
            @"(?:^|\W)feat\.(?:$|\W)",
            @"(?:^|\W)ft\.(?:$|\W)",
            @"(?:^|\W)mixed(?:$|\W)",
            @"(?:^|\W)orch\.(?:$|\W)",
            @"(?:^|\W)presents(?:$|\W)",
            @"(?:^|\W)pres\.(?:$|\W)",
            @"(?:^|\W)prod\.(?:$|\W)",
            @"(?:^|\W)remixed(?:$|\W)",
            @"(?:^|\W)vocals(?:$|\W)",
            @"(?:^|\W)with(?:$|\W)",
            @"(?:^|\W)remix(?:$|\W)$",

        };

        public string[] WordsSeparatorsGlobal = new[]
        {
            "arr.",
            "by",
            "ch",
            "con",
            "demo",
            "edit",
            "extended",
            "feat",
            "feat.",
            "featuring",
            "ft",
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
            "remix",
            "remixed",
            "theme",
            "version",
            "vocal",
            "vocals",
            "vs",
            "vs.",
            "✖",
        };

        public string FeaturedArtistSanityCheckRegEx = "[a-z]{1}";
    }
}