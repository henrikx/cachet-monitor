﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace cachet_monitor
{
    partial class API
    {
        public string LastActionRequest { get; private set; }


        public Dictionary<string, dynamic> CreateIncident(string title, string message, int status, int visible, int? componentid = null, int? componentstatus = null, bool? stickied = null)
        {
            string URL = Configuration.GetConfiguration().BaseURL + "incidents";
            string API = Configuration.GetConfiguration().APIKey;
            string JSON = JsonSerializer.Serialize<Dictionary<string,dynamic>>(new Dictionary<string, dynamic> { { "name", title }, { "message", message }, { "status", status }, { "visible", visible }, { "component_id", componentid }, { "component_status", componentstatus }, { "stickied", stickied } }, new JsonSerializerOptions() { IgnoreNullValues = true });
            LastActionRequest = JSON;
            HTTPBase apiBase = new HTTPBase();
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectToInferredTypesConverter());
            Dictionary<string, dynamic> response = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(apiBase.postRequestString(URL, API, JSON), options);

            return response;
        }
        public Dictionary<string, dynamic> UpdateIncident(int id, string title = null, string message = null, int? status = null, int? visible = null, int? componentid = null, int? componentstatus = null, bool? stickied = null)
        {
            string URL = Configuration.GetConfiguration().BaseURL + "incidents/" + id.ToString();
            string API = Configuration.GetConfiguration().APIKey;
            string JSON = JsonSerializer.Serialize<Dictionary<string, dynamic>>(new Dictionary<string, dynamic> { { "name", title }, { "message", message }, { "status", status }, { "visible", visible }, { "component_id", componentid }, { "component_status", componentstatus }, { "stickied", stickied } }, new JsonSerializerOptions() { IgnoreNullValues = true });

            LastActionRequest = JSON;
            HTTPBase apiBase = new HTTPBase();
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectToInferredTypesConverter());
            Dictionary<string, dynamic> response = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(apiBase.putRequestString(URL, API, JSON), options);
            return response;
        }
        public Dictionary<string, dynamic> CreateIncidentUpdate(int id, string message, int status)
        {
            string URL = Configuration.GetConfiguration().BaseURL + "incidents/" + id.ToString() + "/updates";
            string API = Configuration.GetConfiguration().APIKey;
            string JSON = JsonSerializer.Serialize<Dictionary<string, dynamic>>(new Dictionary<string, dynamic> { { "status", status }, { "message", message } }, new JsonSerializerOptions() { IgnoreNullValues = true });

            LastActionRequest = JSON;

            HTTPBase apiBase = new HTTPBase();
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectToInferredTypesConverter());
            Dictionary<string, dynamic> response = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(apiBase.postRequestString(URL, API, JSON), options);
            return response;
        }
        public Dictionary<string, dynamic> UpdateComponentStatus(int id, int status)
        {
            string URL = Configuration.GetConfiguration().BaseURL + "components/" + id.ToString();
            string API = Configuration.GetConfiguration().APIKey;
            string JSON = JsonSerializer.Serialize<Dictionary<string, dynamic>>(new Dictionary<string, dynamic> { { "status", status } }, new JsonSerializerOptions() { IgnoreNullValues = true });
            LastActionRequest = JSON;
            HTTPBase apiBase = new HTTPBase();
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectToInferredTypesConverter());
            Dictionary<string, dynamic> response = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(apiBase.putRequestString(URL, API, JSON), options);
            return response;
        }
        public Dictionary<string, dynamic> GetComponent(int id)
        {
            string URL = Configuration.GetConfiguration().BaseURL + "components/" + id.ToString();
            string API = Configuration.GetConfiguration().APIKey;
            HTTPBase apiBase = new HTTPBase();
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectToInferredTypesConverter());
            Dictionary<string, dynamic> response = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(apiBase.getRequestString(URL, API), options);
            return response;
        }
        public Dictionary<string, dynamic> GetComponents()
        {
            string URL = Configuration.GetConfiguration().BaseURL + "components/";
            string API = Configuration.GetConfiguration().APIKey;
            HTTPBase apiBase = new HTTPBase();
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectToInferredTypesConverter());
            Dictionary<string, dynamic> response = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(apiBase.getRequestString(URL, API), options);
            return response;
        }


        public class Incident
        {
            public class data
            {
                public string name { get; set; }
                public string message { get; set; }
                public int status { get; set; }
                public int visible { get; set; }
                public int component_id { get; set; }
                public int component_status { get; set; }
                public bool notify { get; set; }
                public DateTime created_at { get; set; }
                public string template { get; set; }
                public string[] vars { get; set; }
                public DateTime scheduled_at { get; set; }
                public DateTime updated_at { get; set; }
                public DateTime deleted_at { get; set; }
                public string human_status { get; set; }

            }
        }

        public class Status
        {
            public static int Fixed = 4;
            public static int Investigating = 1;
            public static int Watching = 3;
            public static int Identified = 2;
            public static int Scheduled = 0;

        }
        public class ComponentStatus
        {
            public static int Operational = 1;
            public static int Watching = 3;
            public static int PartialOutage = 2;
            public static int MajorOutage = 4;

        }
    }
}
