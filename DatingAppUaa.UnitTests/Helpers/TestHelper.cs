﻿using DatingApp.Api;
using System;
using System.Net.Http;

namespace DatingAppUaa.UnitTests.Helpers
{
    public sealed class TestHelper
    {
        private static readonly Lazy<TestHelper> _lazyInstance =
            new Lazy<TestHelper>(() => new TestHelper());

        public static TestHelper Instance
        {
            get
            {
                return _lazyInstance.Value;
            }
        }
        public HttpClient Client { get; set; }

        private TestHelper()
        {
            // place for instance initialization code
            Client = new APIWebApplicationFactory<Startup>().CreateDefaultClient();
        }
    }
}
