using Sanakan.Config.Model;

namespace Sanakan.Config
{
    public interface IConfig
    {
        void Save();
        ConfigModel Get();
    }
}
