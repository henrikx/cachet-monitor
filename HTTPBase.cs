using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using cachet_monitor;

namespace JellyfinMediaGrouper.Core
{
    class HTTPBase
    {
        public string getRequestString(string yourUrl, string ApiKey = "")
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(yourUrl);

            if (ApiKey != "")
            {
                httpWebRequest.Headers.Add("x-cachet-application", "monitor");
                httpWebRequest.Headers.Add("x-cachet-token", ApiKey);
            }

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();

        }
        public HttpWebResponse getRequest(string yourUrl, string ApiKey = "", bool verifySSL = true)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(yourUrl);
            if (!verifySSL)
            {
                httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            }

            if (ApiKey != "")
            {
                httpWebRequest.Headers.Add("x-cachet-application", "monitor");
                httpWebRequest.Headers.Add("x-cachet-token", ApiKey);
            }

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            return response;

        }

        public string postRequestString(string yourUrl, string ApiKey = "", string data = "")
        {

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(yourUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            if (ApiKey != "")
            {
                httpWebRequest.Headers.Add("x-cachet-application", "monitor");
                httpWebRequest.Headers.Add("x-cachet-token", ApiKey);
            }

            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            using (StreamReader streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
        public string putRequestString(string yourUrl, string ApiKey = "", string data = "")
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(yourUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";

            if (ApiKey != "")
            {
                httpWebRequest.Headers.Add("x-cachet-application", "monitor");
                httpWebRequest.Headers.Add("x-cachet-token", ApiKey);
            }


            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            using (StreamReader streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }

        }
        public string deleteRequestString(string yourUrl, string ApiKey = "", string data = "")
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(yourUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";

            if(ApiKey != "")
            {
                httpWebRequest.Headers.Add("x-cachet-application", "monitor");
                httpWebRequest.Headers.Add("x-cachet-token", ApiKey);
            }

            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            using (StreamReader streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
