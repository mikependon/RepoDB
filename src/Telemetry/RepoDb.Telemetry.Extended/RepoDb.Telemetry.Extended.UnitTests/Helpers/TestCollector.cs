#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Receives real HTTP publishing requests on an ephemeral loopback port.
    /// </summary>
    internal sealed class TestCollector : IDisposable
    {
        #region Private Variables

        private readonly HttpListener _listener = new HttpListener();

        #endregion

        #region Constructors

        /// <summary>
        /// Starts a local collector without requiring an external service.
        /// </summary>
        public TestCollector()
        {
            var socket = new TcpListener(IPAddress.Loopback,
                0);
            socket.Start();
            var port = ((IPEndPoint)socket.LocalEndpoint).Port;
            socket.Stop();
            Host = $"http://127.0.0.1:{port}";
            _listener.Prefixes.Add(Host + "/");
            _listener.Start();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the authentication header from the received request.
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Gets the local collector host.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets the path from the received request.
        /// </summary>
        public string Path { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Receives and decodes one Base64 and gzip encoded event array.
        /// </summary>
        /// <param name="statusCode">The HTTP response status to return.</param>
        /// <returns>The decoded JSON payload.</returns>
        public async Task<JsonDocument> ReceiveAsync(int statusCode = 200)
        {
            var context = await _listener.GetContextAsync().WaitAsync(TimeSpan.FromSeconds(15));
            ApiKey = context.Request.Headers["X-API-Key"];
            Path = context.Request.Url.AbsolutePath;
            using var reader = new StreamReader(context.Request.InputStream);
            var encoded = await reader.ReadToEndAsync();
            context.Response.StatusCode = statusCode;
            context.Response.Close();
            using var stream = new MemoryStream(Convert.FromBase64String(encoded));
            using var gzip = new GZipStream(stream,
                CompressionMode.Decompress);
            return await JsonDocument.ParseAsync(gzip);
        }

        /// <summary>
        /// Stops the listener and releases the local endpoint.
        /// </summary>
        public void Dispose()
        {
            _listener.Close();
        }

        #endregion
    }
}

