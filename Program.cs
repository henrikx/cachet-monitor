using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace cachet_monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() > 0 && args[0] == "--config")
            {
                Configuration.GetConfiguration();
                Configuration.ConfigurationDialog();
                return;
            } else
            {
                while (true)
                {
                    CheckHosts();
                    Thread.Sleep(Configuration.GetConfiguration().Interval * 1000);
                }

            }
        }
        static API api = new API();
        static void CheckHosts()
        {
            foreach (Configuration.Host host in Configuration.GetConfiguration().Hosts)
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
                    } else
                    {
                        RunActions(host, false);
                    }

                } else if (host.type == Configuration.Host.Types.ping)
                {
                    throw new NotImplementedException();
                }

            }
        }

        static Dictionary<int, string> trackedIncidents = new Dictionary<int, string>();
        static void RunActions(Configuration.Host host, bool failed)
        {
            Console.WriteLine("Running actions");
            foreach (Configuration.Host.Action action in host.Actions)
            {
                if (action.actiontype == Configuration.Host.Action.create_incident)
                {
                    try
                    {
                        if (failed && !trackedIncidents.ContainsValue(host.path))
                        {
                            Console.WriteLine("Service failed. Creating new incident");
                            int id = Convert.ToInt32((((api.CreateIncident(action.incident_parameters.title, action.incident_parameters.message, action.incident_parameters.status, 2, action.incident_parameters.component_id, action.incident_parameters.componentstatus))["data"])["id"]));
                            trackedIncidents.Remove(id);
                            trackedIncidents.Add(id, host.path);
                        } else if (!failed && trackedIncidents.ContainsValue(host.path))
                        {
                            Console.WriteLine("Service back up. Setting incident as solved.");
                            int id = Convert.ToInt32((((api.CreateIncidentUpdate(trackedIncidents.Where(x => x.Value.Contains(host.path)).ElementAt(0).Key, action.incident_parameters.solvedmessage, API.Status.Fixed))["data"])["id"]));
                            if (host.Actions.Where(x => x.incident_parameters.component_id.HasValue).Count() > 0)
                            {
                                int id2 = Convert.ToInt32((((api.UpdateComponentStatus(action.incident_parameters.component_id.Value, API.ComponentStatus.Operational))["data"])["id"]));

                            }
                            trackedIncidents.Remove(trackedIncidents.Where(x => x.Value.Contains(host.path)).ElementAt(0).Key);
                        }
                    } catch (NullReferenceException)
                    {
                        Console.WriteLine("Omitting duplicate request");
                    } catch (System.Net.WebException ex)
                    {
                        if (Convert.ToInt32((ex.Response as System.Net.HttpWebResponse).StatusCode) == 404)
                        {
                            Console.WriteLine("Webserver returned 404 while trying to create an incident update. It is likely that a user deleted the incident.");
                        }
                    }
                }
                if (action.actiontype == Configuration.Host.Action.update_component && host.Actions.Where(x => x.incident_parameters.component_id == action.component_paramters.component_id).Count() < 1)
                {
                    try
                    {
                        if (failed)
                        {
                            Console.WriteLine("Service failed. Setting component as failed");

                            int id = Convert.ToInt32((((api.UpdateComponentStatus(action.component_paramters.component_id, action.component_paramters.component_status))["data"])["id"]));
                        } else
                        {
                            Console.WriteLine("Service back up. Setting component as operational");
                            int id = Convert.ToInt32((((api.UpdateComponentStatus(action.component_paramters.component_id, API.ComponentStatus.Operational))["data"])["id"]));
                        }
                    } catch (NullReferenceException)
                    {
                        Console.WriteLine("Omitting duplicate component request");
                    }
                } else if (action.component_paramters != null && host.Actions.Where(x => x.incident_parameters.component_id == action.component_paramters.component_id).Count() < 1)
                {
                    Console.WriteLine("Tried to change component status for component managed by incident!");
                }
            }
            Console.WriteLine("Actions ran");
        }
    }
}
