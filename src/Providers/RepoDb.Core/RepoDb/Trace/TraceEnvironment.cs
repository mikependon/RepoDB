#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace RepoDb
{
    /// <summary>
    /// Holds the information of the machine that runs the tracing operations. The values do not change while the process is running,
    /// so they are resolved once. The IP address is resolved at the first use, as it asks the network interfaces of the machine.
    /// </summary>
    internal static class TraceEnvironment
    {
        #region Private Variables

        /// <summary>
        /// The IP address of the machine, resolved when it is first needed.
        /// </summary>
        private static readonly Lazy<string> _ipAddress = new Lazy<string>(ResolveIpAddress);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the IPv4 address of the machine, or <c>null</c> if it cannot be resolved. A network interface that has a gateway is preferred,
        /// so a virtual adapter (i.e.: of a container or a virtual machine) is not picked over the one that reaches the network.
        /// </summary>
        public static string IpAddress => _ipAddress.Value;

        /// <summary>
        /// Gets the name of the machine, or <c>null</c> if it cannot be resolved.
        /// </summary>
        public static string MachineName { get; } = ResolveMachineName();

        #endregion

        #region Private Methods

        /// <summary>
        /// Resolves the IPv4 address of the machine from its network interfaces, without asking a DNS server.
        /// </summary>
        /// <returns>The address, or <c>null</c> if the machine has none or the network interfaces cannot be read.</returns>
        private static string ResolveIpAddress()
        {
            try
            {
                string fallback = null;
                foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    var address = GetUsableAddress(networkInterface);
                    if (address == null)
                    {
                        continue;
                    }
                    if (HasGateway(networkInterface))
                    {
                        return address;
                    }
                    fallback = fallback ?? address;
                }
                return fallback;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the first IPv4 address of a network interface that is up, if the interface is not a loopback or a tunnel.
        /// </summary>
        /// <param name="networkInterface">The network interface.</param>
        /// <returns>The address, or <c>null</c> if the interface cannot be used.</returns>
        private static string GetUsableAddress(NetworkInterface networkInterface)
        {
            if (networkInterface.OperationalStatus != OperationalStatus.Up ||
                networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                networkInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
            {
                return null;
            }
            foreach (var unicast in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (unicast.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(unicast.Address))
                {
                    return unicast.Address.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// Checks whether a network interface has a default gateway.
        /// </summary>
        /// <param name="networkInterface">The network interface.</param>
        /// <returns><c>true</c> if the interface has a gateway that is not the empty address; otherwise, <c>false</c>.</returns>
        private static bool HasGateway(NetworkInterface networkInterface)
        {
            foreach (var gateway in networkInterface.GetIPProperties().GatewayAddresses)
            {
                if (gateway.Address != null && !IPAddress.Any.Equals(gateway.Address) && !IPAddress.IPv6Any.Equals(gateway.Address))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Resolves the name of the machine, falling back to the host name and then to the environment variables that the operating systems set.
        /// </summary>
        /// <returns>The name of the machine, or <c>null</c> if it cannot be resolved.</returns>
        private static string ResolveMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception)
            {
                try
                {
                    return Dns.GetHostName();
                }
                catch (Exception)
                {
                    return Environment.GetEnvironmentVariable("HOSTNAME") ?? Environment.GetEnvironmentVariable("COMPUTERNAME");
                }
            }
        }

        #endregion
    }
}
