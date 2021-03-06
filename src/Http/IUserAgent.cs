﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Http
{
    public interface IUserAgent : IDisposable
    {
        Task ReceiveAsync(CancellationToken cancellationToken, OnResponseHeadersComplete callback = null);

        Task SendAsync(IRequestMessage message, CancellationToken cancellationToken, 
            OnRequestHeadersComplete callback = null);
    }
}