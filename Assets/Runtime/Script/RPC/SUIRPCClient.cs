using AllArt.SUI.Requests;
using Solnet.Rpc.Messages;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

public class SUIRPCClient : RPCClient
{
    public SUIRPCClient(string uri) : base(uri) { }

    public async Task<JsonRpcResponse<T>> SendRequestAsync<T>(object data)
    {
        return await SendRequest<T>(data);
    }

    public async Task<JsonRpcResponse<T>> SendRequestAsync<T>(RPCRequestBase data)
    {
        return await SendRequest<T>(data);
    }

    public async Task<JsonRpcResponse<T>> SendRequestAsync<T>(string method, params object[] parameters)
    {
        return await SendRequest<T>(new RPCRequestBase(method, parameters));
    }

    public async Task<JsonRpcResponse<List<Balance>>> GetAllBalances(Wallet wallet)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getAllBalances");
        rpcRequest.AddParameter(wallet.publicKey);
        Debug.Log(rpcRequest);
        var rpcResponse = await SendRequestAsync<List<Balance>>(rpcRequest);
        return rpcResponse;
    }

    public async Task<JsonRpcResponse<List<Balance>>> GetAllBalances(string publicKey)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getAllBalances");
        rpcRequest.AddParameter(publicKey);
        var rpcResponse = await SendRequestAsync<List<Balance>>(rpcRequest);
        return rpcResponse;
    }

    public async Task<JsonRpcResponse<Balance>> GetBalance(Wallet wallet, string cointType)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getBalance");
        rpcRequest.AddParameter(wallet.publicKey);
        rpcRequest.AddParameter(cointType);
        var rpcResponse = await SendRequestAsync<Balance>(rpcRequest);
        return rpcResponse;
    }

    public async Task<JsonRpcResponse<Balance>> GetBalance(string publicKey, string cointType)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getBalance");
        rpcRequest.AddParameter(publicKey);
        rpcRequest.AddParameter(cointType);
        var rpcResponse = await SendRequestAsync<Balance>(rpcRequest);
        return rpcResponse;
    }

    public async Task<JsonRpcResponse<List<CoinPage>>> GetAllCoins(Wallet wallet, string cursor = "", int limit = 100)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getAllCoins");
        rpcRequest.AddParameter(wallet.publicKey);
        rpcRequest.AddParameter(cursor);
        rpcRequest.AddParameter(limit);
        var rpcResponse = await SendRequestAsync<List<CoinPage>>(rpcRequest);
        return rpcResponse;
    }

    public async Task<JsonRpcResponse<List<CoinPage>>> GetAllCoins(string publicKey, string cursor = "", int limit = 100)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getAllCoins");
        rpcRequest.AddParameter(publicKey);
        rpcRequest.AddParameter(cursor);
        rpcRequest.AddParameter(limit);
        var rpcResponse = await SendRequestAsync<List<CoinPage>>(rpcRequest);
        return rpcResponse;
    }

    public async Task<JsonRpcResponse<CoinPage>> GetCoins(Wallet wallet, string cointType, string cursor = "", int limit = 100)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getCoins");
        rpcRequest.AddParameter(wallet.publicKey);
        rpcRequest.AddParameter(cointType);
        rpcRequest.AddParameter(cursor);
        rpcRequest.AddParameter(limit);
        var rpcResponse = await SendRequestAsync<CoinPage>(rpcRequest);
        return rpcResponse;
    }

    public async Task<JsonRpcResponse<CoinMetadata>> GetCoinMetadata(string coinType)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getCoinMetadata");
        rpcRequest.AddParameter(coinType);
        var rpcResponse = await SendRequestAsync<CoinMetadata>(rpcRequest);
        return rpcResponse;
    }

    public async Task<BigInteger> GetReferenceGasPrice()
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getReferenceGasPrice");
        var rpcResponse = await SendRequestAsync<string>(rpcRequest);
        return BigInteger.Parse(rpcResponse.result);
    }

    public async Task<SUIObjectResponse> GetObject(ObjectId objectId)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getObject");
        rpcRequest.AddParameter(objectId.id);
        ObjectDataOptions objectDataOptions = new ObjectDataOptions();
        rpcRequest.AddParameter(objectDataOptions);
        var rpcResponse = await SendRequestAsync<SUIObjectResponse>(rpcRequest);
        return rpcResponse.result;
    }

    public async Task<SUIObjectResponse> GetObject(string objectId)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getObject");
        rpcRequest.AddParameter(objectId);
        ObjectDataOptions objectDataOptions = new ObjectDataOptions();
        rpcRequest.AddParameter(objectDataOptions);
        var rpcResponse = await SendRequestAsync<SUIObjectResponse>(rpcRequest);
        return rpcResponse.result;
    }

    public async Task<Page_for_SuiObjectResponse_and_ObjectID> GetOwnedObjects(string address, ObjectResponseQuery query, string objectId, uint limit)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getOwnedObjects");
        rpcRequest.AddParameter(address);
        rpcRequest.AddParameter(query);
        rpcRequest.AddParameter(objectId);
        rpcRequest.AddParameter(limit);
        var rpcResponse = await SendRequestAsync<Page_for_SuiObjectResponse_and_ObjectID>(rpcRequest);
        return rpcResponse.result;
    }

    public async Task<Supply> GetTotalSupply(string coinType)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("suix_getTotalSupply");
        rpcRequest.AddParameter(coinType);
        var rpcResponse = await SendRequestAsync<Supply>(rpcRequest);
        return rpcResponse.result;
    }

    public async Task<SuiTransactionBlockResponse> ExecuteTransactionBlock(object[] args)
    {

        RPCRequestBase rpcRequest = new RPCRequestBase("sui_executeTransactionBlock", args);
        var rpcResponse = await SendRequestAsync<SuiTransactionBlockResponse>(rpcRequest);
        return rpcResponse.result;
    }

    public async Task<SuiTransactionBlockResponse> ExecuteTransactionBlock(string txBytes, IEnumerable<string> serializedSignatures, ObjectDataOptions options, ExecuteTransactionRequestType requestType)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("sui_executeTransactionBlock");
        rpcRequest.AddParameter(txBytes);
        rpcRequest.AddParameter(serializedSignatures);
        rpcRequest.AddParameter(options);
        rpcRequest.AddParameter(requestType);
        var rpcResponse = await SendRequestAsync<SuiTransactionBlockResponse>(rpcRequest);
        return rpcResponse.result;
    }

    public async Task<TransactionBlockBytes> PaySui(Wallet signer, string inputCoins, string recipients, ulong amounts, string gasBudget)
    {

        RPCRequestBase rpcRequest = new RPCRequestBase("unsafe_transferSui");
        rpcRequest.AddParameter(signer.publicKey);
        rpcRequest.AddParameter(inputCoins);
        rpcRequest.AddParameter(gasBudget);
        rpcRequest.AddParameter(recipients);
        rpcRequest.AddParameter(amounts.ToString());
        var rpcResponse = await SendRequestAsync<TransactionBlockBytes>(rpcRequest);
        return rpcResponse.result;
    }

    public async Task<TransactionBlockBytes> Pay(Wallet signer, ObjectId[] inputCoins, SUIAddress[] recipients, BigInteger[] amounts, ObjectId gas, BigInteger gasBudget)
    {
        RPCRequestBase rpcRequest = new RPCRequestBase("unsafe_pay");
        rpcRequest.AddParameter(signer.publicKey);
        rpcRequest.AddParameter(inputCoins);
        rpcRequest.AddParameter(recipients);
        rpcRequest.AddParameter(amounts);
        rpcRequest.AddParameter(gas);
        rpcRequest.AddParameter(gasBudget);
        var rpcResponse = await SendRequestAsync<TransactionBlockBytes>(rpcRequest);
        return rpcResponse.result;
    }

}