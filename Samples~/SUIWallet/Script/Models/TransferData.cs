
namespace AllArt.SUI.RPC.Response {
    public class TransferData { 
        public string to;
        public string amount;
        public CoinMetadata coin;
        public bool transferAll;
        public JsonRpcResponse<SuiTransactionBlockResponse> response;
        public ulong transferAmount;
    }
}
