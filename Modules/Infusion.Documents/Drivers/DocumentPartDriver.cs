using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infusion.Documents.Models;
using Infusion.Documents.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace Infusion.Documents.Drivers
{
    public class DocumentPartDriver : ContentPartDriver<DocumentPart>
    {
        private const string TemplateName = "Parts/Document";

        private readonly IDocumentService _documentService;
        private readonly IDocumentContentService _documentContentService;

        public DocumentPartDriver(IDocumentService documentService,
            IDocumentContentService documentContentService) {
            _documentService = documentService;
            _documentContentService = documentContentService;
        }

        protected override string Prefix
        {
            get { return "Document"; }
        }

        protected override DriverResult Display(
            DocumentPart part,
            string displayType,
            dynamic shapeHelper)
        {

            return Combined(
                ContentShape("Parts_Documents_Document_Manage",
                    () => shapeHelper.Parts_Documents_Document_Manage()),
                ContentShape("Parts_Documents_Document_Description",
                    () => shapeHelper.Parts_Documents_Document_Description(Description: part.Description)),
                ContentShape("Parts_Documents_Document_SummaryAdmin",
                    () => shapeHelper.Parts_Documents_Document_SummaryAdmin()),
                ContentShape("Parts_Documents_Document_ContentCount",
                    () => shapeHelper.Parts_Documents_Document_ContentCount(ContentCount: _documentContentService.ContentCount(part)))
                );
        }

        protected override DriverResult Editor(
            DocumentPart part,
            dynamic shapeHelper)
        {

            var shape = ContentShape("Parts_Document_Edit",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: TemplateName,
                        Model: part,
                        Prefix: Prefix));
            var results = new List<DriverResult> {
                shape
            };

            if (part.Id > 0)
                results.Add(ContentShape("Document_DeleteButton",
                    deleteButton => deleteButton));

            return Combined(results.ToArray());
        }

        protected override DriverResult Editor(
            DocumentPart part,
            IUpdateModel updater,
            dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }
    }
}