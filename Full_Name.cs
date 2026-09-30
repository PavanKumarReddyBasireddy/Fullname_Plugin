using System;
using Microsoft.Xrm.Sdk;

namespace Fullname_Plugin
{
    public class Full_Name : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            tracingService.Trace("Plugin execution started.");

            if ((context.MessageName != "Create" && context.MessageName != "Update")
                || !context.InputParameters.Contains("Target"))
            {
                tracingService.Trace("Not Create/Update or Target missing. Exiting.");
                return;
            }

            var en = (Entity)context.InputParameters["Target"];
            tracingService.Trace("Target entity received.");

            Entity preImage = null;

            if (context.MessageName == "Update" && context.PreEntityImages.Contains("PreImage"))
            {
                preImage = context.PreEntityImages["PreImage"];
                tracingService.Trace("PreImage found.");
            }
            else if (context.MessageName == "Update")
            {
                tracingService.Trace("PreImage NOT found.");
            }

            string first = en.Contains("pk_first_name")
                ? en.GetAttributeValue<string>("pk_first_name")
                : preImage?.GetAttributeValue<string>("pk_first_name");

            string last = en.Contains("pk_last_name")
                ? en.GetAttributeValue<string>("pk_last_name")
                : preImage?.GetAttributeValue<string>("pk_last_name");

            tracingService.Trace($"First Name: {first}");
            tracingService.Trace($"Last Name: {last}");

            string full = $"{first} {last}".Trim();

            tracingService.Trace($"Full Name generated: {full}");

            en["pk_full_name"] = full;

            tracingService.Trace("Full Name set on entity.");
            tracingService.Trace("Plugin execution completed.");
        }
    }
}