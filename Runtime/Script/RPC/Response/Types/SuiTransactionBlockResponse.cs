using System.Collections.Generic;
using System.Numerics;

namespace AllArt.SUI.RPC.Response {

    public class PageForTransactionBlockResponseAndTransactionDigest{
        public List<SuiTransactionBlockResponse> data;
    }

    [System.Serializable]
    public class SuiTransactionBlockResponse
    {
        public string digest;
        public TransactionBlock transaction;
        public string rawTransaction;
        public TransactionEffects effects;
        public object[] objectChanges;
        public List<BalanceChange> balanceChanges;
        public string timestampMs;
        public string checkpoint;
        public ObjectResponseError error;
        public string[] errors;
    }

    [System.Serializable]   
    public class ObjectResponseError{
        public int code;
        public string objectId;
        public string message;

    }

    [System.Serializable]
    public class BalanceChange {
        public Owner owner;
        public string amount;
        public string coinType;
    }

    [System.Serializable]
    public class TransactionBlock
    {
        public TransactionData data;
        public string[] txSignatures;
    }

    [System.Serializable]
    public class TransactionDigest{
        public string digest;

        public TransactionBlock transaction;
        public string rawTransaction;
        public TransactionEffects effects;
        public object[] objectChanges;
        public List<BalanceChange> balanceChanges;
        public string timestampMs;
        public string checkpoint;
        public ObjectResponseError error;
        public string[] errors;
    }

    [System.Serializable]
    public class TransactionData {
        public string messageVersion;
        public Transaction transaction;
        public string sender;
        public GasData gasData;

    }

    [System.Serializable]
    public class Transaction
    {
        public string kind;
        public TransactionInputs[] inputs;
        public object[] transactions;
        public string sender;
    }

    [System.Serializable]
    public class TransactionEffects
    {
        public string messageVersion;
        public TransactionStatus status;
        public string executedEpoch;
        public string gas;
        public GasUsed gasUsed;
        public TransactionObject[] mutated;
        public TransactionObject gasObject;
    }

    [System.Serializable]
    public class TransactionInputs
    {
        public string type;
        public string valueType;
        public string value;
    }

    [System.Serializable]
    public class TransactionObject
    {
        public Owner owner;
        public ObjectRef reference;
    }

    public class ObjectRef
    {
        public string objectId;
        public string version;
        public string digest;
    }

    [System.Serializable]
    public class Owner
    {
        public string AddressOwner;
    }

    [System.Serializable]
    public class TransactionStatus
    {
        public string status;
        public string error;
    }

    [System.Serializable]
    public class GasData
    {
        public string owner;
        public string price;
        public string budget;
    }

    [System.Serializable]
    public class GasUsed
    {
        public string computationCost;
        public string storageCost;
        public string storageRebate;
        public string nonRefundableStorageFee;
    }

    [System.Serializable]
    public class SUIEvent
    {
        public string bcs;
        public string id;
        public ObjectId packageId;
        public string parsedJson;
        public SUIAddress sender;
        public BigInteger timestampMs;
        public string transactionModule;
        public string type;
    }

    [System.Serializable]
    public class SUIAddress
    {
        public string value;
    }


}

