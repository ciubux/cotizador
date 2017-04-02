
namespace Framework.DAL.Settings.Implementations
{
    public class CotizadorSettings : DalSettingsBase, IDalSettings
    {
        private const string ConnectionStringAttribute = "DefaultConnection";
        private const string CommandTimeoutAttribute = "CommandTimeout";

        public CotizadorSettings() : base(ConnectionStringAttribute, CommandTimeoutAttribute)
        {
        }
    }
}
