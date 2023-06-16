using System.Numerics;

public class SuiTransactionBlockResponse
{
    public string digest { get; set; }
    public TransactionBlock transaction { get; set; }
    public string rawTransaction { get; set; }
    public TransactionEffects effects { get; set; }
    public object[] objectChanges { get; set; }
}

public class TransactionBlock
{
    public Transaction data { get; set; }
    public string[] txSignatures { get; set; }
}


public class Transaction
{
    public string kind { get; set; }
    public TransactionInputs[] inputs { get; set; }
    public object[] transactions { get; set; }
    public string sender { get; set; }
}

public class TransactionEffects
{
    public string messageVersion { get; set; }
    public TransactionStatus status { get; set; }
    public string executedEpoch { get; set; }
    public GasUsed gasUsed { get; set; }
    public TransactionObject[] mutated { get; set; }
    public TransactionObject gasObject { get; set; }
}

public class TransactionInputs
{
    public string type { get; set; }
    public string valueType { get; set; }
    public string value { get; set; }
}

public class TransactionObject
{
    public Owner owner { get; set; }
    public ObjectRef reference { get; set; }
}

public class ObjectRef
{
    public string objectId { get; set; }
    public int version { get; set; }
    public string digest { get; set; }
}

public class Owner
{
    public string AdressOwner { get; set; }
}

public class TransactionStatus
{
    public string status;
}

public class GasData
{
    public string owner { get; set; }
    public string price { get; set; }
    public string budget { get; set; }
}

public class GasUsed
{
    public string computationCost { get; set; }
    public string storageCost { get; set; }
    public string storageRebate { get; set; }
    public string nonRefundableStorageFees { get; set; }
}

public class BalanceChange
{
    public string amount { get; set; }
    public string coinType { get; set; }
    public Owner owner { get; set; }
}

public class SUIEvent
{
    public string bcs { get; set; }
    public string id { get; set; }
    public ObjectId packageId { get; set; }
    public string parsedJson { get; set; }
    public SUIAddress sender { get; set; }
    public BigInteger timestampMs { get; set; }
    public string transactionModule { get; set; }
    public string type { get; set; }
}

public class SUIAddress
{
    public string value { get; set; }
}


