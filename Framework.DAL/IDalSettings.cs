
namespace Framework.DAL
{
    public interface IDalSettings
    {
        string ConnectionString { get; }
        int CommandTimeout { get; }
    }
}
