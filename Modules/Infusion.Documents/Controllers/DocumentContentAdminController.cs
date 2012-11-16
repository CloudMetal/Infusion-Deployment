using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Infusion.Documents.Extensions;
using Infusion.Documents.Models;
using Infusion.Documents.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Contents.Settings;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc.AntiForgery;
using Orchard.Mvc.Extensions;
using Orchard.UI.Admin;
using Orchard.UI.Notify;

namespace Infusion.Documents.Controllers
{
    [ValidateInput(false), Admin]
    public class DocumentContentAdminController : Controller, IUpdateModel
    {
        private readonly IDocumentContentService _documentContentService;
        private readonly IDocumentService _documentService;

        public DocumentContentAdminController(IDocumentContentService documentContentService,
            IDocumentService documentService,
            IShapeFactory shapeFactory,
            IOrchardServices orchardServices) {
            Shape = shapeFactory;
            _documentContentService = documentContentService;
            _documentService = documentService;
            T = NullLocalizer.Instance;
            Services = orchardServices;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        public ActionResult Create(int documentId) {

            var document = _documentService.Get(documentId, VersionOptions.Latest).As<DocumentPart>();
            if (document == null)
                return HttpNotFound();

            var documentContent = Services.ContentManager.New<DocumentContentPart>("DocumentContent");
            documentContent.DocumentPart = document;

            if (!Services.Authorizer.Authorize(Permissions.EditDocumentContent, document, T("Not allowed to create document content")))
                return new HttpUnauthorizedResult();

            dynamic model = Services.ContentManager.BuildEditor(documentContent);
            
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Save")]
        public ActionResult CreatePOST(int documentId) {
            return CreatePOST(documentId, false);
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishPOST(int documentId) {
            if (!Services.Authorizer.Authorize(Permissions.PublishOwnDocumentContent, T("Couldn't create content")))
                return new HttpUnauthorizedResult();

            return CreatePOST(documentId, true);
        }

        private ActionResult CreatePOST(int documentId, bool publish = false) {
            var document = _documentService.Get(documentId, VersionOptions.Latest).As<DocumentPart>();

            if (document == null)
                return HttpNotFound();

            var documentContent = Services.ContentManager.New<DocumentContentPart>("DocumentContent");
            documentContent.DocumentPart = document;

            if (!Services.Authorizer.Authorize(Permissions.EditDocumentContent, document, T("Couldn't create content")))
                return new HttpUnauthorizedResult();
            
            Services.ContentManager.Create(documentContent, VersionOptions.Draft);
            var model = Services.ContentManager.UpdateEditor(documentContent, this);

            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            if (publish) {
                if (!Services.Authorizer.Authorize(Permissions.PublishDocumentContent, document.ContentItem, T("Couldn't publish document content")))
                    return new HttpUnauthorizedResult();

                Services.ContentManager.Publish(documentContent.ContentItem);
            }

            Services.Notifier.Information(T("Your {0} has been created.", documentContent.TypeDefinition.DisplayName));
            return Redirect(Url.DocumentContentEdit(documentContent));
        }

        //todo: the content shape template has extra bits that the core contents module does not (remove draft functionality)
        //todo: - move this extra functionality there or somewhere else that's appropriate?
        public ActionResult Edit(int documentId, int contentId) {
            var document = _documentService.Get(documentId, VersionOptions.Latest);
            if (document == null)
                return HttpNotFound();

            var post = _documentContentService.Get(contentId, VersionOptions.Latest);
            if (post == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.EditDocumentContent, post, T("Couldn't edit document content")))
                return new HttpUnauthorizedResult();

            dynamic model = Services.ContentManager.BuildEditor(post);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int documentId, int contentId, string returnUrl) {
            return EditPOST(documentId, contentId, returnUrl, contentItem => {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    Services.ContentManager.Publish(contentItem);
            });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public ActionResult EditAndPublishPOST(int documentId, int contentId, string returnUrl) {
            var document = _documentService.Get(documentId, VersionOptions.Latest);
            if (document == null)
                return HttpNotFound();

            // Get draft (create a new version if needed)
            var blogPost = _documentContentService.Get(contentId, VersionOptions.DraftRequired);
            if (blogPost == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.PublishDocumentContent, blogPost, T("Couldn't publish content")))
                return new HttpUnauthorizedResult();

            return EditPOST(documentId, contentId, returnUrl, contentItem => Services.ContentManager.Publish(contentItem));
        }

        public ActionResult EditPOST(int documentId, int contentId, string returnUrl, Action<ContentItem> conditionallyPublish) {
            var document = _documentService.Get(documentId, VersionOptions.Latest);
            if (document == null)
                return HttpNotFound();

            // Get draft (create a new version if needed)
            var documentContent = _documentContentService.Get(contentId, VersionOptions.DraftRequired);
            if (documentContent == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.EditDocumentContent, documentContent, T("Couldn't edit content")))
                return new HttpUnauthorizedResult();

            // Validate form input
            var model = Services.ContentManager.UpdateEditor(documentContent, this);
            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            conditionallyPublish(documentContent.ContentItem);

            Services.Notifier.Information(T("Your {0} has been saved.", documentContent.TypeDefinition.DisplayName));

            return this.RedirectLocal(returnUrl, Url.DocumentContentEdit(documentContent));
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult DiscardDraft(int id) {
            // get the current draft version
            var draft = Services.ContentManager.Get(id, VersionOptions.Draft);
            if (draft == null) {
                Services.Notifier.Information(T("There is no draft to discard."));
                return RedirectToEdit(id);
            }

            // check edit permission
            if (!Services.Authorizer.Authorize(Permissions.EditDocumentContent, draft, T("Couldn't discard document content draft")))
                return new HttpUnauthorizedResult();

            // locate the published revision to revert onto
            var published = Services.ContentManager.Get(id, VersionOptions.Published);
            if (published == null) {
                Services.Notifier.Information(T("Can not discard draft on unpublished document content."));
                return RedirectToEdit(draft);
            }

            // marking the previously published version as the latest
            // has the effect of discarding the draft but keeping the history
            draft.VersionRecord.Latest = false;
            published.VersionRecord.Latest = true;

            Services.Notifier.Information(T("Document content draft version discarded"));
            return RedirectToEdit(published);
        }

        ActionResult RedirectToEdit(int id) {
            return RedirectToEdit(Services.ContentManager.GetLatest<DocumentContentPart>(id));
        }

        ActionResult RedirectToEdit(IContent item) {
            if (item == null || item.As<DocumentContentPart>() == null)
                return HttpNotFound();
            return RedirectToAction("Edit", new { BlogId = item.As<DocumentContentPart>().DocumentPart.Id, PostId = item.ContentItem.Id });
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult Delete(int documentId, int contentId) {
            var document = _documentService.Get(documentId, VersionOptions.Latest);
            if (document == null)
                return HttpNotFound();

            var documentContent = _documentContentService.Get(contentId, VersionOptions.Latest);
            if (documentContent == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.DeleteDocumentContent, documentContent, T("Couldn't delete document content")))
                return new HttpUnauthorizedResult();

            _documentContentService.Delete(documentContent);
            Services.Notifier.Information(T("Document content was successfully deleted"));

            return Redirect(Url.DocumentForAdmin(document.As<DocumentPart>()));
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult Publish(int documentId, int contentId) {
            var document = _documentService.Get(documentId, VersionOptions.Latest);
            if (document == null)
                return HttpNotFound();

            var documentContent = _documentContentService.Get(contentId, VersionOptions.Latest);
            if (documentContent == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.PublishDocumentContent, documentContent, T("Couldn't publish document content")))
                return new HttpUnauthorizedResult();

            _documentContentService.Publish(documentContent);
            Services.Notifier.Information(T("Document content successfully published."));

            return Redirect(Url.DocumentForAdmin(document.As<DocumentPart>()));
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult Unpublish(int documentId, int contentId) {
            var document = _documentService.Get(documentId, VersionOptions.Latest);
            if (document == null)
                return HttpNotFound();

            var documentContent = _documentContentService.Get(contentId, VersionOptions.Latest);
            if (documentContent == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.PublishDocumentContent, documentContent, T("Couldn't unpublish document content")))
                return new HttpUnauthorizedResult();

            _documentContentService.Unpublish(documentContent);
            Services.Notifier.Information(T("Document content successfully unpublished."));

            return Redirect(Url.DocumentForAdmin(document.As<DocumentPart>()));
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }

    public class FormValueRequiredAttribute : ActionMethodSelectorAttribute {
        private readonly string _submitButtonName;

        public FormValueRequiredAttribute(string submitButtonName) {
            _submitButtonName = submitButtonName;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
            var value = controllerContext.HttpContext.Request.Form[_submitButtonName];
            return !string.IsNullOrEmpty(value);
        }
    }
}