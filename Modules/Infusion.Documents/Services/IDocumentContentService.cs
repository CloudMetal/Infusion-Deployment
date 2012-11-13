using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infusion.Documents.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Infusion.Documents.Services
{
    public interface IDocumentContentService : IDependency
    {
        DocumentContentPart Get(int id);
        DocumentContentPart Get(int id, VersionOptions versionOptions);
        IEnumerable<DocumentContentPart> Get(DocumentPart documentPart);
        IEnumerable<DocumentContentPart> Get(DocumentPart documentPart, VersionOptions versionOptions);
        IEnumerable<DocumentContentPart> Get(DocumentPart documentPart, int skip, int count);
        IEnumerable<DocumentContentPart> Get(DocumentPart documentPart, int skip, int count, VersionOptions versionOptions);
        int ContentCount(DocumentPart documentPart);
        int ContentCount(DocumentPart documentPart, VersionOptions versionOptions);
        void Delete(DocumentContentPart documentContentPart);
        void Publish(DocumentContentPart documentContentPart);
        void Publish(DocumentContentPart documentContentPart, DateTime scheduledPublishUtc);
        void Unpublish(DocumentContentPart documentContentPart);
        DateTime? GetScheduledPublishUtc(DocumentContentPart documentContentPart);
    }
}
