﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiLibs.General
{
    public class SubService
    {
        internal Service Service { get; }
        public SubService(Service service)
        {
            Service = service;
        }

        internal async Task<T> MakeRequest<T>(string url, Call m = Call.GET, List<Param> parameters = null, List<Param> header = null, object content = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return await Service.MakeRequest<T>(url, m, parameters, header, content, statusCode);
        }

        internal async Task<string> HandleRequest(string url, Call m = Call.GET, List<Param> parameters = null, List<Param> header = null, object content = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return await Service.HandleRequest(url, m, parameters, header, content, statusCode);
        }
    }
}