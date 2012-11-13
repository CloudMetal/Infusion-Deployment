using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infusion.Documents.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Infusion.Documents.Services
{
    public interface IDocumentService : IDependency
    {
        DocumentPart Get(string path);
        ContentItem Get(int id, VersionOptions versionOptions);
        IEnumerable<DocumentPart> Get();
        IEnumerable<DocumentPart> Get(VersionOptions versionOptions);
        void Delete(ContentItem blog);
    }
}
