using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TYManager
{
    class NetworkClient
    {
        private static HttpClient httpClient;
        private static object obj = new object();
        public delegate void reqSucCallBack(string res);
        public delegate void reqFailCallBack(string res);
        public event reqSucCallBack sucNotifer;
        public event reqFailCallBack failNotifer;
        public NetworkClient()
        {
            if(httpClient == null)
            {
                lock (obj)
                {
                    if (httpClient == null) {
                        HttpClientHandler handler = new HttpClientHandler();
                        handler.MaxConnectionsPerServer = 20;
                        httpClient = new HttpClient(handler);
                        httpClient.Timeout = TimeSpan.FromSeconds(20);
                    }
                }
            }
        }

        public string BaseUrl { get; set; }

        public virtual async void HttpGetReq(string url, Dictionary<string,string> header)
        {
            HttpRequestMessage request = getRequest(url, HttpMethod.Get, header);

            Task <HttpResponseMessage> responseTask = httpClient.SendAsync(request);
            HttpResponseMessage response = null;
            try
            {
                response = await responseTask;
                ExecResult(response);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                ExecResult(null);
            }
            
        }

        public virtual async void HttpPostReq(string url,Dictionary<string,decimal>data, Dictionary<string, string> header)
        {
            HttpRequestMessage request = getRequest(url,HttpMethod.Post,header);

            HttpResponseMessage response = await httpClient.SendAsync(request);

            ExecResult(response);
        }

        private HttpRequestMessage getRequest(string url,HttpMethod method, Dictionary<string, string> header)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            if (header != null)
            {
                foreach (KeyValuePair<string, string> kvp in header)
                {
                    request.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            return request;
        }

        private async void ExecResult(HttpResponseMessage response)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                failNotifer.Invoke(response==null?"网络错误或超时":response.ReasonPhrase);
                return;
            }
            string result = await response.Content.ReadAsStringAsync();
            sucNotifer.Invoke(result);
        }

        public void client_dispose()
        {
            httpClient = null;
        }
    }

    class BaiduNetClient : NetworkClient
    {
        public BaiduNetClient()
        {
           this.BaseUrl = @"http://www.ranknowcn.com/";
        }

        public override void HttpGetReq(string url, Dictionary<string, string> header)
        {
            url = this.BaseUrl + url;
            base.HttpGetReq(url, header);
        }
    }
}
