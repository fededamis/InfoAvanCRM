using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Plugin.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infoavan.Plugins
{
    public class HttpGetContactApi : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            var ctx = new PluginContext(factory, service, tracer, context);

            WebResponse response = null;
            Stream dataStreamResponse = null;
            StreamReader reader = null;

            try
            {
                ctx.TracingService.Trace("[BEGIN] Main");
                string contactId = context.InputParameters["ContactId"].ToString();

                if (String.IsNullOrEmpty(contactId))
                    throw new Exception("No se envió el parametro contactid.");

                string url = "https://serviciocontacto.azurewebsites.net/" + contactId;

                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";                
                request.ContentType = "application/json";
                //Token OAuth 2.0
                string token = GetToken();
                request.Headers["Authorization"] = "Bearer " + token;

                //Se comenta código para evitar que de error 404

                //response = request.GetResponse();
                //dataStreamResponse = response.GetResponseStream();
                //reader = new StreamReader(dataStreamResponse);
                //string responseFromServer = reader.ReadToEnd();

                //Devuelvo MOCK del servicio
                string responseFromServer = @"
                {
                    ""code"":""1"",
                    ""description"":""DESCRIPCION MOCK API""
                }";

                context.OutputParameters["Resultado"] = responseFromServer;
                ctx.TracingService.Trace("[END] Main");
            }
            catch (Exception ex)
            {
                context.OutputParameters["Error"] = "Error: " + ex.Message;
            }     
            finally
            {
                //Cierro los objetos del WebResponse
                if (response != null)
                {                    
                    if (reader != null)
                        reader.Close();
                    if (dataStreamResponse != null)
                        dataStreamResponse.Close();
                    response.Close();                    
                }
            }
        }
        private string GetToken()
        {
            return "TOKEN DE PRUEBA";
        }
    }
}
