using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Infusion.Documents.Models;
using Infusion.Documents.Services;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Infusion.Documents.Handlers
{
    [UsedImplicitly]
    public class DocumentContentPartHandler : ContentHandler
    {
        private readonly IDocumentService _documentService;
        private readonly IDocumentContentService _documentContentService;

        public DocumentContentPartHandler(IRepository<DocumentContentPartRecord> repository,
            IDocumentService documentService,
            IDocumentContentService documentContentService) {
            _documentService = documentService;
            _documentContentService = documentContentService;

            Filters.Add(StorageFilter.For(repository));

            OnGetDisplayShape<DocumentContentPart>(SetModelProperties);
            OnGetEditorShape<DocumentContentPart>(SetModelProperties);
            OnUpdateEditorShape<DocumentContentPart>(SetModelProperties);

            //OnCreated<BlogPostPart>((context, part) => UpdateBlogPostCount(part));
            //OnPublished<BlogPostPart>((context, part) => UpdateBlogPostCount(part));
            //OnUnpublished<BlogPostPart>((context, part) => UpdateBlogPostCount(part));
            //OnVersioned<BlogPostPart>((context, part, newVersionPart) => UpdateBlogPostCount(newVersionPart));
            //OnRemoved<BlogPostPart>((context, part) => UpdateBlogPostCount(part));

            OnRemoved<DocumentPart>(
                (context, b) =>
                documentContentService.Get(context.ContentItem.As<DocumentPart>()).ToList().ForEach(
                    blogPost => context.ContentManager.Remove(blogPost.ContentItem)));
        }

        private static void SetModelProperties(BuildShapeContext context, DocumentContentPart documentContentPart)
        {
            context.Shape.Document = documentContentPart.DocumentPart;
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var content = context.ContentItem.As<DocumentContentPart>();

            if (content == null)
                return;

            context.Metadata.CreateRouteValues = new RouteValueDictionary {
                {"Area", "Infusion.Documents"},
                {"Controller", "DocumentContentAdmin"},
                {"Action", "Create"},
                {"documentId", content.DocumentPart.Id}
            };
            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", "Infusion.Documents"},
                {"Controller", "DocumentContentAdmin"},
                {"Action", "Edit"},
                {"contentId", context.ContentItem.Id},
                {"documentId", content.DocumentPart.Id}
            };
            context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                {"Area", "Infusion.Documents"},
                {"Controller", "DocumentContentAdmin"},
                {"Action", "Delete"},
                {"postId", context.ContentItem.Id},
                {"blogId", content.DocumentPart.Id}
            };
        }
    }
}