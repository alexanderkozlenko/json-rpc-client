﻿using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Community.JsonRpc.ServiceClient.UnitTests.TestStubs
{
    internal sealed class TestJsonRpcClient : JsonRpcClient
    {
        public TestJsonRpcClient(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler = null)
            : base(new Uri("https://localhost", UriKind.Absolute), CreateHttpInvoker(new TestHttpHandler(handler)))
        {
        }

        private static HttpMessageInvoker CreateHttpInvoker(HttpMessageHandler httpHandler)
        {
            var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            return httpClient;
        }

        protected override void VisitHttpRequestHeaders(HttpRequestHeaders headers)
        {
            VisitHttpRequestHeadersAction?.Invoke(headers);
        }

        protected override void VisitHttpResponseHeaders(HttpResponseHeaders headers)
        {
            VisitHttpResponseHeadersAction?.Invoke(headers);
        }

        public Task<IReadOnlyList<JsonRpcResponse>> PublicSendJsonRpcRequestsAsync(IReadOnlyList<JsonRpcRequest> requests, CancellationToken cancellationToken)
        {
            return SendJsonRpcRequestsAsync(requests, cancellationToken);
        }

        protected override Version HttpProtocolVersion
        {
            get => PublicHttpProtocolVersion;
        }

        public Version PublicHttpProtocolVersion
        {
            get;
            set;
        }

        public JsonRpcContractResolver PublicContractResolver
        {
            get => ContractResolver;
        }

        public Uri PublicServiceUri
        {
            get => ServiceUri;
        }

        public Action<HttpRequestHeaders> VisitHttpRequestHeadersAction
        {
            get;
            set;
        }

        public Action<HttpResponseHeaders> VisitHttpResponseHeadersAction
        {
            get;
            set;
        }
    }
}