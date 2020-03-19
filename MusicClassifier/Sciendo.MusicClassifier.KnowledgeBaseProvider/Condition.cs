namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    public class Condition
    {
        public LengthConditons LengthConditions { get; set; }
        public string[] NonSplittingContent { get; set; }
        
        public int? Position { get; set; }
    }
}