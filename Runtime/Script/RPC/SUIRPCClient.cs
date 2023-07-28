using AllArt.SUI.Requests;
using AllArt.SUI.RPC.Filter.Types;
using AllArt.SUI.RPC.Response;
using AllArt.SUI.RPC.Response;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

namespace AllArt.SUI.RPC
{
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

        public async Task<JsonRpcResponse<List<Balance>>> GetAllBalances(AllArt.SUI.Wallets.Wallet wallet)
        {
            RPCRequestBase rpcRequest = new RPCRequestBase("suix_getAllBalances");
            rpcRequest.AddParameter(wallet.publicKey);
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

        public async Task<JsonRpcResponse<Balance>> GetBalance(AllArt.SUI.Wallets.Wallet wallet, string cointType)
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

        public async Task<JsonRpcResponse<PageForCoinAndObjectID>> GetAllCoins(AllArt.SUI.Wallets.Wallet wallet, string cursor = "", int limit = 100)
        {
            RPCRequestBase rpcRequest = new RPCRequestBase("suix_getAllCoins");
            rpcRequest.AddParameter(wallet.publicKey);
            rpcRequest.AddParameter(cursor);
            rpcRequest.AddParameter(limit);
            var rpcResponse = await SendRequestAsync<PageForCoinAndObjectID>(rpcRequest);
            return rpcResponse;
        }

        public async Task<JsonRpcResponse<PageForCoinAndObjectID>> GetAllCoins(string publicKey, string cursor = "", int limit = 100)
        {
            RPCRequestBase rpcRequest = new RPCRequestBase("suix_getAllCoins");
            rpcRequest.AddParameter(publicKey);
            //rpcRequest.AddParameter(cursor);
            //rpcRequest.AddParameter(limit);
            var rpcResponse = await SendRequestAsync<PageForCoinAndObjectID>(rpcRequest);
            return rpcResponse;
        }

        public async Task<JsonRpcResponse<CoinPage>> GetCoins(AllArt.SUI.Wallets.Wallet wallet, string cointType, string cursor = "", int limit = 100)
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

        public async Task<JsonRpcResponse<string>> GetReferenceGasPrice()
        {
            RPCRequestBase rpcRequest = new RPCRequestBase("suix_getReferenceGasPrice");
            var rpcResponse = await SendRequestAsync<string>(rpcRequest);
            return rpcResponse;
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

        public async Task<JsonRpcResponse<SuiTransactionBlockResponse>> ExecuteTransactionBlock(string txBytes, IEnumerable<string> serializedSignatures, TransactionBlockResponseOptions options, ExecuteTransactionRequestType requestType)
        {
            RPCRequestBase rpcRequest = new("sui_executeTransactionBlock");
            rpcRequest.AddParameter(txBytes);
            rpcRequest.AddParameter(serializedSignatures);
            rpcRequest.AddParameter(options);
            rpcRequest.AddParameter(requestType);
            var rpcResponse = await SendRequestAsync<SuiTransactionBlockResponse>(rpcRequest);
            return rpcResponse;
        }

        public async Task<JsonRpcResponse<SuiTransactionBlockResponse>> DryRunTransactionBlock(string txBytes)
        {
            RPCRequestBase rpcRequest = new("sui_dryRunTransactionBlock");
            rpcRequest.AddParameter(txBytes);
            var rpcResponse = await SendRequestAsync<SuiTransactionBlockResponse>(rpcRequest);
            Debug.LogError(JsonConvert.SerializeObject(rpcResponse));
            return rpcResponse;
        }

        public async Task<JsonRpcResponse<TransactionBlockBytes>> PaySui(AllArt.SUI.Wallets.Wallet signer, List<string> inputCoins, List<string> recipients, List<string> amounts, string gasBudget)
        {
            RPCRequestBase rpcRequest = new("unsafe_paySui");
            rpcRequest.AddParameter(signer.publicKey);
            rpcRequest.AddParameter(inputCoins);
            rpcRequest.AddParameter(recipients);
            rpcRequest.AddParameter(amounts);
            rpcRequest.AddParameter(gasBudget);
            var rpcResponse = await SendRequestAsync<TransactionBlockBytes>(rpcRequest);
            return rpcResponse;
        }

        public async Task<JsonRpcResponse<TransactionBlockBytes>> PayAllSui(AllArt.SUI.Wallets.Wallet signer, List<string> inputCoins, string recipients, string gasBudget)
        {

            RPCRequestBase rpcRequest = new("unsafe_payAllSui");
            rpcRequest.AddParameter(signer.publicKey);
            rpcRequest.AddParameter(inputCoins);
            rpcRequest.AddParameter(recipients);
            rpcRequest.AddParameter(gasBudget);
            var rpcResponse = await SendRequestAsync<TransactionBlockBytes>(rpcRequest);
            return rpcResponse;
        }

        public async Task<TransactionBlockBytes> Pay(AllArt.SUI.Wallets.Wallet signer, ObjectId[] inputCoins, SUIAddress[] recipients, BigInteger[] amounts, ObjectId gas, BigInteger gasBudget)
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

        public async Task<JsonRpcResponse<TransactionBlockBytes>> Pay(AllArt.SUI.Wallets.Wallet signer, string[] inputCoins, string[] recipients, string[] amounts, string gas, string gasBudget)
        {
            RPCRequestBase rpcRequest = new RPCRequestBase("unsafe_pay");
            rpcRequest.AddParameter(signer.publicKey);
            rpcRequest.AddParameter(inputCoins);
            rpcRequest.AddParameter(recipients);
            rpcRequest.AddParameter(amounts);
            rpcRequest.AddParameter(gas);
            rpcRequest.AddParameter(gasBudget);
            var rpcResponse = await SendRequestAsync<TransactionBlockBytes>(rpcRequest);
            return rpcResponse;
        }

        public async Task<JsonRpcResponse<PageForEventAndEventID>> QueryEvents(object filter)
        {
            RPCRequestBase rpcRequest = new("suix_queryEvents");
            rpcRequest.AddParameter(filter);
            var rpcResponse = await SendRequestAsync<PageForEventAndEventID>(rpcRequest);
            return rpcResponse;
        }

        public async Task<JsonRpcResponse<PageForTransactionBlockResponseAndTransactionDigest>> QueryTransactionBlocks(object filters)
        {
            RPCRequestBase rpcRequest = new("suix_queryTransactionBlocks");
            rpcRequest.AddParameter(filters);
            var rpcResponse = await SendRequestAsync<PageForTransactionBlockResponseAndTransactionDigest>(rpcRequest);
            return rpcResponse;
        }

        public async Task<List<SuiTransactionBlockResponse>> MultiGetTransactionBlocks(List<string> digests, TransactionBlockResponseOptions options)
        {
            RPCRequestBase rpcRequest = new("sui_multiGetTransactionBlocks");
            rpcRequest.AddParameter(digests);
            rpcRequest.AddParameter(options);
            var rpcResponse = await SendRequestAsync<List<SuiTransactionBlockResponse>>(rpcRequest);
            return rpcResponse.result;
        }
    }
}
