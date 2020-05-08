using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace cachet_monitor
{
    class Program
    {
        static bool stopping = false;
        static bool isclean = false;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnExitEventHandler;
            Console.CancelKeyPress += OnCancelKeyPressedEventHandler;

            if (args.Count() > 0 && args[0] == "--config")
            {
                Configuration.GetConfiguration();
                Configuration.ConfigurationDialog();
                isclean = true;
                return;
            } else
            {
                failedComponents = ((Dictionary<string,object>) PersistentData.LoadPersistentData()[0]).ToDictionary(x => x.Key, x=> x.Value.ToString());
                trackedIncidents = ((Dictionary<string, object>)PersistentData.LoadPersistentData()[1]).ToDictionary(x => x.Key, x => x.Value.ToString());
                hostFailCount = ((Dictionary<string, object>)PersistentData.LoadPersistentData()[2]).ToDictionary(x => x.Key, x => Convert.ToInt32(x.Value));

                while (true && !stopping)
                {
                    CheckHosts();
                    if (!stopping)
                    {
                        Thread.Sleep(Configuration.GetConfiguration().Interval * 1000);
                    }
                }
                isclean = true;
            }
        }

        static void OnExitEventHandler(object sender, EventArgs e)
        {
            stopping = true;
            Console.WriteLine("Shutting down...");
            while (!isclean)
            {
                Thread.Sleep(1000);
            }
            Environment.ExitCode = 0;
        }
        static void OnCancelKeyPressedEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Cancel key pressed");
            Environment.Exit(0);
        }
        static API api = new API();
        static void CheckHosts()
        {
            foreach (Configuration.Host host in Configuration.GetConfiguration().Hosts)
            {
                if (!stopping)
                {
                    if (host.type == Configuration.Host.Types.http)
                    {
                        Statuscheck statuscheck = new Statuscheck();
                        int statuscodeMin = Convert.ToInt32(host.ExpectedStatusCodeRange.Substring(0, 3));
                        int statuscodeMax = Convert.ToInt32(host.ExpectedStatusCodeRange.Substring(4, 3));
                        statuscheck.VerifySSL = host.verifySSL;
                        Console.WriteLine("Checking host: " + host.path);
                        bool success = statuscheck.CheckHTTP(host.path, statuscodeMin, statuscodeMax);
                        if (success == false)
                        {
                            RunActions(host, true);
                        }
                        else
                        {
                            RunActions(host, false);
                        }

                    }
                    else if (host.type == Configuration.Host.Types.ping)
                    {
                        throw new NotImplementedException();
                    }
                } else
                {
                    break;
                }
            }
        }

        static Dictionary<string, int> hostFailCount = new Dictionary<string, int>();
        static Dictionary<string, string> trackedIncidents = new Dictionary<string, string>();
        static Dictionary<string, string> failedComponents = new Dictionary<string, string>();
        static void RunActions(Configuration.Host host, bool failed)
        {
            if (failed && !hostFailCount.ContainsKey(host.path))
            {
                hostFailCount.Add(host.path, 0);
            }
            if (failed)
            {
                hostFailCount[host.path]++;
            } else
            {
                if (hostFailCount.ContainsKey(host.path))
                {
                    hostFailCount.Remove(host.path);
                }
            }
            foreach (Configuration.Host.Action action in host.Actions)
            {
                if (!hostFailCount.ContainsKey(host.path) || hostFailCount[host.path] >= action.failed_count)
                {
                    if (action.actiontype == Configuration.Host.Action.create_incident)
                    {
                        try
                        {
                            if (failed && !trackedIncidents.ContainsValue(host.path))
                            {
                                Console.WriteLine("Service failed. Creating new incident");
                                int id = Convert.ToInt32((((api.CreateIncident(action.incident_parameters.title, action.incident_parameters.message, action.incident_parameters.status, 2, action.incident_parameters.component_id, action.incident_parameters.componentstatus))["data"])["id"]));
                                trackedIncidents.Remove(id.ToString());
                                failedComponents.Remove(action.incident_parameters.component_id.Value.ToString());
                                trackedIncidents.Add(id.ToString(), host.path);
                                if (action.incident_parameters.component_id != null)
                                {
                                    failedComponents.Add(action.incident_parameters.component_id.Value.ToString(), host.path);
                                }
                            }
                            else if (!failed && trackedIncidents.ContainsValue(host.path))
                            {
                                Console.WriteLine("Service back up. Setting incident as solved.");
                                int id = Convert.ToInt32((((api.CreateIncidentUpdate(Convert.ToInt32(trackedIncidents.Where(x => x.Value.Contains(host.path)).ElementAt(0).Key), action.incident_parameters.solvedmessage, API.Status.Fixed))["data"])["id"]));
                                if (action.incident_parameters.component_id.HasValue)
                                {
                                    int id2 = Convert.ToInt32((((api.UpdateComponentStatus(action.incident_parameters.component_id.Value, API.ComponentStatus.Operational))["data"])["id"]));
                                    failedComponents.Remove(action.incident_parameters.component_id.Value.ToString());
                                }
                                trackedIncidents.Remove(trackedIncidents.Where(x => x.Value.Contains(host.path)).ElementAt(0).Key);
                            }
                        }
                        catch (NullReferenceException)
                        {
                            Console.WriteLine("Omitting duplicate request");
                        }
                        catch (System.Net.WebException ex)
                        {
                            if (Convert.ToInt32((ex.Response as System.Net.HttpWebResponse).StatusCode) == 404)
                            {
                                Console.WriteLine("Webserver returned 404 while trying to create an incident update. It is likely that a user deleted the incident.");
                            }
                        }
                    }
                    if (action.actiontype == Configuration.Host.Action.update_component && host.Actions.Where(x => x.incident_parameters != null && x.incident_parameters.component_id == action.component_paramters.component_id).Count() < 1)
                    {
                        try
                        {
                            if (failed && !failedComponents.ContainsKey(action.component_paramters.component_id.ToString()))
                            {
                                Console.WriteLine("Service failed. Setting component as failed");
                                failedComponents.Remove(action.component_paramters.component_id.ToString());
                                failedComponents.Add(action.component_paramters.component_id.ToString(), host.path);
                                int id = Convert.ToInt32((((api.UpdateComponentStatus(action.component_paramters.component_id, action.component_paramters.component_status))["data"])["id"]));
                            }
                            else if (!failed && failedComponents.Contains(new KeyValuePair<string, string>(action.component_paramters.component_id.ToString(), host.path)))
                            {
                                Console.WriteLine("Service up. Setting component as operational");
                                failedComponents.Remove(action.component_paramters.component_id.ToString());
                                int id = Convert.ToInt32((((api.UpdateComponentStatus(action.component_paramters.component_id, API.ComponentStatus.Operational))["data"])["id"]));
                            }
                        }
                        catch (NullReferenceException)
                        {
                            Console.WriteLine("Omitting duplicate component request");
                        }
                    }
                    else if (action.component_paramters != null && host.Actions.Where(x => x.incident_parameters.component_id == action.component_paramters.component_id).Count() < 1)
                    {
                        Console.WriteLine("Tried to change component status for component managed by incident!");
                    }
                } else
                {
                    Console.WriteLine($"{host.path} failed {hostFailCount[host.path]}/{action.failed_count}");
                }
            }
            PersistentData.SavePersistentData(failedComponents, trackedIncidents, hostFailCount);
        }
    }
}
