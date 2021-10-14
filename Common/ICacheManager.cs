namespace Sanakan.Common
{
    public interface ICacheManager
    {
        void ExpireTag(params string[] tags);
        void AddTag(params string[] tags);
        T? Get<T>(string key);
    }
}
