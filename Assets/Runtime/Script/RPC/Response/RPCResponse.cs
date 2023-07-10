namespace AllArt.SUI.RPC.Response {


    public class JsonRpcResponse<T> : ResponseBase
    {
        public T result { get; set; }
    }

    public class ResponseData<T>
    {
        public T[] data { get; set; }
    }

    public class JsonRpcErrorResponse : ResponseBase
    {
        public ErrorContent Error { get; set; }
    }

    public class ErrorContent
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class ContextObj
    {
        public int Slot { get; set; }
    }

    public class ResponseValue<T>
    {
        public ContextObj Context { get; set; }

        public T Value { get; set; }
    }
}
