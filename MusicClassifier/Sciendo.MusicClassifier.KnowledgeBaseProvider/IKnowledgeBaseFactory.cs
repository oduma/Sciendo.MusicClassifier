namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    public interface IKnowledgeBaseFactory
    {
        KnowledgeBase GetKnowledgeBase(string file);
    }
}