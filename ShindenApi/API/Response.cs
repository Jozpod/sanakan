using System.Net;

namespace Shinden.API
{
    public abstract class Response<T> : IResponse<T> where T : class
    {
        public static implicit operator T(Response<T> response) => response.Body;
        public static implicit operator HttpStatusCode(Response<T> response) => response.Code;

        public override string ToString()
        {
            return ToString("body");
        }

        public string ToString(string format)
        {
            switch(format.ToLower())
            {
                case "error": return ((int) Code).ToString();
                case "ename": return Code.ToString();

                case "body":
                default: return Body?.ToString();
            }
        }

        // IResponse<T>
        public T Body { get; protected set; }
        public HttpStatusCode Code { get; protected set; }

        public bool IsSuccessStatusCode()
        {
            var code = (int) Code;
            return code >= 200 && code < 300;
        }
    }
}