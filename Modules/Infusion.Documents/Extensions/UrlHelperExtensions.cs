using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infusion.Documents.Models;
using Orchard.ContentManagement.Aspects;
using Orchard.Mvc.Extensions;

namespace Infusion.Documents.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string Documents(this UrlHelper urlHelper)
        {
            return urlHelper.Action("List", "Document", new { area = "Infusion.Documents" });
        }

        public static string DocumentsForAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.Action("List", "DocumentAdmin", new { area = "Infusion.Documents" });
        }

        public static string Document(this UrlHelper urlHelper, DocumentPart documentPart)
        {
            return urlHelper.Action("Item", "Document", new { documentId = documentPart.Id, area = "Infusion.Documents" });
        }

        public static string DocumentForAdmin(this UrlHelper urlHelper, DocumentPart documentPart)
        {
            return urlHelper.Action("Item", "DocumentAdmin", new { documentId = documentPart.Id, area = "Infusion.Documents" });
        }

        public static string DocumentCreate(this UrlHelper urlHelper)
        {
            return urlHelper.Action("Create", "DocumentAdmin", new { area = "Infusion.Documents" });
        }

        public static string DocumentEdit(this UrlHelper urlHelper, DocumentPart documentPart)
        {
            return urlHelper.Action("Edit", "DocumentAdmin", new { documentId = documentPart.Id, area = "Infusion.Documents" });
        }

        public static string DocumentRemove(this UrlHelper urlHelper, DocumentPart documentPart)
        {
            return urlHelper.Action("Remove", "DocumentAdmin", new { documentId = documentPart.Id, area = "Infusion.Documents" });
        }

        public static string DocumentContentCreate(this UrlHelper urlHelper, DocumentPart documentPart)
        {
            return urlHelper.Action("Create", "DocumentContentAdmin", new { documentId = documentPart.Id, area = "Infusion.Documents" });
        }

        public static string DocumentContentEdit(this UrlHelper urlHelper, DocumentContentPart documentContentPart)
        {
            return urlHelper.Action("Edit", "DocumentContentAdmin", new { documentId = documentContentPart.DocumentPart.Id, contentId = documentContentPart.Id, area = "Infusion.Documents" });
        }

        public static string DocumentContentDelete(this UrlHelper urlHelper, DocumentContentPart documentContentPart)
        {
            return urlHelper.Action("Delete", "DocumentContentAdmin", new { documentId = documentContentPart.DocumentPart.Id, contentId = documentContentPart.Id, area = "Infusion.Documents" });
        }

        public static string DocuemntContentPublish(this UrlHelper urlHelper, DocumentContentPart documentContentPart)
        {
            return urlHelper.Action("Publish", "DocumentContentAdmin", new { documentId = documentContentPart.DocumentPart.Id, contentId = documentContentPart.Id, area = "Infusion.Documents" });
        }

        public static string DocumentContentUnpublish(this UrlHelper urlHelper, DocumentContentPart documentContentPart)
        {
            return urlHelper.Action("Unpublish", "DocumentContentAdmin", new { documentId = documentContentPart.DocumentPart.Id, contentId = documentContentPart.Id, area = "Infusion.Documents" });
        }
    }
}