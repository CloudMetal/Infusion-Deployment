using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Contrib.Taxonomies.Fields;
using Infusion.Documents.Models;
using Infusion.Documents.Services;
using Orchard;
using Orchard.Collections;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.Indexing;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Search.Models;
using Orchard.Search.Services;
using Orchard.Search.ViewModels;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using SearchViewModel = Infusion.Documents.ViewModels.SearchViewModel;

namespace Infusion.Documents.Controllers
{
    [Themed]
    public class DocumentController : Controller {
        private readonly IDocumentService _documentService;
        private readonly IDocumentContentService _documentContentService;
        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        private readonly ISearchService _searchService;
        private readonly IContentManager _contentManager;

        public DocumentController(IDocumentService documentService,
            IOrchardServices orchardServices,
            ISiteService siteService,
            IDocumentContentService documentContentService,
            IShapeFactory shapeFactory, ISearchService searchService) {
            _documentService = documentService;
            _orchardServices = orchardServices;
            _siteService = siteService;
            _documentContentService = documentContentService;
            Shape = shapeFactory;
            _searchService = searchService;
            _contentManager = orchardServices.ContentManager;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
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

        public ActionResult Find(PagerParameters pagerParameters, string q = "") {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var searchFields = _orchardServices.WorkContext.CurrentSite.As<SearchSettingsPart>().SearchedFields;

            IPageOfItems<ISearchHit> searchHits = new PageOfItems<ISearchHit>(new ISearchHit[] { });
            try {

                searchHits = _searchService.Query(q, pager.Page, pager.PageSize,
                                                  _orchardServices.WorkContext.CurrentSite.As<SearchSettingsPart>().Record.FilterCulture,
                                                  searchFields,
                                                  searchHit => searchHit);
            }
            catch (Exception exception) {
                Logger.Error(T("Invalid search query: {0}", exception.Message).Text);
                _orchardServices.Notifier.Error(T("Invalid search query: {0}", exception.Message));
            }

            var list = Shape.List();
            var foundIds = searchHits.Select(searchHit => searchHit.ContentItemId).ToList();

            // ignore search results which content item has been removed or unpublished
            var foundItems = _contentManager.GetMany<IContent>(foundIds, VersionOptions.Published, new QueryHints()).Where(x => x.ContentItem.ContentType == "DocumentContent").Select(x => x.As<DocumentContentPart>()).ToList();

            foreach (var contentItem in foundItems) {
                list.Add(_contentManager.BuildDisplay(contentItem, "Summary"));
            }
            searchHits.TotalItemCount -= foundIds.Count() - foundItems.Count();


            var pagerShape = Shape.Pager(pager).TotalItemCount(searchHits.TotalItemCount);

            var searchViewModel = new SearchViewModel {
                Query = q,
                TotalItemCount = searchHits.TotalItemCount,
                StartPosition = (pager.Page - 1) * pager.PageSize + 1,
                EndPosition = pager.Page * pager.PageSize > searchHits.TotalItemCount ? searchHits.TotalItemCount : pager.Page * pager.PageSize,
                ContentItems = list,
                Pager = pagerShape
            };

            var firstItem = foundItems.FirstOrDefault();

            if (firstItem != null)
            {
                var document = firstItem.DocumentPart;
                var documentContents = _documentContentService.Get(document).ToList();

                searchViewModel.ElevatorPitches = BuildDocumentContentDisplay(documentContents, "ElevatorPitch");
                searchViewModel.PainPoints = BuildDocumentContentDisplay(documentContents, "PainPoints"); ;
                searchViewModel.Faq = BuildDocumentContentDisplay(documentContents, "FAQ"); ;
                searchViewModel.Benefits = BuildDocumentContentDisplay(documentContents, "Benefits"); ;
                searchViewModel.WhitePapers = BuildDocumentContentDisplay(documentContents, "WhitePaper"); ;
                searchViewModel.CaseStudies = BuildDocumentContentDisplay(documentContents, "CaseStudy"); ;
                searchViewModel.Presentations = BuildDocumentContentDisplay(documentContents, "Presentation"); ;
            }

            return View(searchViewModel);
        }

        private static IEnumerable<DocumentContentPart> FilterDocumentContents(IEnumerable<DocumentContentPart> documentContents, string termName) {

            foreach (var documentContent in documentContents) {
                dynamic content = documentContent;
                var documentTypeField = (TaxonomyField) ((dynamic) content).DocumentType;

                if (documentTypeField.Terms.Value.Any(x => x.Name == termName))
                    yield return documentContent;
            }

            //return from documentContent in documentContents 
            //       let content = documentContent 
            //       let documentTypeField = (TaxonomyField)((dynamic) content).DocumentType 
            //       where documentTypeField.Terms.Value.Any(x => x.Name == termName) 
            //       select documentContent;
        }

        private IList<dynamic> BuildDocumentContentDisplay(IEnumerable<DocumentContentPart> documentContents, string termName) {
            return FilterDocumentContents(documentContents, termName).Select(x => _contentManager.BuildDisplay(x, "Summary")).ToList();
        }
    }
}