﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using ApiLibs.General;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace ApiLibs
{
    public abstract class Service
    {
        private readonly bool _debug;
        protected RestClient Client;
        private readonly List<Param> _standardParameter = new List<Param>();
        private readonly List<Param> _standardHeader = new List<Param>();

        public Service(string hostUrl, bool debug = false)
        {
            _debug = debug;
            Client = new RestClient { BaseUrl = new Uri(hostUrl) };
        }

        protected void AddStandardParameter(Param p)
        {
            _standardParameter.Add(p);
        }

        protected void AddStandardHeader(Param p)
        {
            _standardHeader.Add(p);
        }

        protected void AddStandardHeader(string name, string content)
        {
            AddStandardHeader(new Param(name, content));
        }

        protected void RemoveStandardHeader(string name)
        {
            _standardHeader.RemoveAll(p => p.Name == name);
        }

        protected void UpdateParameterIfExists(Param p)
        {
            foreach (Param para in _standardParameter)
            {
                if (para.Name == p.Name)
                {
                    Print(para.Name + " was: " + para.Value + " is: " + p.Value);
                    para.Value = p.Value;
                }
            }
        }

        protected void UpdateHeaderIfExists(string name, string value)
        {
            UpdateHeaderIfExists(new Param(name, value));
        }

        protected void UpdateHeaderIfExists(Param p)
        {
            foreach (Param para in _standardHeader)
            {
                if (para.Name == p.Name)
                {
                    Print(para.Name + " was: " + para.Value + " is: " + p.Value);
                    para.Value = p.Value;

                }
            }
        }

        protected void ConnectOAuth(string username, string secret)
        {
            Client.Authenticator = new HttpBasicAuthenticator(username, secret);
        }


        protected internal async Task<T> MakeRequest<T>(string url, Call m = Call.GET, List<Param> parameters = null, List<Param> header = null, object content = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return Convert<T>(await HandleRequest(url, m, parameters, header, content, statusCode));
        }

        protected internal virtual async Task<string> HandleRequest(string url, Call call = Call.GET, List<Param> parameters = null, List<Param> headers = null, object content = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            RestRequest request = new RestRequest(url, Convert(call));
            return await HandleRequest(request, parameters, headers, content, statusCode);
        }

        protected async Task<string> HandleRequest(IRestRequest request, List<Param> parameters = null, List<Param> headers = null, object content = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (headers != null)
            {
                foreach (Param p in headers)
                {
                    request.AddHeader(p.Name, p.Value);
                }
            }

            foreach (Param para in _standardHeader)
            {
                request.AddHeader(para.Name, para.Value);
            }

            parameters = parameters ?? new List<Param>();
            parameters.AddRange(_standardParameter);

            foreach (Param para in parameters)
            {
                if (para is OParam)
                {
                    if (para.Value == null)
                    {
                        continue;
                    }
                }

                if (request.Method == Method.GET || request.Method == Method.POST)
                {

                    request.AddParameter(para.Name, para.Value);
                }
                else
                {
                    request.AddParameter(para.Name, para.Value, ParameterType.QueryString);
                }
            }


            if (content != null)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                request.AddParameter("application/json", JsonConvert.SerializeObject(content, settings), ParameterType.RequestBody);
                request.AddHeader("Content-Type", "application/json");
            }

            return (await ExcecuteRequest(request, statusCode)).Content;
        }

        protected async Task<IRestResponse> ExcecuteRequest(IRestRequest request, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Debug.Assert(Client != null, "Client != null");
            IRestResponse resp = await Client.ExecuteTaskAsync(request);

            if (resp.StatusCode != statusCode && resp.StatusCode.ToString() != "Created" && resp.StatusCode.ToString() != "ResetContent")
            {
                if (resp.ErrorException != null)
                {
                    if (resp.ErrorException is System.Net.WebException)
                    {

                        throw new NoInternetException(resp.ErrorException);
                    }

                    throw resp.ErrorException;
                }

                RequestException<IRestResponse>.ConvertToException((int)resp.StatusCode, resp.StatusDescription, resp.ResponseUri.ToString(), resp.ErrorMessage, resp.Content, resp);
            }
            return resp;
        }

        protected T Convert<T>(string text)
        {
            if (typeof(T) == typeof(string))
            {
                return (T) (object) text;
            }

            T returnObj = JsonConvert.DeserializeObject<T>(text);
            if (returnObj is ObjectSearcher)
            {
                //Enable better OOP
                (returnObj as ObjectSearcher).Search(this);
            }
            return returnObj;
        }

        private Method Convert(Call m)
        {
            switch (m)
            {
                case Call.POST:
                    return Method.POST;
                case Call.PATCH:
                    return Method.PATCH;
                case Call.DELETE:
                    return Method.DELETE;
                case Call.PUT:
                    return Method.PUT;
                default:
                    return Method.GET;
            }
        }

        protected void SetBaseUrl(string baseurl)
        {
            Client.BaseUrl = new Uri(baseurl);
        }

        protected void Print(string s)
        {
            if (_debug)
            {
                Console.WriteLine(s);
            }
        }
    }
}

public enum Call
{
    POST,
    GET,
    PATCH,
    DELETE,
    PUT
}
