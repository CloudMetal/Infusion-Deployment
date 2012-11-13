using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infusion.Documents.Models;
using Infusion.Documents.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Title.Models;

namespace Infusion.Documents.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IContentManager _contentManager;
        private readonly IDocumentPathConstraint _documentPathConstraint;

        public DocumentService(IContentManager contentManager, IDocumentPathConstraint documentPathConstraint)
        {
            _contentManager = contentManager;
            _documentPathConstraint = documentPathConstraint;
        }

        public DocumentPart Get(string path)
        {
            return _contentManager.Query<DocumentPart>().List().FirstOrDefault(rr => rr.As<IAliasAspect>().Path == path);
        }

        public ContentItem Get(int id, Orchard.ContentManagement.VersionOptions versionOptions)
        {
            return _contentManager.Get(id, versionOptions);
        }

        public IEnumerable<DocumentPart> Get()
        {
            return Get(VersionOptions.Published);
        }

        public IEnumerable<DocumentPart> Get(Orchard.ContentManagement.VersionOptions versionOptions)
        {
            return _contentManager.Query<DocumentPart, DocumentPartRecord>(versionOptions)
                .Join<TitlePartRecord>()
                .OrderBy(br => br.Title)
                .List();
        }

        public void Delete(ContentItem document)
        {
            _contentManager.Remove(document);
            _documentPathConstraint.RemovePath(document.As<IAliasAspect>().Path);
        }
    }
}