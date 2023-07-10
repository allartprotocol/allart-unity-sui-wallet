using System.Collections.Generic;

namespace AllArt.SUI.RPC.Response.Types {

    public class TransactionBlockBytes { 
        public string txBytes { get; set; }
        public List<ObjectRef> gas { get; set; }
        public List<InputObjectKind> inputObjects { get; set; }
    }

    public class InputObjectKind { 
        public ObjectRef ImmOrOwnedMoveObject { get; set; }
    }
}


