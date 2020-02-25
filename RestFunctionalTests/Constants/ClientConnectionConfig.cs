using Microsoft.Rest;
using RestSDKLibrary;
using System;

namespace RestFunctionalTests.Constants
{
    public static class ClientConnectionConfig
    {
        public static readonly string LocalEndpointUrl = "https://localhost:44309";
        public static ServiceClientCredentials serviceClientCredentials = new TokenCredentials("FakeTokenValue");
        public static readonly RestSDKLibraryClient client = new RestSDKLibraryClient(new Uri(LocalEndpointUrl), serviceClientCredentials);
    }
}
