namespace AllArt.SUI.RPC.Response {
    public class Balance {
        public uint coinObjectCount { get; set; }
        public string coinType { get; set; }
        public object lockedBalance { get; set; }
        public long totalBalance { get; set; }
    }
}
