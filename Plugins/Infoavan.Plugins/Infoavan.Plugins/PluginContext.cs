using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.Helpers
{
    public class PluginContext
    {
        private IOrganizationServiceFactory serviceFactory;
        private IOrganizationService integrationUserService;

        public IOrganizationService IntegrationUserService
        {
            get { return integrationUserService; }
            set { integrationUserService = value; }
        }

        public IOrganizationServiceFactory ServiceFactory
        {
            get { return serviceFactory; }
            set { serviceFactory = value; }
        }
        private IOrganizationService service;

        public IOrganizationService Service
        {
            get { return service; }
            set { service = value; }
        }
        private ITracingService tracingService;

        public ITracingService TracingService
        {
            get { return tracingService; }
            set { tracingService = value; }
        }
        private StringBuilder sb = new StringBuilder();

        public StringBuilder Sb
        {
            get { return sb; }
            set { sb = value; }
        }
      
        private Microsoft.Xrm.Sdk.IPluginExecutionContext context;

        public Microsoft.Xrm.Sdk.IPluginExecutionContext Context
        {
            get { return context; }
            set { context = value; }
        }



        public PluginContext(IOrganizationServiceFactory serviceFactory, IOrganizationService service, ITracingService tracingService, Microsoft.Xrm.Sdk.IPluginExecutionContext context)
        {

            this.service = service;
            this.tracingService = tracingService;
            this.context = context;
            this.serviceFactory = serviceFactory;
            integrationUserService = changeToUser(ServiceFactory, Service, "System");


        }

        private static IOrganizationService changeToUser(IOrganizationServiceFactory serviceFact, IOrganizationService curService, string userName)
        {
            #region Change To System User
            var userid = getUserId(curService, userName);
            if (userid.HasValue)
            {
                return serviceFact.CreateOrganizationService(userid);
            }
            else
            {
                throw new Exception("Cannot find: " + userName);
            }
            #endregion
        }
        private static Guid? getUserId(IOrganizationService service, String userName)
        {
            QueryExpression qe = new QueryExpression("systemuser")
            {
                Criteria = new FilterExpression(),
                ColumnSet = new ColumnSet()
            };

            qe.Criteria.AddCondition("fullname", ConditionOperator.Equal, userName);

            EntityCollection results = service.RetrieveMultiple(qe);
            if (results.Entities == null || results.Entities.Count == 0) return null;
            return results.Entities[0].Id;
        }
    }

}
