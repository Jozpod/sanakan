namespace Shinden.Models.Entities
{
    public class EmptyResponse : IEmptyResponse
    {
        public EmptyResponse(string Message)
        {
            this.Message = Message;
        }

        public override string ToString() => Message;

        // IEmptyResponse
        public string Message { get; }
    }
}