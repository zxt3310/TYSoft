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
using System.IO;

namespace TYManager
{
    class NetworkClient
    {
        private static HttpClient httpClient;
        private static object obj = new object();
        public delegate void reqSucCallBack(string res);
        public delegate void reqFailCallBack(string res);
        public delegate void downloadPrograss(long? total, int length);
        public event reqSucCallBack sucNotifer;
        public event reqFailCallBack failNotifer;
        public event downloadPrograss downloadhook;
        private const int BufferSize = 8192;
        public NetworkClient()
        {
            if(httpClient == null)
            {
                lock (obj)
                {
                    if (httpClient == null) {
                        HttpClientHandler handler = new HttpClientHandler();
                        handler.MaxConnectionsPerServer = 50;//并发
                        httpClient = new HttpClient(handler);
                        httpClient.Timeout = TimeSpan.FromSeconds(20);
                    }
                }
            }
        }

        public string BaseUrl { get; set; }

        /// <summary>
        /// start a request with GET 
        /// </summary>
        /// <param name="url">Absolute url</param>
        /// <param name="header">custom HTTP header addition null_able</param>
        public virtual void HttpGetReq(string url, Dictionary<string,string> header)
        {
            HttpRequestMessage request = getRequest(url, HttpMethod.Get, header);

            startHttpRequest(request);
            
        }
        /// <summary>
        /// start a request with POST
        /// </summary>
        /// <param name="url">Absolute url</param>
        /// <param name="data">Body</param>
        /// <param name="header">custom HTTP header addition null_able</param>
        public virtual void HttpPostReq(string url,Object data, Dictionary<string, string> header)
        {
            HttpRequestMessage request = getRequest(url,HttpMethod.Post,header);

            string postStr = JsonConvert.SerializeObject(data);

            StringContent body = new StringContent(postStr,Encoding.UTF8);

            request.Content = body;

            startHttpRequest(request);
        }

        /// <summary>
        /// start a download request
        /// </summary>
        /// <param name="url">Absolute url</param>
        /// <param name="header">custom HTTP header addition null_able</param>
        /// <param name="path">File path</param>
        public virtual async void HttpDownload(string url,Dictionary<string,string> header,string path)
        {
            HttpRequestMessage request = getRequest(url, HttpMethod.Get, header);


            HttpResponseMessage res;
            try
            {
                res = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var content = res.Content;
                if(content == null)
                {
                    failNotifer.Invoke(@"Download faild.File not exist");
                    LogHelper.WriteLog(res.RequestMessage.RequestUri.AbsoluteUri + " download faild");
                    return;
                }
                var headers = content.Headers;
                long? length = headers.ContentLength;
                using (var responseStream = await content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    LogHelper.WriteLog("Start downloading " + res.RequestMessage.RequestUri.AbsoluteUri);

                    var buffer = new byte[BufferSize];
                    int bytesRead;
                    var bytes = new List<byte>();
                    FileStream fileStream = new FileStream(path, FileMode.Create);
                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, BufferSize).ConfigureAwait(false)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        bytes.AddRange(buffer.Take(bytesRead));
                        if (downloadhook != null)
                        {
                            downloadhook.Invoke(length, bytes.Count);
                        }
                        Console.WriteLine(string.Format("{0}/{1}", bytes.Count, length));
                    }
                    if (sucNotifer != null)
                    {
                        sucNotifer.Invoke("successed!");
                    }
                    fileStream.Close();
                }
            }
            catch(Exception ex)
            {
                failNotifer.Invoke(ex.Message);
                LogHelper.WriteLog(ex.Message, ex);
            }
            
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

        private async void startHttpRequest(HttpRequestMessage request)
        {
            Task<HttpResponseMessage> responseTask = httpClient.SendAsync(request);
            HttpResponseMessage response = null;
            try
            {
                response = await responseTask;
                ExecResult(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogHelper.WriteLog(ex.Message, ex);
                ExecResult(null);
            }
        }
        private async void ExecResult(HttpResponseMessage response)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                if (failNotifer != null)
                {
                    failNotifer.Invoke(response == null ? "Net error or Time out!" : response.ReasonPhrase);
                }
                return;
            }
            string result = await response.Content.ReadAsStringAsync();
            if (sucNotifer != null)
            {
                sucNotifer.Invoke(result);
            }
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

        public override void HttpPostReq(string url, Object data, Dictionary<string, string> header)
        {
            url = this.BaseUrl + url;
            base.HttpPostReq(url, data, header); 
        }

        public override void HttpDownload(string url, Dictionary<string, string> header,string path)
        {
            url = this.BaseUrl + url;
            base.HttpDownload(url, header,path);
        }
    }
}
