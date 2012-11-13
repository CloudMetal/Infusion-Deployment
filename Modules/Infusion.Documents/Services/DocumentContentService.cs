using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infusion.Documents.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Tasks.Scheduling;

namespace Infusion.Documents.Services
{
    public class DocumentContentService : IDocumentContentService
    {
        private readonly IContentManager _contentManager;
        private readonly IRepository<DocumentDocumentContentsRecord> _documentAssociationsRepository; 
        private readonly IPublishingTaskManager _publishingTaskManager;

        public DocumentContentService(
            IContentManager contentManager,
            IRepository<DocumentDocumentContentsRecord> documentAssociationsRepository, 
            IPublishingTaskManager publishingTaskManager) {
            _contentManager = contentManager;
            _documentAssociationsRepository = documentAssociationsRepository;
            _publishingTaskManager = publishingTaskManager;
        }

        public Models.DocumentContentPart Get(int id)
        {
            return Get(id, VersionOptions.Published);
        }

        public Models.DocumentContentPart Get(int id, Orchard.ContentManagement.VersionOptions versionOptions)
        {
            return _contentManager.Get<DocumentContentPart>(id, versionOptions);
        }

        public IEnumerable<Models.DocumentContentPart> Get(Models.DocumentPart documentPart)
        {
            return Get(documentPart, VersionOptions.Published);
        }

        public IEnumerable<Models.DocumentContentPart> Get(Models.DocumentPart documentPart, Orchard.ContentManagement.VersionOptions versionOptions)
        {
            return GetDocumentQuery(documentPart, versionOptions).List().Select(ci => ci.As<DocumentContentPart>());
        }

        public IEnumerable<Models.DocumentContentPart> Get(Models.DocumentPart documentPart, int skip, int count)
        {
            return Get(documentPart, skip, count, VersionOptions.Published);
        }

        public IEnumerable<Models.DocumentContentPart> Get(Models.DocumentPart documentPart, int skip, int count, Orchard.ContentManagement.VersionOptions versionOptions)
        {
            return GetDocumentQuery(documentPart, versionOptions)
                    .Slice(skip, count)
                    .ToList()
                    .Select(ci => ci.As<DocumentContentPart>());
        }

        public void Delete(Models.DocumentContentPart documentContentPart)
        {
            _publishingTaskManager.DeleteTasks(documentContentPart.ContentItem);
            _contentManager.Remove(documentContentPart.ContentItem);
        }

        public void Publish(Models.DocumentContentPart documentContentPart)
        {
            _publishingTaskManager.DeleteTasks(documentContentPart.ContentItem);
            _contentManager.Publish(documentContentPart.ContentItem);
        }

        public void Publish(Models.DocumentContentPart documentContentPart, DateTime scheduledPublishUtc)
        {
            _publishingTaskManager.Publish(documentContentPart.ContentItem, scheduledPublishUtc);
        }

        public void Unpublish(Models.DocumentContentPart documentContentPart)
        {
            _contentManager.Unpublish(documentContentPart.ContentItem);
        }

        public DateTime? GetScheduledPublishUtc(Models.DocumentContentPart documentContentPart)
        {
            var task = _publishingTaskManager.GetPublishTask(documentContentPart.ContentItem);
            return (task == null ? null : task.ScheduledUtc);
        }

        private IContentQuery<ContentItem, CommonPartRecord> GetDocumentQuery(ContentPart<DocumentPartRecord> document, VersionOptions versionOptions)
        {
            return
                _contentManager.Query(versionOptions, "DocumentContent")
                .Join<CommonPartRecord>().Where(
                    cr => cr.Container == document.Record.ContentItemRecord).OrderByDescending(cr => cr.CreatedUtc)
                .WithQueryHintsFor("DocumentContent")
                    ;
        }


        public int ContentCount(DocumentPart documentPart)
        {
            return ContentCount(documentPart, VersionOptions.Published);
        }

        public int ContentCount(DocumentPart documentPart, VersionOptions versionOptions)
        {
            return GetDocumentQuery(documentPart, versionOptions).Count();
        }
    }
}