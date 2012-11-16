using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace Infusion.Documents.Models
{
    public class DocumentContentPartRecord : ContentPartRecord
    {
        public virtual int SourceDocumentId { get; set; }
    }
}