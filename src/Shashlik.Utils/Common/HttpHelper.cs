using Guc.Utils.Extensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Guc.Utils.Common
{
    public static class HttpHelper
    {
        private static IRestClient GetClient(Uri uri, IWebProxy proxy = null, Encoding encoding = null)
        {
            RestClient client = new RestClient(uri);
            if (encoding != null)
                client.Encoding = encoding;
            client.Proxy = proxy;
            // 撒鬼玩意证实都通过,linux环境下可能出现 https验证不通过的莫名其妙的错误
            client.RemoteCertificateValidationCallback = (a, b, c, d) => true;
            return client;
        }

        private static IRestRequest GetRequest(Uri uri, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30)
        {
            var request = new RestRequest { Timeout = timeout * 1000 };

            if (!headers.IsNullOrEmpty())
                foreach (var item in headers)
                {
                    request.AddHeader(item.Key, item.Value);
                }
            if (!cookies.IsNullOrEmpty())
                foreach (var item in headers)
                    request.AddCookie(item.Key, item.Value);

            return request;
        }

        /// <summary>
        /// post json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="jsonData">json对象</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<string> PostJson(string url, object jsonData, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (jsonData != null)
                request.AddJsonBody(jsonData);

            var response = await client.ExecutePostAsync(request);
            if (response.IsSuccessful)
                return response.Content;
            else
                throw new Exception($"请求发生错误,url:{url},method:post,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// post json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="jsonData">json对象</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<TResult> PostJson<TResult>(string url, object jsonData, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
            where TResult : class
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (jsonData != null)
                request.AddJsonBody(jsonData);

            var response = await client.ExecutePostAsync<TResult>(request);
            if (response.IsSuccessful)
                return response.Data;
            else
                throw new Exception($"请求发生错误,url:{url},method:post,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// post json对象,返回原始的http响应对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="jsonData">json对象</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<IRestResponse> PostJsonForOriginResponse<TResult>(string url, object jsonData, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
            where TResult : class
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (jsonData != null)
                request.AddJsonBody(jsonData);

            return await client.ExecutePostAsync<TResult>(request);
        }

        /// <summary>
        /// post form表单
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单键值对</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<string> PostForm(string url, IEnumerable<KeyValuePair<string, string>> formData, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (!formData.IsNullOrEmpty())
                foreach (var item in formData)
                    request.AddParameter(item.Key, item.Value, ParameterType.GetOrPost);

            var response = await client.ExecutePostAsync(request);
            if (response.IsSuccessful)
                return response.Content;
            else
                throw new Exception($"请求发生错误,url:{url},method:post,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// post form表单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单对象,读取public 可读的属性</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<string> PostForm(string url, object formData, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (formData != null)
                request.AddObject(formData);
            var response = await client.ExecutePostAsync(request);
            if (response.IsSuccessful)
                return response.Content;
            else
                throw new Exception($"请求发生错误,url:{url},method:post,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);

        }

        /// <summary>
        /// post form表单
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单键值对</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<TResult> PostForm<TResult>(string url, IEnumerable<KeyValuePair<string, string>> formData, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
            where TResult : class
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (!formData.IsNullOrEmpty())
                foreach (var item in formData)
                    request.AddParameter(item.Key, item.Value, ParameterType.GetOrPost);

            var response = await client.ExecutePostAsync<TResult>(request);
            if (response.IsSuccessful)
                return response.Data;
            else
                throw new Exception($"请求发生错误,url:{url},method:post,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// post form表单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单对象,读取public 可读的属性</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<TResult> PostForm<TResult>(string url, object formData, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
            where TResult : class
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (formData != null)
                request.AddObject(formData);
            var response = await client.ExecutePostAsync<TResult>(request);
            if (response.IsSuccessful)
                return response.Data;
            else
                throw new Exception($"请求发生错误,url:{url},method:post,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);

        }

        /// <summary>
        /// post form表单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单对象,读取public 可读的属性</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<IRestResponse> PostFormForOriginResponse(string url, object formData, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (formData != null)
                request.AddObject(formData);
            return await client.ExecutePostAsync(request);
        }

        /// <summary>
        /// post form表单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单对象,读取public 可读的属性</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<IRestResponse> PostFormForOriginResponse(string url, IEnumerable<KeyValuePair<string, string>> formData, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (!formData.IsNullOrEmpty())
                foreach (var item in formData)
                    request.AddParameter(item.Key, item.Value, ParameterType.GetOrPost);
            return await client.ExecutePostAsync(request);
        }

        /// <summary>
        /// post文件上传
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单键值对</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<string> PostFiles(string url, IEnumerable<KeyValuePair<string, string>> formDatas, IEnumerable<UploadFileModel> files, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (!formDatas.IsNullOrEmpty())
                foreach (var item in formDatas)
                    request.AddParameter(item.Key, item.Value, ParameterType.GetOrPost);

            if (!files.IsNullOrEmpty())
                foreach (var item in files)
                    request.AddFileBytes(item.Name, item.FileBytes, item.FileName);

            var response = await client.ExecutePostAsync(request);
            if (response.IsSuccessful)
                return response.Content;
            else
                throw new Exception($"请求发生错误,url:{url},method:post,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// post文件上传
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单对象,读取public 可读的属性</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<string> PostFiles(string url, object formData, IEnumerable<UploadFileModel> files, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (formData != null)
                request.AddObject(formData);
            if (!files.IsNullOrEmpty())
                foreach (var item in files)
                    request.AddFileBytes(item.Name, item.FileBytes, item.FileName);

            var response = await client.ExecutePostAsync(request);
            if (response.IsSuccessful)
                return response.Content;
            else
                throw new Exception($"请求发生错误,url:{url},method:post,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// post文件上传
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单键值对</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<TResult> PostFiles<TResult>(string url, IEnumerable<KeyValuePair<string, string>> formDatas, IEnumerable<UploadFileModel> files, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
            where TResult : class
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (!formDatas.IsNullOrEmpty())
                foreach (var item in formDatas)
                    request.AddParameter(item.Key, item.Value, ParameterType.GetOrPost);

            if (!files.IsNullOrEmpty())
                foreach (var item in files)
                    request.AddFileBytes(item.Name, item.FileBytes, item.FileName);

            var response = await client.ExecutePostAsync<TResult>(request);
            if (response.IsSuccessful)
                return response.Data;
            else
                throw new Exception($"请求发生错误,url:{url},method:post,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// post文件上传
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单对象,读取public 可读的属性</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<TResult> PostFiles<TResult>(string url, object formData, IEnumerable<UploadFileModel> files, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
            where TResult : class
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (formData != null)
                request.AddObject(formData);
            if (!files.IsNullOrEmpty())
                foreach (var item in files)
                    request.AddFileBytes(item.Name, item.FileBytes, item.FileName);

            var response = await client.ExecutePostAsync<TResult>(request);
            if (response.IsSuccessful)
                return response.Data;
            else
                throw new Exception($"请求发生错误,url:{url},method:post,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// post文件上传
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单键值对</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<IRestResponse> PostFilesForOriginResponse(string url, IEnumerable<KeyValuePair<string, string>> formDatas, IEnumerable<UploadFileModel> files, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (!formDatas.IsNullOrEmpty())
                foreach (var item in formDatas)
                    request.AddParameter(item.Key, item.Value, ParameterType.GetOrPost);

            if (!files.IsNullOrEmpty())
                foreach (var item in files)
                    request.AddFileBytes(item.Name, item.FileBytes, item.FileName);

            return await client.ExecutePostAsync(request);
        }

        /// <summary>
        /// post文件上传
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formData">form表单对象,读取public 可读的属性</param>
        /// <param name="headers">请求头部</param>
        /// <param name="cookies">请求cookie</param>
        /// <param name="timeout">请求超时事件 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<IRestResponse> PostFilesForOriginResponse(string url, object formData, IEnumerable<UploadFileModel> files, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (formData != null)
                request.AddObject(formData);
            if (!files.IsNullOrEmpty())
                foreach (var item in files)
                    request.AddFileBytes(item.Name, item.FileBytes, item.FileName);

            return await client.ExecutePostAsync(request);
        }

        public static async Task<IRestResponse> Post(string url, string body, string contentType, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);
            request.AddParameter(contentType, body, ParameterType.RequestBody);

            return await client.ExecutePostAsync(request);
        }

        /// <summary>
        /// get 请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="queryStringData">url参数,读取pubic 可取的属性,转换为url参数</param>
        /// <param name="headers">请求头</param>
        /// <param name="cookies">请求时发送的cookie</param>
        /// <param name="timeout">请求超时 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<string> GetString(string url, object queryStringData = null, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (queryStringData != null)
                request.AddObject(queryStringData);
            var response = await client.ExecuteGetAsync(request);
            if (response.IsSuccessful)
                return response.Content;
            else
                throw new Exception($"请求发生错误,url:{url},method:get,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }
        /// <summary>
        /// get 请求
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="queryStringData">url参数,读取pubic 可取的属性,转换为url参数</param>
        /// <param name="headers">请求头</param>
        /// <param name="cookies">请求时发送的cookie</param>
        /// <param name="timeout">请求超时 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<TResult> Get<TResult>(string url, object queryStringData = null, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (queryStringData != null)
                request.AddObject(queryStringData);
            var response = await client.ExecuteGetAsync<TResult>(request);
            if (response.IsSuccessful)
                return response.Data;
            else
                throw new Exception($"请求发生错误,url:{url},method:get,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// get获取数据流
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="queryStringData">url参数,读取pubic 可取的属性,转换为url参数</param>
        /// <param name="headers">请求头</param>
        /// <param name="cookies">请求时发送的cookie</param>
        /// <param name="timeout">请求超时 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<Stream> GetStream(string url, object queryStringData = null, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (queryStringData != null)
                request.AddObject(queryStringData);
            var response = await client.ExecuteGetAsync(request);
            if (response.IsSuccessful)
                return new MemoryStream(response.RawBytes);
            else
                throw new Exception($"请求发生错误,url:{url},method:get,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// get 获取数据字节数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="queryStringData">url参数,读取pubic 可取的属性,转换为url参数</param>
        /// <param name="headers">请求头</param>
        /// <param name="cookies">请求时发送的cookie</param>
        /// <param name="timeout">请求超时 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<byte[]> GetBytes(string url, object queryStringData = null, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (queryStringData != null)
                request.AddObject(queryStringData);

            var response = await client.ExecuteGetAsync(request);
            if (response.IsSuccessful)
                return response.RawBytes;
            else
                throw new Exception($"请求发生错误,url:{url},method:get,httpcode:{response.StatusCode},result:{response.Content}", response.ErrorException);
        }

        /// <summary>
        /// get 返回响应结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="queryStringData">url参数,读取pubic 可取的属性,转换为url参数</param>
        /// <param name="headers">请求头</param>
        /// <param name="cookies">请求时发送的cookie</param>
        /// <param name="timeout">请求超时 秒</param>
        /// <param name="proxy">代理设置</param>
        /// <returns></returns>
        public static async Task<IRestResponse> GetForOriginResponse(string url, object queryStringData = null, IDictionary<string, string> headers = null, IDictionary<string, string> cookies = null, int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);

            if (queryStringData != null)
                request.AddObject(queryStringData);
            return await client.ExecuteGetAsync(request);
        }

        /// <summary>
        /// 通用http调方法,body不为空时,contentType也不能为空
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="contentType"></param>
        /// <param name="queryStringData"></param>
        /// <param name="headers"></param>
        /// <param name="cookies"></param>
        /// <param name="timeout"></param>
        /// <param name="proxy"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<IRestResponse> Invoke(
            Method method,
            string url,
            string body,
            string contentType,
            IEnumerable<KeyValuePair<string, string>> queryStringData = null,
            IDictionary<string, string> headers = null,
            IDictionary<string, string> cookies = null,
            int timeout = 30, IWebProxy proxy = null, Encoding encoding = null)
        {
            var uri = new Uri(url);
            var client = GetClient(uri, proxy, encoding);
            var request = GetRequest(uri, headers, cookies, timeout);
            if (!queryStringData.IsNullOrEmpty())
                foreach (var item in queryStringData)
                    request.AddParameter(item.Key, item.Value, ParameterType.QueryString);
            if (!body.IsNullOrWhiteSpace() && !contentType.IsNullOrWhiteSpace())
                request.AddParameter(contentType, body, ParameterType.RequestBody);

            return await client.ExecuteAsync(request, method);
        }
    }

    public class UploadFileModel
    {
        /// <summary>
        /// 上传的文件参数名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件字节
        /// </summary>
        public byte[] FileBytes { get; set; }

        /// <summary>
        /// 上传的文件名
        /// </summary>
        public string FileName { get; set; }
    }
}
