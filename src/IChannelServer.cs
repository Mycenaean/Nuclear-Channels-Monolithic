﻿// Copyright © Nikola Milinkovic 
// Licensed under the MIT License (MIT).
// See License.md in the repository root for more information.

using Nuclear.Channels.Authentication;
using Nuclear.Channels.Base.Exceptions;
using Nuclear.Channels.Heuristics.CacheCleaner;
using System;
using System.Collections.Generic;

namespace Nuclear.Channels
{
    /// <summary>
    /// Service that will host the Channels
    /// </summary>
    public interface IChannelServer : IChannelAuthenticationEnabled, IChannelCacheCleanable , IServerManaged
    {
        /// <summary>
        /// Method to load all assemblies containing channels
        /// </summary>
        /// <remarks>
        /// Use this method if your AppDomain does not contain referencing Channel Assemblies
        /// </remarks>
        /// <param name="domain">Current AppDomain</param>
        /// <param name="assemblies">Assembly names of your projects</param>        
        void LoadAssemblies(AppDomain domain, string[] assemblies);

        /// <summary>
        /// Load Application Domain
        /// </summary>
        /// <remarks>
        /// Use this method if your AppDomain contains referencing Channel Assemblies
        /// </remarks>
        /// <param name="domain">Current AppDomain</param>
        public void LoadAssemblies(AppDomain domain);

        /// <summary>
        /// AuthenticationOptions for Token Authentication
        /// </summary>
        /// <param name="tokenAuthenticationMethod">Delegate for token authentication</param>
        [Obsolete("Please use one of the overloads of AddTokenAuthentication extension method")]
        void AuthenticationOptions(Func<string, bool> tokenAuthenticationMethod);

        /// <summary>
        /// AuthenticationOptions for Basic Authentication
        /// </summary>
        /// <param name="basicAuthenticationMethod">Delegate for basic authentication</param>
        [Obsolete("Please use one of the overloads of AddBasicAuthentication extension method")]
        void AuthenticationOptions(Func<string, string, bool> basicAuthenticationMethod);

        /// <summary>
        /// Starts hosting
        /// </summary>
        /// <param name="baseURL">Base url to be provided , if not base url will be http://localhost:4200 </param>
        /// <exception cref="HttpListenerNotSupportedException"></exception>
        void StartHosting(string baseURL);

        /// <summary>
        /// Registers assemblies used for Channel lookups
        /// </summary>
        /// <remarks>
        /// Use this method if your calling application is referencing other Channel Assemblies
        /// </remarks>
        /// <param name="assemblieContainingChannels">Assemblies containing Channels</param>
        void RegisterChannels(List<string> assemblieContainingChannels);
    }
}
