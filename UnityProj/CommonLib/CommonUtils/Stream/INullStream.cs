
namespace Nullspace
{
    public interface INullStream
    {
        int SaveToStream(NullMemoryStream stream);
        bool LoadFromStream(NullMemoryStream stream);
    }
}
