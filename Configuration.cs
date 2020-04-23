using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Collections;
using System.Linq;

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
                            int hostindex = 0;
                            if (currentConfiguration.Hosts == null)
                            {
                                currentConfiguration.Hosts = new List<Host>();
                            }
                            foreach (Host host in currentConfiguration.Hosts)
                            {
                                hostindex++;
                                optionslevel2.Add(host.path+"-"+hostindex, host.type);
                            }
                            KeyValuePair<string, string> selectedOptionLevel2 = RenderConfigurationDialogMenu(optionslevel2);
                            switch (selectedOptionLevel2.Key)
                            {
                                case "Add":
                                    Console.Write("Path (URL for HTTP and IP for ping): ");
                                    userInputLevel2 = Console.ReadLine();
                                    if (currentConfiguration.Hosts == null)
                                    {
                                        currentConfiguration.Hosts = new List<Host>();
                                    }
                                    currentConfiguration.Hosts.Add(new Host { path = userInputLevel2 });
                                    break;
                                case "Exit":
                                    backlevel2 = true;
                                    break;
                                default: //LEVEL3STARTER Host Details
                                    bool backlevel3 = false;
                                    string userInputLevel3;
                                    int SelectedHost = currentConfiguration.Hosts.FindIndex(host => host == currentConfiguration.Hosts.FindAll(host => host.path == selectedOptionLevel2.Key.Split("-")[0])[Convert.ToInt32(selectedOptionLevel2.Key.Split("-")[1])-1]); //Find index of userselected host
                                    while (!backlevel3) 
                                    {
                                        Dictionary<string, string> optionslevel3 = new Dictionary<string, string>()
                                        {
                                            { "Delete", "" },
                                            { "Path", currentConfiguration.Hosts[SelectedHost].path },
                                            { "Type", currentConfiguration.Hosts[SelectedHost].type },
                                            { "Expected Status Code Range", currentConfiguration.Hosts[SelectedHost].ExpectedStatusCodeRange },
                                            { "Verify SSL", currentConfiguration.Hosts[SelectedHost].verifySSL.ToString() },
                                            { "Actions", "Actions List" }
                                        };
                                        KeyValuePair<string, string> selectedOptionLevel3 = RenderConfigurationDialogMenu(optionslevel3);
                                        switch (selectedOptionLevel3.Key)
                                        {
                                            case "Delete":
                                                currentConfiguration.Hosts.Remove(currentConfiguration.Hosts[SelectedHost]);
                                                backlevel3 = true;
                                                break;
                                            case "Path":
                                                Console.Write("Path (URL for HTTP and IP for ping): ");
                                                userInputLevel3 = Console.ReadLine();
                                                currentConfiguration.Hosts[SelectedHost].path = userInputLevel3;
                                                backlevel3 = true;
                                                break;
                                            case "Type":
                                                Console.Write("Type (http/ping): ");
                                                userInputLevel3 = Console.ReadLine();
                                                currentConfiguration.Hosts[SelectedHost].type = userInputLevel3;
                                                break;
                                            case "Expected Status Code Range":
                                                Console.Write("Expected Status Code Range (from-to): ");
                                                userInputLevel3 = Console.ReadLine();
                                                currentConfiguration.Hosts[SelectedHost].ExpectedStatusCodeRange = userInputLevel3;
                                                break;
                                            case "Verify SSL":
                                                Console.Write("Verify SSL (true/false): ");
                                                userInputLevel3 = Console.ReadLine();
                                                try
                                                {
                                                    currentConfiguration.Hosts[SelectedHost].verifySSL = Convert.ToBoolean(userInputLevel3);
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
                                                    if (currentConfiguration.Hosts[SelectedHost].Actions != null)
                                                    {
                                                        foreach (Host.Action action in currentConfiguration.Hosts[SelectedHost].Actions)
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
                                                            if (currentConfiguration.Hosts[SelectedHost].Actions == null)
                                                            {
                                                                currentConfiguration.Hosts[SelectedHost].Actions = new List<Host.Action>();
                                                            }
                                                            currentConfiguration.Hosts[SelectedHost].Actions.Add(new Host.Action() { actiontype = userInputLevel4 });
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
                                                                if (currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].actiontype == "create_incident")
                                                                {
                                                                    if (currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters == null)
                                                                    {
                                                                        currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters = new Host.Action.IncidentParameters();
                                                                    }
                                                                    optionslevel5 = new Dictionary<string, string>()
                                                                        {
                                                                            { "Delete", "" },
                                                                            { "actiontype", currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].actiontype},
                                                                            { "incident_title", currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters.title},
                                                                            { "incident_message", currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters.message},
                                                                            { "incident_status", currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters.status.ToString()},
                                                                            { "incident_component_id", currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters.component_id.ToString()},
                                                                            { "incident_component_status", currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters.componentstatus.ToString()},
                                                                        };
                                                                } else if (currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].actiontype == "update_component")
                                                                {
                                                                    if (currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].component_paramters == null)
                                                                    {
                                                                        currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].component_paramters = new Host.Action.ComponentParamters();
                                                                    }
                                                                    optionslevel5 = new Dictionary<string, string>()
                                                                        {
                                                                            { "actiontype", currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].actiontype},
                                                                            { "component_id", currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].component_paramters.component_id.ToString()},
                                                                            { "component_status", currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].component_paramters.component_status.ToString()}
                                                                        };
                                                                } else
                                                                {
                                                                    currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].actiontype = "create_incident";
                                                                    backlevel5 = true;
                                                                }
                                                                KeyValuePair<string, string> selectedOptionLevel5 = RenderConfigurationDialogMenu(optionslevel5);
                                                                switch (selectedOptionLevel5.Key)
                                                                {
                                                                    case "Delete":
                                                                        currentConfiguration.Hosts[SelectedHost].Actions.Remove(currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction]);
                                                                        backlevel5 = true;
                                                                        break;
                                                                    case "actiontype":
                                                                        Console.Write("Action Type (create_incident/update_component): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].actiontype = userInputLevel5;
                                                                        backlevel5 = true;
                                                                        break;
                                                                    case "incident_title":
                                                                        Console.Write("Incident title (Title of an incient): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters.title = userInputLevel5;
                                                                        break;
                                                                    case "incident_message":
                                                                        Console.Write("Incident message (Description of an incient): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters.message = userInputLevel5;
                                                                        break;
                                                                    case "incident_status":
                                                                        Console.Write("Incident status (fixed=4/investigating=1/watching=3/identified=2): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters.status = Convert.ToInt32(userInputLevel5);
                                                                        break;
                                                                    case "incident_component_id":
                                                                        Console.WriteLine("(Optional. Leave blank if using an update_component action!)");
                                                                        Console.Write("Incident component id (id of component): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters.component_id = Convert.ToInt32(userInputLevel5);
                                                                        break;
                                                                    case "incident_component_status":
                                                                        Console.WriteLine("(Optional. Use only if component id is specified)");
                                                                        Console.Write("Incident component status (Operational=1/Watching=3/PartialOutage=2/MajorOutage=4): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].incident_parameters.componentstatus = Convert.ToInt32(userInputLevel5);
                                                                        break;
                                                                    case "component_id":
                                                                        Console.Write("Component ID to update (id): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].component_paramters.component_id = Convert.ToInt32(userInputLevel5);
                                                                        break;
                                                                    case "component_status":
                                                                        Console.Write("Component status (fixed=4/investigating=1/watching=3/identified=2): ");
                                                                        userInputLevel5 = Console.ReadLine();
                                                                        currentConfiguration.Hosts[SelectedHost].Actions[SelectedAction].component_paramters.component_status = Convert.ToInt32(userInputLevel5);
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
                    Console.WriteLine("Error occured while loading configuration:");
                    if (ex is FileNotFoundException)
                    {
                        return currentConfiguration;
                    }
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
        public List<Host> Hosts { get; set; }
        public class Host
        {
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
        }
    }
}
