using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace Infusion.Documents.Models
{
    public class DocumentPart : ContentPart<DocumentPartRecord>
    {
        public string Name
        {
            get { return this.As<ITitleAspect>().Title; }
        }

        public string Description {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public IEnumerable<DocumentContentPart> ContentParts { get; set; } 
    }
}