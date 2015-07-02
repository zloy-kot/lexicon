
namespace Lexicon.SimpleTextStorage
{
    public interface ISimpleSerializer<T>
    {
        T Deserialize(string objString);

        string Serialize(T obj);
    }
}
