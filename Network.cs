using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TYManager
{
    class NetworkClient
    {
        public NetworkClient()
        {

        }

        public string BaseUrl { get; set; }

        public virtual async Task<string> HttpGetReq(string url, Dictionary<string,string> header)
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync(url);
            if(response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }
    }

    class BaiduNetClient : NetworkClient
    {
        private static BaiduNetClient Singleton = null;
        private static Object Singleton_Lock = new Object();
        public static BaiduNetClient DefaultClient()
        {
            if (Singleton == null)
            {
                lock (Singleton_Lock)
                {
                    if (Singleton == null)
                    {
                        Singleton = new BaiduNetClient();
                    }
                }
            }
            return Singleton;
        }

        public override async Task<string> HttpGetReq(string url, Dictionary<string, string> header)
        {

            return await base.HttpGetReq(url, header);
        }
    }
}
