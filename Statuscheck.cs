using JellyfinMediaGrouper.Core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace cachet_monitor
{
    class Statuscheck
    {
        public bool VerifySSL { get; set; } = true;
        public bool CheckHTTP(string URL, int statuscodeMin = 200, int statuscodeMax = 399)
        {
            bool alive = true;
            HTTPBase httpBase = new HTTPBase();
            try
            {
                HttpWebResponse response = httpBase.getRequest(URL, verifySSL: VerifySSL);
                if ((int)response.StatusCode >= statuscodeMin && (int)response.StatusCode <= statuscodeMax)
                {
                    alive = true;
                }
            } catch (WebException ex)
            {
                Console.WriteLine($"Host {URL} failed with message {ex.Message}");
                alive = false;
                var response = ex.Response as HttpWebResponse;
                if (response != null && (int)response.StatusCode >= statuscodeMin && (int)response.StatusCode <= statuscodeMax)
                {
                    alive = true;
                }
            }
            previousRunResult = alive;
            return alive;
        }
        public bool previousRunResult { get; private set; }
    }
}
