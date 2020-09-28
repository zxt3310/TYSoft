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
        private static HttpClient httpClient = new HttpClient();
        public delegate void reqSucCallBack(string res);
        public delegate void reqFailCallBack(string res);
        public event reqSucCallBack sucNotifer;
        public event reqFailCallBack failNotifer;
        public NetworkClient()
        {

        }

        public string BaseUrl { get; set; }

        public virtual async void HttpGetReq(string url, Dictionary<string,string> header)
        {
            HttpRequestMessage request = getRequest(url, HttpMethod.Post, header);

            HttpResponseMessage response = await httpClient.SendAsync(request);
            if (response == null | response.StatusCode != HttpStatusCode.OK)
            {
                failNotifer.Invoke(response.ReasonPhrase);
            }
            string result = await response.Content.ReadAsStringAsync();
            sucNotifer.Invoke(result);
        }

        public virtual async void HttpPostReq(string url,Dictionary<string,decimal>data, Dictionary<string, string> header)
        {
            HttpRequestMessage request = getRequest(url,HttpMethod.Get,header);

            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response == null | response.StatusCode != HttpStatusCode.OK)
            {
                failNotifer.Invoke(response.ReasonPhrase);
            }
            string result = await response.Content.ReadAsStringAsync();
            sucNotifer.Invoke(result);
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
