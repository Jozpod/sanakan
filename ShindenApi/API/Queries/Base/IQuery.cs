using Shinden.Logger.In;

namespace Shinden.API
{
    public interface IQuery<T> : IRequest where T : class
    {
        string Uri { get; }
        string BaseUri { get; }
        
        T Parse(string json);
        void WithToken(string token);
    }
}
