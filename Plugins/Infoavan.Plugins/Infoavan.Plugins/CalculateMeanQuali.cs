﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Plugin.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoavan.Plugins
{
    public class CalculateMeanQuali : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            var ctx = new PluginContext(factory, service, tracer, context);

            try
            {
                ctx.TracingService.Trace("[BEGIN] Main");
                string nombreCliente = context.InputParameters["ClientName"].ToString();
                Entity cliente = GetCliente(ctx, nombreCliente);
                EntityCollection contactos = GetContactos(ctx, cliente);
                float? mean = GetPromedioContactos(ctx, contactos);
                context.OutputParameters["Promedio"] = mean;
                ctx.TracingService.Trace("[END] Main");
            }
            catch (Exception ex)
            {
                context.OutputParameters["Error"] = "Error: " + ex.Message;
            }
        }

        private Entity GetCliente(PluginContext ctx, string nombreCliente)
        {
            ctx.TracingService.Trace("[BEGIN] GetCliente");
            Entity result = null;

            QueryExpression query = new QueryExpression("account");
            query.ColumnSet = new ColumnSet(false);
            query.Criteria.AddCondition("name", ConditionOperator.Equal, nombreCliente);
            EntityCollection clientes = ctx.Service.RetrieveMultiple(query);

            if (clientes.Entities.Count == 0)
                throw new Exception("No existen clientes con el nombre " + nombreCliente);

            if (clientes.Entities.Count > 1)
                throw new Exception("Existe más de un cliente con el nombre " + nombreCliente);

            result = clientes.Entities.First();
            ctx.TracingService.Trace("[END] GetCliente");
            return result;
        }

        private EntityCollection GetContactos(PluginContext ctx, Entity cliente)
        {
            ctx.TracingService.Trace("[BEGIN] GetContactos");
            EntityCollection result = null;

            QueryExpression query = new QueryExpression("contact");
            query.ColumnSet = new ColumnSet("info_qualification");
            query.Criteria.AddCondition("parentcustomerid", ConditionOperator.Equal, cliente.ToEntityReference().Id);
            EntityCollection contactos = ctx.Service.RetrieveMultiple(query);

            if (contactos.Entities.Count == 0)
                return result;            

            result = contactos;
            ctx.TracingService.Trace("[END] GetContactos");
            return result;
        }

        private float? GetPromedioContactos(PluginContext ctx, EntityCollection contactos)
        {
            ctx.TracingService.Trace("[BEGIN] GetPromedioContactos");
            float? promedio = 0f;
            float? suma = 0f;

            if (contactos == null || contactos.Entities.Count == 0)
                return promedio;

            foreach (var contacto in contactos.Entities)
            {
                float? calificacion = (float?)contacto.GetAttributeValue<double>("info_qualification");

                if (calificacion == null)
                    throw new Exception("Existen contactos asociados al cliente que no tienen una calificación cargada.");

                suma += calificacion;
            }

            promedio = suma / (float?)contactos.Entities.Count;
            ctx.TracingService.Trace("[END] GetPromedioContactos");
            return promedio;
        }
    }
}
