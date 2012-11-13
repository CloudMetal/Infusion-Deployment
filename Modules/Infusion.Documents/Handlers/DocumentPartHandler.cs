using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Infusion.Documents.Models;
using Infusion.Documents.Routing;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Infusion.Documents.Handlers
{
    [UsedImplicitly]
    public class DocumentPartHandler : ContentHandler
    {
        private readonly IDocumentPathConstraint _documentPathConstraint;

        public DocumentPartHandler(IRepository<DocumentPartRecord> repository,
            IDocumentPathConstraint documentPathConstraint) {
            _documentPathConstraint = documentPathConstraint;

            Filters.Add(StorageFilter.For(repository));

            OnGetDisplayShape<DocumentPart>((context, document) =>
            {
                context.Shape.Description = document.Description;
            });

            OnPublished<DocumentPart>((context, blog) => _documentPathConstraint.AddPath(blog.As<IAliasAspect>().Path));
            OnUnpublished<DocumentPart>((context, blog) => _documentPathConstraint.RemovePath(blog.As<IAliasAspect>().Path));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var document = context.ContentItem.As<DocumentPart>();

            if (document == null)
                return;

            context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                {"Area", "Infusion.Documents"},
                {"Controller", "Document"},
                {"Action", "Item"},
                {"documentId", context.ContentItem.Id}
            };
            context.Metadata.CreateRouteValues = new RouteValueDictionary {
                {"Area", "Infusion.Documents"},
                {"Controller", "DocumentAdmin"},
                {"Action", "Create"}
            };
            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", "Infusion.Documents"},
                {"Controller", "DocumentAdmin"},
                {"Action", "Edit"},
                {"documentId", context.ContentItem.Id}
            };
            context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                {"Area", "Infusion.Documents"},
                {"Controller", "DocumentAdmin"},
                {"Action", "Remove"},
                {"documentId", context.ContentItem.Id}
            };
            context.Metadata.AdminRouteValues = new RouteValueDictionary {
                {"Area", "Infusion.Documents"},
                {"Controller", "DocumentAdmin"},
                {"Action", "Item"},
                {"documentId", context.ContentItem.Id}
            };
        }
    }
}