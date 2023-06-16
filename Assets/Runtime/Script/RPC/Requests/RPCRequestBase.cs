using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AllArt.SUI.Requests
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RPCRequestBase {
        public string jsonrpc = "2.0";
        public int id = 1;
        public string method = "";
        public List<object> @params;
        public RPCRequestBase(string method, object[] args = default, int id = 1)
        {
            if (args != null) { 
                @params = args.ToList();
            }
            this.method = method;
            this.id = id;
        }

        public void AddParameter(object param)
        {
            if (@params == null)
            {
                @params = new List<object>();
            }
            @params.Add(param);
        }
    }

}
