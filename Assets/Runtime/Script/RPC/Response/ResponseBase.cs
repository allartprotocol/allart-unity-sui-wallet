
namespace AllArt.SUI.RPC.Response {
    public class ResponseBase {
        public string jsonrpc;
        public int id;
        public RPCErrorMessage error;
    }

    public class RPCErrorMessage {
        public int code;
        public string message;
    }
}
