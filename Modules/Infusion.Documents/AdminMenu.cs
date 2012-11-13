using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infusion.Documents.Services;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Infusion.Documents
{
    public class AdminMenu : INavigationProvider
    {
        private readonly IDocumentService _documentService;

        public AdminMenu(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("document")
                .Add(T("Document"), "1.5", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            var documents = _documentService.Get();
            var docCount = documents.Count();
            var singleBlog = docCount == 1 ? documents.ElementAt(0) : null;

            if (docCount > 0 && singleBlog == null)
            {
                menu.Add(T("Manage Documents"), "3",
                         item => item.Action("List", "DocumentAdmin", new { area = "Infusion.Documents" }).Permission(Permissions.MetaListDocuments));
            }
            else if (singleBlog != null)
                menu.Add(T("Manage Document"), "1.0",
                    item => item.Action("Item", "DocumentAdmin", new { area = "Infusion.Documents", documentId = singleBlog.Id }).Permission(Permissions.MetaListDocuments));

            if (singleBlog != null)
                menu.Add(T("New Content"), "1.1",
                         item =>
                         item.Action("Create", "DocumentContentAdmin", new { area = "Infusion.Documents", documentId = singleBlog.Id }).Permission(Permissions.MetaListDocuments));

            menu.Add(T("New Document"), "1.2",
                     item =>
                     item.Action("Create", "DocumentAdmin", new { area = "Infusion.Documents" }).Permission(Permissions.ManageDocuments));

        }
    }
}