namespace Sciendo.ArtistEnhancer.KnowledgeBaseProvider
{
    public interface IKnowledgeBaseFactory
    {
        KnowledgeBase GetKnowledgeBase(string file);
    }
}