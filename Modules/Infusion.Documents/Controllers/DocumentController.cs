using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infusion.Documents.Models;
using Infusion.Documents.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Navigation;

namespace Infusion.Documents.Controllers
{
    [Themed]
    public class DocumentController : Controller {
        private readonly IDocumentService _documentService;
        private readonly IDocumentContentService _documentContentService;
        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;

        public DocumentController(IDocumentService documentService,
            IOrchardServices orchardServices,
            ISiteService siteService,
            IDocumentContentService documentContentService,
            IShapeFactory shapeFactory) {
            _documentService = documentService;
            _orchardServices = orchardServices;
            _siteService = siteService;
            _documentContentService = documentContentService;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
        }

        dynamic Shape { get; set; }
        protected ILogger Logger { get; set; }

        public ActionResult List()
        {
            var documents = _documentService.Get().Select(b => _orchardServices.ContentManager.BuildDisplay(b, "Summary"));

            var list = Shape.List();
            list.AddRange(documents);

            dynamic viewModel = Shape.ViewModel()
                .ContentItems(list);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)viewModel);
        }

        public ActionResult Item(int documentId, PagerParameters pagerParameters)
        {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var documentPart = _documentService.Get(documentId, VersionOptions.Published).As<DocumentPart>();
            if (documentPart == null)
                return HttpNotFound();

            var documentContents = _documentContentService.Get(documentPart, pager.GetStartIndex(), pager.PageSize)
                .Select(b => _orchardServices.ContentManager.BuildDisplay(b));
            dynamic document = _orchardServices.ContentManager.BuildDisplay(documentPart);

            var list = Shape.List();
            list.AddRange(documentContents);
            document.Content.Add(Shape.Parts_Documents_DocumentContent_List(ContentItems: list), "5");

            var totalItemCount = _documentContentService.ContentCount(documentPart);
            document.Content.Add(Shape.Pager(pager).TotalItemCount(totalItemCount), "Content:after");

            return new ShapeResult(this, document);
        }
    }
}