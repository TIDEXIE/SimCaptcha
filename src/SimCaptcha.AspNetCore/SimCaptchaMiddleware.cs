﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using SimCaptcha.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace SimCaptcha.AspNetCore
{
    public abstract class SimCaptchaMiddleware
    {
        protected readonly RequestDelegate _next;

        protected readonly SimCaptchaOptions _options;

        protected readonly SimCaptchaService _service;

        protected readonly AspNetCoreJsonHelper _jsonHelper;

        protected readonly IHttpContextAccessor _accessor;

        public SimCaptchaMiddleware(RequestDelegate next, IOptions<SimCaptchaOptions> optionsAccessor, IMemoryCache memoryCache, IHttpContextAccessor accessor)
        {
            _next = next;
            _options = optionsAccessor.Value;

            _service = new SimCaptchaService(
                optionsAccessor.Value,
                new LocalCache(memoryCache) { TimeOut = optionsAccessor.Value.ExpiredSec },
                new AspNetCoreVCodeImage(),
                new AspNetCoreJsonHelper()
                );
            _accessor = accessor;
            _jsonHelper = new AspNetCoreJsonHelper();
        }

        public abstract Task InvokeAsync(HttpContext context);
    }
}
