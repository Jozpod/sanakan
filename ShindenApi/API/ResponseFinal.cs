using System.Net;

namespace Shinden.API
{
    public class ResponseFinal<T> : Response<T> where T : class
    {
        public ResponseFinal(int Code, T Body = null)
        {
            this.Body = Body;
            this.Code = (HttpStatusCode)Code;
        }

        public ResponseFinal(HttpStatusCode Code, T Body = null)
        {
            this.Code = Code;
            this.Body = Body;
        }

        public void SetCode(int Code) => this.Code = (HttpStatusCode)Code;
        public void SetCode(HttpStatusCode Code) => this.Code = Code;
        public void SetBody(T Body) => this.Body = Body;
    }
}