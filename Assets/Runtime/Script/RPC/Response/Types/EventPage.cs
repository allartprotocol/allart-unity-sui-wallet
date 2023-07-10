using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllArt.SUI.RPC.Response.Types{
    public class PageForEventAndEventID
    {
        public List<EventPage> data;
    }

    public class EventPage {
        public string packageId;
        public string transactionModule;
        public string sender;
        public string type;
        public string timestampMs;
    }

    public class Transactions {
        public string hash;
        public string block_height;
        public string timestamp;
        public string sender;
        public string recipient;
        public string amount;
    }

}
