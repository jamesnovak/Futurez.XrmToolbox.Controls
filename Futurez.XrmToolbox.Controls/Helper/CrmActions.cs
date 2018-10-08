﻿using System.Linq;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System;
using Microsoft.Xrm.Sdk.Query;

namespace Futurez.XrmToolbox.Controls
{
    /// <summary>
    /// Various helper methods for working with CRM queries, objects, etc.
    /// </summary>
    public class CrmActions
    {
        #region Entities
        /// <summary>
        /// Rerieve all entities with the given filter conditions
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityFilters"></param>
        /// <param name="retrieveAsIfPublished"></param>
        /// <returns></returns>
        public static List<EntityMetadata> RetrieveAllEntities(IOrganizationService service, List<EntityFilters> entityFilters = null, bool retrieveAsIfPublished = true)
        {
            if (entityFilters == null) {
                entityFilters = new List<EntityFilters>() { EntityFilters.Default };
            }

            // build the bitwise or list of the entity filters
            var filters = entityFilters.Aggregate<EntityFilters, EntityFilters>(0, (current, item) => current | item);

            var req = new RetrieveAllEntitiesRequest() {
                EntityFilters = filters,
                RetrieveAsIfPublished = retrieveAsIfPublished
            };
            var resp = (RetrieveAllEntitiesResponse)service.Execute(req);

            // set the itemsource of the itembox equal to entity metadata that is customizable (takes out systemjobs and stuff like that)
            var entities = resp.EntityMetadata.Where(x => x.IsCustomizable.Value == true).ToList<EntityMetadata>();

            return entities;
        }

        /// <summary>
        /// Rerieve all entities with the given filter conditions
        /// </summary>
        /// <param name="service"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<EntityMetadata> RetrieveAllEntities(IOrganizationService service, ConfigurationInfo config)
        {
            return RetrieveAllEntities(service, config.EntityRequestFilters, config.RetrieveAsIfPublished);
        }

        /// <summary>
        /// Retrieve an Entity Metadata and include all Entity details
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogicalName"></param>
        /// <param name="retrieveAsIfPublished"></param>
        /// <returns></returns>
        public static EntityMetadata RetrieveEntity(IOrganizationService service, string entityLogicalName, bool retrieveAsIfPublished)
        {
            return RetrieveEntity(service, entityLogicalName, retrieveAsIfPublished, new List<EntityFilters> { EntityFilters.All });
        }

        /// <summary>
        /// Retrieve an Entity Metadata and include Entity details as specified in the EntityFilters provided 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogicalName"></param>
        /// <param name="retrieveAsIfPublished"></param>
        /// <param name="entityFilters"></param>
        /// <returns></returns>
        public static EntityMetadata RetrieveEntity(IOrganizationService service, string entityLogicalName, bool retrieveAsIfPublished, List<EntityFilters> entityFilters)
        {
            var filters = entityFilters.Aggregate<EntityFilters, EntityFilters>(0, (current, item) => current | item);

            var req = new RetrieveEntityRequest() {
                RetrieveAsIfPublished = retrieveAsIfPublished,
                EntityFilters = EntityFilters.All,
                LogicalName = entityLogicalName
            };

            var resp = (RetrieveEntityResponse)service.Execute(req);

            return resp.EntityMetadata;
        }

        /// <summary>
        /// Retrieve entities in a list of Entity logical names
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogcialNames"></param>
        /// <param name="retrieveAsIfPublished"></param>
        /// <param name="entityFilters"></param>
        /// <returns></returns>
        public static List<EntityMetadata> RetrieveEntity(IOrganizationService service, List<string> entityLogcialNames, bool retrieveAsIfPublished = true, List<EntityFilters> entityFilters = null) {

            var entities = new List<EntityMetadata>();
            // Create an ExecuteMultipleRequest object.
            var batchRequest = new ExecuteMultipleRequest() {
                Settings = new ExecuteMultipleSettings() {
                    ContinueOnError = false,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (var entityLogicalName in entityLogcialNames) {
                batchRequest.Requests.Add(
                    new RetrieveEntityRequest() {
                        RetrieveAsIfPublished = retrieveAsIfPublished,
                        EntityFilters = EntityFilters.All,
                        LogicalName = entityLogicalName
                    });
            }

            var responses = (ExecuteMultipleResponse)service.Execute(batchRequest);


            foreach (var resp in responses.Responses) {
                entities.Add(((RetrieveEntityResponse)resp.Response).EntityMetadata);
            }

            return entities;
        }

        #endregion

        #region Entity Keys

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogicalName"></param>
        /// <param name="logicalName"></param>
        /// <param name="keyDisplayName"></param>
        /// <param name="keyAttributes"></param>
        public static string CreateEntityKey(IOrganizationService service, string entityLogicalName, string logicalName, string keyDisplayName, List< string > keyAttributes)
        {
            try
            {
                var entKeyMeta = new EntityKeyMetadata() {
                    KeyAttributes = keyAttributes.ToArray(),
                    LogicalName = logicalName,
                    DisplayName = new Label(keyDisplayName, 1033),
                    SchemaName = logicalName
                };
                var req = new CreateEntityKeyRequest() {
                    EntityKey = entKeyMeta,
                    EntityName = entityLogicalName
                };

                var resp = service.Execute(req) as CreateEntityKeyResponse;

                return null;
            }
            catch (Exception ex) {
                return $"Error createing the new Alternate Key: {keyDisplayName} ({logicalName}): \n{ex.Message}";
            }
        }

        /// <summary>
        /// Reactivate an Entity Key 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<string> ReactivateEntityKey(IOrganizationService service, EntityKeyMetadata key) {

            try {
                // call the Activate action for the selected key 
                var req = new ReactivateEntityKeyRequest() {
                    EntityKeyLogicalName = key.SchemaName,
                    EntityLogicalName = key.EntityLogicalName
                };

                var resp = (ReactivateEntityKeyResponse)service.Execute(req);

                return null;
            }
            catch(Exception ex)
            {
                return new List<string>() {$"Error reactivating key {key.SchemaName}: \n{ex.Message}" };
            }
        }

        /// <summary>
        /// Reactivate a batch of Entity Alternate Keys
        /// </summary>
        /// <param name="service"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static List<string> ReactivateEntityKey(IOrganizationService service, List<EntityKeyMetadata> keys)
        {
            // Create an ExecuteMultipleRequest object.
            var batchRequest = new ExecuteMultipleRequest() {
                Settings = new ExecuteMultipleSettings() {
                    ContinueOnError = false,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            // add the requests
            foreach (var key in keys) 
            {
                // call the Activate action for the selected key 
                batchRequest.Requests.Add(new ReactivateEntityKeyRequest() {
                    EntityKeyLogicalName = key.SchemaName,
                    EntityLogicalName = key.EntityLogicalName
                });
            }

            var responses = (ExecuteMultipleResponse)service.Execute(batchRequest);

            if (responses.IsFaulted) {

                var faultList = new List<string>();

                foreach (var resp in responses.Responses) {
                    var respDetail = resp.Response as ReactivateEntityKeyResponse;
                    var name = (respDetail != null) ? respDetail.ResponseName : $"Error code: {resp.Fault.ErrorCode.ToString()}";
                    faultList.Add($"Error reactivating key {name}\n{resp.Fault.Message}");
                }
                return faultList;
            }
            else {
                return null;
            }            
        }

        /// <summary>
        /// Retrieve an Entity Key Metadata record 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="config"></param>
        /// <param name="entity"></param>
        /// <param name="retrieveAsIfPublished"></param>
        /// <returns></returns>
        public static EntityKeyMetadata RetrieveEntityKey(IOrganizationService service, ConfigurationInfo config, EntityMetadata entity, bool retrieveAsIfPublished)
        {
            var req = new RetrieveEntityKeyRequest() {
                EntityLogicalName = entity.LogicalName,
                RetrieveAsIfPublished = true
            };

            var resp = service.Execute(req) as RetrieveEntityKeyResponse;
            return resp.EntityKeyMetadata;
        }

        /// <summary>
        /// Delete a selected Entity Alternate Key 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        public static void DeleteEntityKey(IOrganizationService service, EntityKeyMetadata key)
        {
            var req = new DeleteEntityKeyRequest() {
                EntityLogicalName = key.EntityLogicalName,
                Name = key.LogicalName
            };

            var resp = service.Execute(req) as DeleteEntityKeyResponse;
        }

        /// <summary>
        /// Delete a list of Alternate Keys
        /// </summary>
        /// <param name="service"></param>
        /// <param name="keys"></param>
        public static void DeleteEntityKey(IOrganizationService service, List<EntityKeyMetadata> keys)
        {
            var batchRequest = new ExecuteMultipleRequest() {
                Settings = new ExecuteMultipleSettings() {
                    ContinueOnError = false,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            // add the requests
            foreach (var key in keys) 
            {
                batchRequest.Requests.Add(new DeleteEntityKeyRequest() {
                    EntityLogicalName = key.EntityLogicalName,
                    Name = key.LogicalName
                });
            }
            var resp = service.Execute(batchRequest) as DeleteEntityKeyResponse;
        }
        #endregion

        #region Attributes
        /// <summary>
        /// Retrieve all attributes for an Entity
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogicalName"></param>
        /// <param name="retrieveAsIfPublished"></param>
        /// <returns></returns>
        public static List<AttributeMetadata> RetrieveEntityAttributes(IOrganizationService service, string entityLogicalName, bool retrieveAsIfPublished)
        {
            // Retrieve the attribute metadata
            var req = new RetrieveEntityRequest() {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.Attributes,
                RetrieveAsIfPublished = retrieveAsIfPublished
            };
            var resp = (RetrieveEntityResponse)service.Execute(req);

            return resp.EntityMetadata.Attributes.ToList();
        }

        /// <summary>
        /// Retrieve all attributes for an Entity
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogicalName"></param>
        /// <returns></returns>
        public static List<AttributeMetadata> RetrieveEntityAttributes(IOrganizationService service, string entityLogicalName) {

            return RetrieveEntityAttributes(service, entityLogicalName, true);
        }
        #endregion

        #region Additional Metadata helper methods 
        /// <summary>
        /// Rtrieve a list of Publishers with some common attributes
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static List<Entity> RetrievePublishers(IOrganizationService service)
        {
            var query = new QueryExpression("publisher") {
                ColumnSet = new ColumnSet("uniquename", "friendlyname", "isreadonly", "description", "customizationprefix", "publisherid", "organizationid")
            };
            var resp = service.RetrieveMultiple(query);

            return resp.Entities.ToList();
        }

        /// <summary>
        /// Retrieve a list of Solutions with some common attributes
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static List<Entity> RetrieveSolutions(IOrganizationService service)
        {
            // Instantiate QueryExpression QEsolution
            var query = new QueryExpression("solution") {
                ColumnSet = new ColumnSet("friendlyname", "publisherid", "organizationid", "solutionid", "uniquename", "installedon", "description", "solutiontype", "ismanaged")
            };

            var resp = service.RetrieveMultiple(query);

            return resp.Entities.ToList();
        }
        #endregion
        /// <summary>
        /// Helper method to get the first label in the list of LocalizedLabels 
        /// </summary>
        /// <param name="label">Label object containing Localized Labels </param>
        /// <param name="valueIfNull">If the Localizd Labels are null, use this value instead</param>
        /// <returns></returns>
        public static string GetLocalizedLabel(Label label, string valueIfNull) {
            var labels = label.LocalizedLabels;
            return (labels.Count > 0) ? labels[0].Label : valueIfNull;

        }
    }
}
