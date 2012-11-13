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
    public class DocumentContentPartDriver : ContentPartDriver<DocumentContentPart>
    {
        private const string TemplateName = "Parts/DocumentContent";

        private readonly IDocumentService _documentService;

        public DocumentContentPartDriver(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        protected override string Prefix
        {
            get { return "DocumentContent"; }
        }

        protected override DriverResult Display(
            DocumentContentPart part,
            string displayType,
            dynamic shapeHelper)
        {

            return ContentShape("Parts_DocumentContent",
                            () => shapeHelper.Parts_DocumentContent(
                                ContentPart: part));
        }

        protected override DriverResult Editor(
            DocumentContentPart part,
            dynamic shapeHelper)
        {

            return ContentShape("Parts_Document_Edit",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: TemplateName,
                        Model: part,
                        Prefix: Prefix));
        }

        protected override DriverResult Editor(
            DocumentContentPart part,
            IUpdateModel updater,
            dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }
    }
}