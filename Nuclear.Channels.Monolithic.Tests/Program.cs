using System;
using System.Collections.Generic;
using System.Security.Claims;
using Nuclear.Channels.Authentication.Extensions;
using Nuclear.Channels.Heuristics.CacheCleaner;

namespace Nuclear.Channels.Monolithic.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthMethods authMethods = new AuthMethods();
            IChannelServer host = ChannelServerBuilder.CreateServer();
            //host.LoadAssemblies(AppDomain.CurrentDomain, null);
            List<string> asm = new List<string>
            {
                "Nuclear.Channels.Monolithic.Tests"
            };

            host.RegisterChannels(asm);
            host.AddTokenAuthentication(authMethods.AuthenticateToken);
            //host.ConfigureCacheCleaner(TimeSpan.FromSeconds(30));
            host.StartHosting(null);

            Console.ReadLine();
        }
    }

    public class AuthMethods
    {
        public bool AuthenticateBasic(string username, string password)
        {
            return true;
        }

        //public bool AuthenticateToken(string token)
        //{
        //    return true;
        //}

        public Claim[] AuthenticateToken(string token)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("Role","Admin")
            };

            return claims.ToArray();
        }
    }
}
