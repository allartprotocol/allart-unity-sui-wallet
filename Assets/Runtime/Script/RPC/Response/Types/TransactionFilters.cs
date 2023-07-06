using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionFilters {
    public TransactionFilters(object filter)
    {
        this.filter = filter;
    }
    public object filter;
}

public class TransactionInputObject{

    public TransactionInputObject(string inputObject)
    {
        InputObject = inputObject;
    }

    public string InputObject { get; set; }
}

public class TransactionBlockResponseOptions {
    
    public TransactionBlockResponseOptions()
    {
        this.showInput = true;
        this.showRawInput = false;
        this.showEffects = true;
        this.showEvents = true;
        this.showObjectChanges = false;
        this.showBalanceChanges = true;
    }

    public TransactionBlockResponseOptions(bool showInput, bool showRawInput, bool showEffects, bool showEvents, bool showObjectChanges, bool showBalanceChanges)
    {
        this.showInput = showInput;
        this.showRawInput = showRawInput;
        this.showEffects = showEffects;
        this.showEvents = showEvents;
        this.showObjectChanges = showObjectChanges;
        this.showBalanceChanges = showBalanceChanges;
    }

    public bool showInput { get; set; }
    public bool showRawInput { get; set; }
    public bool showEffects { get; set; }
    public bool showEvents { get; set; }
    public bool showObjectChanges { get; set; }
    public bool showBalanceChanges { get; set; }


}
