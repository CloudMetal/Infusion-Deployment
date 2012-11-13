using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Infusion.Documents
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageDocuments = new Permission { Description = "Manage documents for others", Name = "ManageDocuments" };
        public static readonly Permission ManageOwnDocuments = new Permission { Description = "Manage own documents", Name = "ManageOwnDocuments", ImpliedBy = new[] { ManageDocuments } };

        public static readonly Permission PublishDocumentContent = new Permission { Description = "Publish or unpublish document content for others", Name = "PublishBlogPost", ImpliedBy = new[] { ManageDocuments } };
        public static readonly Permission PublishOwnDocumentContent = new Permission { Description = "Publish or unpublish own document content", Name = "PublishOwnBlogPost", ImpliedBy = new[] { PublishDocumentContent, ManageOwnDocuments } };
        public static readonly Permission EditDocumentContent = new Permission { Description = "Edit document content for others", Name = "EditDocumentContent", ImpliedBy = new[] { PublishDocumentContent } };
        public static readonly Permission EditOwnDocumentContent = new Permission { Description = "Edit own document content", Name = "EditOwnDocumentContent", ImpliedBy = new[] { EditDocumentContent, PublishOwnDocumentContent } };
        public static readonly Permission DeleteDocumentContent = new Permission { Description = "Delete document content for others", Name = "DeleteDocumentContent", ImpliedBy = new[] { ManageDocuments } };
        public static readonly Permission DeleteOwnDocumentContent = new Permission { Description = "Delete own document content", Name = "DeleteOwnDocumentContent", ImpliedBy = new[] { DeleteDocumentContent, ManageOwnDocuments } };

        public static readonly Permission MetaListDocuments = new Permission { ImpliedBy = new[] { EditDocumentContent, PublishDocumentContent, DeleteDocumentContent } };
        public static readonly Permission MetaListOwnDocuments = new Permission { ImpliedBy = new[] { EditOwnDocumentContent, PublishOwnDocumentContent, DeleteOwnDocumentContent } };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ManageOwnDocuments,
                ManageDocuments,
                EditOwnDocumentContent,
                EditDocumentContent,
                PublishOwnDocumentContent,
                PublishDocumentContent,
                DeleteOwnDocumentContent,
                DeleteDocumentContent,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageDocuments}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {PublishDocumentContent,EditDocumentContent,DeleteDocumentContent}
                },
                new PermissionStereotype {
                    Name = "Moderator",
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] {ManageOwnDocuments}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new[] {EditOwnDocumentContent}
                },
            };
        }

    }
}