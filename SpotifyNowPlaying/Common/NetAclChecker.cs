using System;
using System.Diagnostics;
using log4net;

namespace SpotifyNowPlaying.Common
{
    public static class NetAclChecker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NetAclChecker));

        public static void AddAddress(string address)
        {
            AddAddress(address, Environment.UserDomainName, Environment.UserName);
        }

        public static void AddAddress(string address, string domain, string user)
        {
            log.Debug($@"Adding http url '{address}' for user {domain}\{user}");

            var args = $@"http add urlacl url={address} user={domain}\{user}";
            var psi = new ProcessStartInfo("netsh", args)
            {
                Verb = @"runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            Process.Start(psi).WaitForExit();
        }
    }
}
