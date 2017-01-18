using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using System.ServiceModel.Web;
using System.Net;
using ScmWcfService.Config;

namespace ScmWcfService.Provider
{
    public class ApiClient
    {
        public string token { get; set; }
        public ApiClient() { }
        public ApiClient(string token) { this.token = token; }

        public IRestRequest GenRequest(string url, Method method = Method.GET, DataFormat format = DataFormat.Json)
        {
            var req = new RestRequest(url, method);
            req.RequestFormat = format;
            return req;
        }

        public T Execute<T>(IRestRequest request) where T : new()
        {
            var response = genClient().Execute<T>(request);
            return response.Data;
        }


        public IRestResponse Execute(IRestRequest request)
        {
            var response = genClient().Execute(request);
            return responseHandler(response);
        }

        private RestClient genClient()
        {
            var client = new RestClient();
            client.Timeout = 10000;
            client.BaseUrl = ApiConfig.BaseUri;
            if (this.token != null)
            {
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(this.token, "Bearer");
            }

            if (ApiConfig.Token != null && !string.IsNullOrEmpty(ApiConfig.Token))
            {
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(ApiConfig.Token, "Bearer");
            }
            return client;
        }

        private IRestResponse responseHandler(IRestResponse res)
        {
            try
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new WebFaultException<string>("无权限访问系统，请重新登陆", HttpStatusCode.Unauthorized);
                }
                else if (res.StatusCode != HttpStatusCode.OK && res.StatusCode != HttpStatusCode.Created)
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = res.StatusCode;
                    WebOperationContext.Current.OutgoingResponse.StatusDescription = res.StatusDescription;
                    throw new WebFaultException<string>(res.StatusDescription, res.StatusCode);
                }
                return res;
            }
            catch (WebFaultException<string> we)
            {
                throw new WebFaultException<string>(we.Detail, we.StatusCode);
            }
            catch (Exception ex)
            {
                throw new WebFaultException<string>("网络错误，请检查网络连接", HttpStatusCode.GatewayTimeout);
            }
        }
    }

    public class Parameter { 
      
    }
}