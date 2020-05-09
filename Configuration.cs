using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace cachet_monitor
{
    class Configuration
    {
        public static void ConfigurationDialog()
        {
            string userInputLevel1;
            bool exit = false;
            while (!exit)
            {
                Dictionary<string, string> optionslevel1 = new Dictionary<string, string>()
                {
                    { "API Key", currentConfiguration.APIKey },
                    { "Base URL", currentConfiguration.BaseURL },
                    { "Interval", currentConfiguration.Interval.ToString() },
                    { "Hosts", "Host List"},
                    { "Save and Exit", "" }
                };

                KeyValuePair<string, string> selectedOptionLevel1 = RenderConfigurationDialogMenu(optionslevel1);
                switch (selectedOptionLevel1.Key)
                {
                    case "Save and Exit":
                        SaveConfiguration(currentConfiguration);
                        exit = true;
                        break;
                    case "Base URL":
                        Console.Write("Base URL(the URL to Cachet): ");
                        userInputLevel1 = Console.ReadLine();
                        currentConfiguration.BaseURL = userInputLevel1;
                        break;
                    case "Interval":
                        Console.Write("Interval (in seconds): ");
                        userInputLevel1 = Console.ReadLine();
                        currentConfiguration.Interval = Convert.ToInt32(userInputLevel1);
                        break;
                    case "API Key":
                        Console.Write("API Key: ");
                        userInputLevel1 = Console.ReadLine();
                        currentConfiguration.APIKey = userInputLevel1;
                        break;
                    case "Hosts": //LEVEL2STARTER Hosts
                        bool backlevel2 = false;
                        string userInputLevel2;
                        while (!backlevel2)
                        {
                            Dictionary<string, string> optionslevel2 = new Dictionary<string, string>();

                            optionslevel2.Add("Add", "");
                            if (currentConfiguration.Expectations == null)
                            {
                                currentConfiguration.Expectations = new List<Expectation>();
                            }
                            List<List<Expectation>> groupedhostlist = new List<List<Expectation>>();
                            foreach (Expectation host in currentConfiguration.Expectations)
                            {
                                List<Expectation> samehosts = new List<Expectation>();
                                if (groupedhostlist.Where(h => h.Contains(host)).Count() < 1)
                                {
                                    foreach (Expectation host_ in currentConfiguration.Expectations)
                                    {
                                        if (host_.path == host.path)
                                        {
                                            samehosts.Add(host_);
                                        }
                                    }
                                    groupedhostlist.Add(samehosts);

                                }
                            }
                            foreach (List<Expectation> groupedhost in groupedhostlist)
                            {
                                int hostindex = 0;
                                foreach (Expectation host in groupedhost)
                                {
                                    hostindex++;
                                    optionslevel2.Add(host.path + "|" + hostindex, host.type);
                                }
                            }
                            KeyValuePair<string, string> selectedOptionLevel2 = RenderConfigurationDialogMenu(optionslevel2);
                            switch (selectedOptionLevel2.Key)
                            {
                                case "Add":
                                    Console.Write("Path (URL for HTTP and IP for ping): ");
                                    userInputLevel2 = Console.ReadLine();
                                    if (currentConfiguration.Expectations == null)
                                    {
                                        currentConfiguration.Expectations = new List<Expectation>();
                                    }
                                    currentConfiguration.Expectations.Add(new Expectation { path = userInputLevel2 });
                                    break;
                                case "Exit":
                                    backlevel2 = true;
                                    break;
                                default: //LEVEL3STARTER Host Details
                                    bool backlevel3 = false;
                                    string userInputLevel3;
                                    int SelectedHost = currentConfiguration.Expectations.FindIndex(host => host == currentConfiguration.Expectations.FindAll(host => host.path == selectedOptionLevel2.Key.Split("|")[0])[Convert.ToInt32(selectedOptionLevel2.Key.Split("|")[1])-1]); //Find index of userselected host
                                    while (!backlevel3) 
                                    {
                                        Dictionary<string, string> optionslevel3 = new Dictionary<string, string>()
                                        {
                                            { "Delete", "" },
                                            { "Path", currentConfiguration.Expectations[SelectedHost].path },
                                            { "Type", currentConfiguration.Expectations[SelectedHost].type },
                                            { "Expected Status Code Range", currentConfiguration.Expectations[SelectedHost].ExpectedStatusCodeRange },
                                            { "Verify SSL", currentConfiguration.Expectations[SelectedHost].verifySSL.ToString() },
                                            { "Actions", "Actions List" }
                                        };
                                        KeyValuePair<string, string> selectedOptionLevel3 = RenderConfigurationDialogMenu(optionslevel3);
                                        switch (selectedOptionLevel3.Key)
                                        {
                                            case "Delete":
                                                currentConfiguration.Expectations.Remove(currentConfiguration.Expectations[SelectedHost]);
                                                backlevel3 = true;
                                                break;
                                            case "Path":
                                                Console.Write("Path (URL for HTTP and IP for ping): ");
                                                userInputLevel3 = Console.ReadLine();
                                                currentConfiguration.Expectations[SelectedHost].path = userInputLevel3;
                                                backlevel3 = true;
                                                break;
                                            case "Type":
                                                Console.Write("Type (http/ping): ");
                                                userInputLevel3 = Console.ReadLine();
                                                currentConfiguration.Expectations[SelectedHost].type = userInputLevel3;
                                                break;
                                            case "Expected Status Code Range":
                                                Console.Write("Expected Status Code Range (from-to): ");
                                                userInputLevel3 = Console.ReadLine();
                                                currentConfiguration.Expectations[SelectedHost].ExpectedStatusCodeRange = userInputLevel3;
                                                break;
                                            case "Verify SSL":
                                                Console.Write("Verify SSL (true/false): ");
                                                userInputLevel3 = Console.ReadLine();
                                                try
                                                {
                                                    currentConfiguration.Expectations[SelectedHost].verifySSL = Convert.ToBoolean(userInputLevel3);
                                                } catch (FormatException)
                                                {
                                                    Console.WriteLine($"{selectedOptionLevel3.Key} is not a valid option.");
                                                    break;
                                                }
                                                break;
                                            case "Actions": //LEVEL4STARTER Host Actions
                                                bool backlevel4 = false;
                                                string userInputLevel4;
                                                while (!backlevel4)
                                                {
                                                    Dictionary<string, string> optionslevel4 = new Dictionary<string, string>();
                                                    optionslevel4.Add("Add", "");
                                                    int actionindex = 0;
                                                    if (currentConfiguration.Expectations[SelectedHost].Actions != null)
                                                    {
                                                        foreach (Expectation.Action action in currentConfiguration.Expectations[SelectedHost].Actions)
                                                        {
                                                            actionindex++;
                                                            optionslevel4.Add($"{action.actiontype}-{actionindex}", $"");

                                                        }
                                                    }
                                                    
                                                    KeyValuePair<string, string> selectedOptionLevel4 = RenderConfigurationDialogMenu(optionslevel4);
                                                    switch (selectedOptionLevel4.Key)
                                                    {
                                                        case "Add":
                                                            Console.Write("Type (create_incident/update_component): ");
                                                            userInputLevel4 = Console.ReadLine();
                                                            string userInputLevel4incident_title = "";
                                                            string userInputLevel4incident_message = "";
                                                            string userInputLevel4failed_count = "";
                                                            string userInputLevel4incident_solvedmessage = "";
                                                            string userInputLevel4incident_status = "";
                                                            string userInputLevel4incident_component_id = "";
                                                            string userInputLevel4incident_component_status = "";
                                                            string userInputLevel4component_id = "";
                                                            string userInputLevel4component_status = "";
                                                            if (currentConfiguration.Expectations[SelectedHost].Actions == null)
                                                            {
                                                                currentConfiguration.Expectations[SelectedHost].Actions = new List<Expectation.Action>();
                                                            }
                                                            if (userInputLevel4 == "create_incident")
                                                            {
                                                                Console.Write("Incident title (Title of incident): ");
                                                                userInputLevel4incident_title = Console.ReadLine();
                                                                Console.Write("Incident message (Message of incident): ");
                                                                userInputLevel4incident_message = Console.ReadLine();
                                                                Console.Write("How many times the host has failed before the action is ran: ");
                                                                userInputLevel4failed_count = Console.ReadLine();
                                                                Console.Write("Incident solved message (Message of incident when solved): ");
                                                                userInputLevel4incident_solvedmessage = Console.ReadLine();
                                                                Console.Write("Incident status (Status of created incident (fixed=4/investigating=1/watching=3/identified=2)): ");
                                                                userInputLevel4incident_status = Console.ReadLine();
                                                                Console.Write("Incident component id (Optional component ID):");
                                                                userInputLevel4incident_component_id = Console.ReadLine();

                                                                if (userInputLevel4component_id != "")
                                                                {
                                                                    Console.Write("Incident component status (Status of component (Operational=1/Watching=3/PartialOutage=2/MajorOutage=4)): ");
                                                                    userInputLevel4incident_component_status = Console.ReadLine();
                                                                    currentConfiguration.Expectations[SelectedHost].Actions.Add(new Expectation.Action()
                                                                    {
                                                                        actiontype = userInputLevel4,
                                                                        failed_count = Convert.ToInt32(userInputLevel4failed_count),
                                                                        incident_parameters = new Expectation.Action.IncidentParameters()
                                                                        {
                                                                            title = userInputLevel4incident_title,
                                                                            message = userInputLevel4incident_message,
                                                                            solvedmessage = userInputLevel4incident_solvedmessage,
                                                                            componentstatus = Convert.ToInt32(userInputLevel4incident_component_status),
                                                                            component_id = Convert.ToInt32(userInputLevel4incident_component_id),
                                                                            status = Convert.ToInt32(userInputLevel4incident_status)
                                                                        }
                                                                    }) ;
                                                                } else
                                                                {
                                                                    currentConfiguration.Expectations[SelectedHost].Actions.Add(new Expectation.Action()
                                                                    {
                                                                        actiontype = userInputLevel4,
                                                                        failed_count = Convert.ToInt32(userInputLevel4failed_count),
                                                                        incident_parameters = new Expectation.Action.IncidentParameters()
                                                                        {
                                                                            title = userInputLevel4incident_title,
                                                                            message = userInputLevel4incident_message,
                                                                            solvedmessage = userInputLevel4incident_solvedmessage,
                                                                            status = Convert.ToInt32(userInputLevel4incident_status)
                                                                        }
                                                                    });
                                                                }

                                                            } else if (userInputLevel4 == "update_component")
                                                            {
                                                                Console.Write("Component id (component ID):");
                                                                userInputLevel4component_id = Console.ReadLine();
                                                                Console.Write("Component status (Status of component (Operational=1/Watching=3/PartialOutage=2/MajorOutage=4)):");
                                                                userInputLevel4component_status = Console.ReadLine();
                                                                Console.Write("How many times the host has failed before the action is ran: ");
                                                                userInputLevel4failed_count = Console.ReadLine();


                                                                currentConfiguration.Expectations[SelectedHost].Actions.Add(new Expectation.Action()
                                                                {
                                                                    actiontype = userInputLevel4,
                                                                    failed_count = Convert.ToInt32(userInputLevel4failed_count),
                                                                    component_paramters = new Expectation.Action.ComponentParamters()
                                                                    {
                                                                        component_id = Convert.ToInt32(userInputLevel4component_id),
                                                                        component_status = Convert.ToInt32(userInputLevel4component_status)
                                                                    }
                                                                }) ;

                                                            }

                                                            break;
                                                        case "Exit":
                                                            backlevel4 = true;
                                                            break;
                                                        default: //LEVEL5STARTER
                                                            bool backlevel5 = false;
                                                            string userInputLevel5;
                                                            int SelectedAction = Convert.ToInt32(selectedOptionLevel4.Key.Split("-")[1])-1;
                                                            while (!backlevel5)
                                                            {
                                                                Dictionary<string, string> optionslevel5 = new Dictionary<string, string>();
                                                                if (currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].actiontype == "create_incident")
                                                                {
                                                                    if (currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters == null)
                                                                    {
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters = new Expectation.Action.IncidentParameters();
                                                                    }
                                                                    optionslevel5 = new Dictionary<string, string>()
                                                                    {
                                                                        { "Delete", "" },
                                                                        { "actiontype", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].actiontype},
                                                                        { "failed_count", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].failed_count.ToString()},
                                                                        { "incident_title", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.title},
                                                                        { "incident_message", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.message},
                                                                        { "incident_status", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.status.ToString()},
                                                                        { "incident_solvedmessage", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.solvedmessage},
                                                                        { "incident_component_id", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.component_id.ToString()},
                                                                        { "incident_component_status", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.componentstatus.ToString()},
                                                                    };
                                                                } else if (currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].actiontype == "update_component")
                                                                {
                                                                    if (currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].component_paramters == null)
                                                                    {
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].component_paramters = new Expectation.Action.ComponentParamters();
                                                                    }
                                                                    optionslevel5 = new Dictionary<string, string>()
                                                                    {
                                                                        { "Delete", "" },
                                                                        { "actiontype", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].actiontype},
                                                                        { "failed_count", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].failed_count.ToString()},
                                                                        { "component_id", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].component_paramters.component_id.ToString()},
                                                                        { "component_status", currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].component_paramters.component_status.ToString()}
                                                                    };
                                                                } else
                                                                {
                                                                    currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].actiontype = "create_incident";
                                                                    backlevel5 = true;
                                                                }
                                                                KeyValuePair<string, string> selectedOptionLevel5 = RenderConfigurationDialogMenu(optionslevel5);
                                                                switch (selectedOptionLevel5.Key)
                                                                {
                                                                    case "Delete":
                                                                        currentConfiguration.Expectations[SelectedHost].Actions.Remove(currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction]);
                                                                        backlevel5 = true;
                                                                        break;
                                                                    case "failed_count":
                                                                        Console.Write("How many times the host has failed before the action is ran: ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].failed_count = Convert.ToInt32(userInputLevel5);
                                                                        backlevel5 = true;
                                                                        break;
                                                                    case "incident_solvedmessage":
                                                                        Console.Write("Incident solved message (Message of incident when solved): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.solvedmessage = userInputLevel5;
                                                                        backlevel5 = true;
                                                                        break;

                                                                    case "actiontype":
                                                                        Console.Write("Action Type (create_incident/update_component): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].actiontype = userInputLevel5;
                                                                        backlevel5 = true;
                                                                        break;
                                                                    case "incident_title":
                                                                        Console.Write("Incident title (Title of an incient): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.title = userInputLevel5;
                                                                        break;
                                                                    case "incident_message":
                                                                        Console.Write("Incident message (Description of an incient): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.message = userInputLevel5;
                                                                        break;
                                                                    case "incident_status":
                                                                        Console.Write("Incident status (fixed=4/investigating=1/watching=3/identified=2): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.status = Convert.ToInt32(userInputLevel5);
                                                                        break;
                                                                    case "incident_component_id":
                                                                        Console.WriteLine("(Optional. Leave blank if using an update_component action!)");
                                                                        Console.Write("Incident component id (id of component): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.component_id = Convert.ToInt32(userInputLevel5);
                                                                        break;
                                                                    case "incident_component_status":
                                                                        Console.WriteLine("(Optional. Use only if component id is specified)");
                                                                        Console.Write("Incident component status (Operational=1/Watching=3/PartialOutage=2/MajorOutage=4): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].incident_parameters.componentstatus = Convert.ToInt32(userInputLevel5);
                                                                        break;
                                                                    case "component_id":
                                                                        Console.Write("Component ID to update (id): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].component_paramters.component_id = Convert.ToInt32(userInputLevel5);
                                                                        break;
                                                                    case "component_status":
                                                                        Console.Write("Component status (Operational=1/Watching=3/PartialOutage=2/MajorOutage=4): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Expectations[SelectedHost].Actions[SelectedAction].component_paramters.component_status = Convert.ToInt32(userInputLevel5);
                                                                        break;


                                                                    case "Exit":
                                                                        backlevel5 = true;
                                                                        break;
                                                                    default:
                                                                        Console.WriteLine($"{selectedOptionLevel5.Key} is not a valid option.");
                                                                        break;

                                                                }
                                                            }
                                                            break;
                                                    }
                                                }
                                            break;
                                            case "Exit":
                                                backlevel3 = true;
                                                break;
                                            default:
                                                Console.WriteLine($"{selectedOptionLevel3.Key} is not a valid option.");
                                                break;

                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case "Exit":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine($"{selectedOptionLevel1.Key} is not a valid option.");
                        break;

                }
            }

        }
        public static KeyValuePair<string, string> RenderConfigurationDialogMenu(Dictionary<string, string> options)
        {
            KeyValuePair<string, string> selectedOption;
            int DrawCount = 0;
            foreach (KeyValuePair<string, string> menuOption in options)
            {
                DrawCount++;
                if (menuOption.Value != "")
                {
                    Console.WriteLine($"[{DrawCount}] {menuOption.Key} ({menuOption.Value})");
                } else
                {
                    Console.WriteLine($"[{DrawCount}] {menuOption.Key}");
                }
            }
            DrawCount++;
            Console.WriteLine($"[{DrawCount}] Exit");
            Console.Write("Selection: ");
            string userSelection = Console.ReadLine();
            try
            {
                if (Convert.ToInt32(userSelection) > options.Count)
                {
                    selectedOption = new KeyValuePair<string, string>("Exit", "");
                    return selectedOption;
                }
                selectedOption = options.ElementAt(Convert.ToInt32(userSelection) - 1);
                return selectedOption;
            }catch (FormatException) //If user inputs non-menuoption value, return that
            {
                selectedOption = new KeyValuePair<string, string>(userSelection,"");
                return selectedOption;
            }
        }

        private static Configuration currentConfiguration = new Configuration();
        public static Configuration GetConfiguration(string path = "Config.json", bool refresh = false)
        {
            if (refresh || currentConfiguration == null || currentConfiguration.BaseURL == null)
            {
                Console.WriteLine("Reading configuration");

                try
                {
                    currentConfiguration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(path));
                }
                catch (Exception ex)
                {
                    if (ex is FileNotFoundException)
                    {
                        return currentConfiguration;
                    }
                    Console.WriteLine("Error occured while loading configuration:");
                    throw ex;
                }
            }
            return currentConfiguration;
        }
        public static void SaveConfiguration(Configuration configuration, string path = "Config.json")
        {
            try
            {
                File.WriteAllText(path, JsonSerializer.Serialize<Configuration>(configuration, new JsonSerializerOptions() { IgnoreNullValues = true }));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while writing configuration:");
                throw ex;
            }
        }

        public string APIKey { get; set; }
        public string BaseURL { get; set; }
        public int Interval { get; set; }
        public List<Expectation> Expectations { get; set; }
        public class Expectation
        {
            [field: NonSerialized] public string id { get; set; }
            public string path { get; set; }
            public string type { get; set; }
            public static class Types
            {
                public static string ping = "ping";
                public static string http = "http";
            }

            public string ExpectedStatusCodeRange { get; set; }
            public bool verifySSL { get; set; }
            public List<Action> Actions { get; set; }
            public class Action
            {
                public string actiontype { get; set; }
                public static string create_incident = "create_incident";
                public int failed_count { get; set; } = 1;
                public IncidentParameters incident_parameters { get; set; }
                public class IncidentParameters
                {
                    public string title { get; set; }
                    public string message { get; set; }
                    public string solvedmessage { get; set; }
                    public int status { get; set; }
                    public int? component_id { get; set; }
                    public int? componentstatus { get; set; }
                }

                public static string update_component = "update_component";
                public ComponentParamters component_paramters { get; set; }
                public class ComponentParamters
                {
                    public int component_id { get; set; }
                    public int component_status { get; set; }
                }
            }
            public Expectation Clone()
            {
                return this.MemberwiseClone() as Expectation;
            }
        }
    }
}
