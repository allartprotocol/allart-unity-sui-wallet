using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllArt.SUI.RPC.Response {

    public class SUIObjectResponse {
        public ObjectData data;
        public ObjectResponseError error;
    }

    public class ObjectData
    { 
        public string objectId;
        public string version;
        public string digest;
        public string type;
        public Owner owner;
        public string previousTransaction;
        public string storageRebate;
        public Content content;
    }

    public class  Content
    {
        public string dataType;
        public string type;
        public bool hasPublicTransfer;
        public Fields fields;
    }

    public class Fields
    {
        public string balance;
        public ObjectId id;
    }

    public class ObjectDataOptions {

        public ObjectDataOptions()
        {        
            showType = true;
            showOwner = true;
            showPreviousTransaction = true;
            showDisplay = true;
            showContent = true;
            showBcs = true;
            showStorageRebate = true;
        }

        public ObjectDataOptions(bool showType, bool showOwner, bool showPreviousTransaction, bool showDisplay, bool showContent, bool showBcs, bool showStorageRebate)
        {
            this.showType = showType;
            this.showOwner = showOwner;
            this.showPreviousTransaction = showPreviousTransaction;
            this.showDisplay = showDisplay;
            this.showContent = showContent;
            this.showBcs = showBcs;
            this.showStorageRebate = showStorageRebate;
        }

        public bool showType;
        public bool showOwner;
        public bool showPreviousTransaction;
        public bool showDisplay;
        public bool showContent;
        public bool showBcs;
        public bool showStorageRebate;
    }
}

