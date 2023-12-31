
using System.Collections;
using AllArt.SUI.RPC.Response;

namespace AllArt.SUI.RPC.Filter.Types {
    public class ObjectResponseQuery { 
        public ObjectDataFilter filter { get; set; }
        public ObjectDataOptions options { get; set; }
    }

    public class SuiTransactionBlockResponseQuery {
        public object filter { get; set; }
        public TransactionBlockResponseOptions options { get; set; }
    }

}
