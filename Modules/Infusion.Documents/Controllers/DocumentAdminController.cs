using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infusion.Documents.Extensions;
using Infusion.Documents.Models;
using Infusion.Documents.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace Infusion.Documents.Controllers
{
    [ValidateInput(false), Admin]
    public class DocumentAdminController : Controller, IUpdateModel {
        private readonly IDocumentService _documentService;
        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ISiteService _siteService;
        private readonly IDocumentContentService _documentContentService;
        public DocumentAdminController(IDocumentService documentService,
            IDocumentContentService documentContentService,
            IShapeFactory shapeFactory,
            IContentManager contentManager,
            ITransactionManager transactionManager,
            ISiteService siteService,
            IOrchardServices orchardServices) {
            Shape = shapeFactory;
            _documentService = documentService;
            _documentContentService = documentContentService;
            _contentManager = contentManager;
            _siteService = siteService;
            _transactionManager = transactionManager;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageDocuments, T("Not allowed to create documents")))
                return new HttpUnauthorizedResult();

            var document = Services.ContentManager.New<DocumentPart>("Document");
            if (document == null)
                return HttpNotFound();

            dynamic model = Services.ContentManager.BuildEditor(document);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageDocuments, T("Couldn't create document")))
                return new HttpUnauthorizedResult();

            var document = Services.ContentManager.New<DocumentPart>("Document");

            _contentManager.Create(document, VersionOptions.Draft);
            dynamic model = _contentManager.UpdateEditor(document, this);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _contentManager.Publish(document.ContentItem);
            return Redirect(Url.DocumentForAdmin(document));
        }

        public ActionResult Edit(int documentId)
        {
            var document = _documentService.Get(documentId, VersionOptions.Latest);

            if (!Services.Authorizer.Authorize(Permissions.ManageDocuments, document, T("Not allowed to edit blog")))
                return new HttpUnauthorizedResult();

            if (document == null)
                return HttpNotFound();

            dynamic model = Services.ContentManager.BuildEditor(document);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Delete")]
        public ActionResult EditDeletePOST(int documentId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageDocuments, T("Couldn't delete document")))
                return new HttpUnauthorizedResult();

            var document = _documentService.Get(documentId, VersionOptions.DraftRequired);
            if (document == null)
                return HttpNotFound();
            _documentService.Delete(document);

            Services.Notifier.Information(T("Document deleted"));

            return Redirect(Url.DocumentsForAdmin());
        }


        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int documentId)
        {
            var blog = _documentService.Get(documentId, VersionOptions.DraftRequired);

            if (!Services.Authorizer.Authorize(Permissions.ManageDocuments, blog, T("Couldn't edit document")))
                return new HttpUnauthorizedResult();

            if (blog == null)
                return HttpNotFound();

            dynamic model = Services.ContentManager.UpdateEditor(blog, this);
            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _contentManager.Publish(blog);
            Services.Notifier.Information(T("Document information updated"));

            return Redirect(Url.DocumentsForAdmin());
        }

        [HttpPost]
        public ActionResult Remove(int documentId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageDocuments, T("Couldn't delete document")))
                return new HttpUnauthorizedResult();

            var blog = _documentService.Get(documentId, VersionOptions.Latest);

            if (blog == null)
                return HttpNotFound();

            _documentService.Delete(blog);

            Services.Notifier.Information(T("Document was successfully deleted"));
            return Redirect(Url.DocumentsForAdmin());
        }

        public ActionResult List()
        {
            var list = Services.New.List();
            list.AddRange(_documentService.Get(VersionOptions.Latest)
                              .Select(b =>
                              {
                                  var document = Services.ContentManager.BuildDisplay(b, "SummaryAdmin");
                                  document.TotalPostCount = _documentContentService.ContentCount(b, VersionOptions.Latest);
                                  return document;
                              }));

            dynamic viewModel = Services.New.ViewModel()
                .ContentItems(list);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)viewModel);
        }

        public ActionResult Item(int documentId, PagerParameters pagerParameters)
        {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            var documentPart = _documentService.Get(documentId, VersionOptions.Latest).As<DocumentPart>();

            if (documentPart == null)
                return HttpNotFound();

            var docContents = _documentContentService.Get(documentPart, pager.GetStartIndex(), pager.PageSize, VersionOptions.Latest).ToArray();
            var contentShapes = docContents.Select(bp => _contentManager.BuildDisplay(bp, "SummaryAdmin")).ToArray();

            dynamic document = Services.ContentManager.BuildDisplay(documentPart, "DetailAdmin");

            var list = Shape.List();
            list.AddRange(contentShapes);
            document.Content.Add(Shape.Parts_Documents_DocumentContent_ListAdmin(ContentItems: list), "5");

            var totalItemCount = _documentContentService.ContentCount(documentPart, VersionOptions.Latest);
            document.Content.Add(Shape.Pager(pager).TotalItemCount(totalItemCount), "Content:after");

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)document);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}