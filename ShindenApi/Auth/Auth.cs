namespace Shinden
{
    public class Auth
    {
        public readonly string Token;
        public readonly string UserAgent;
        public readonly string Marmolade;

        public Auth(string token, string useragent, string marmolade = null)
        {
            Token = token;
            UserAgent = useragent;
            Marmolade = marmolade;
        }
    }
}
