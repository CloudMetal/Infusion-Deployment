using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace Infusion.Documents.Models
{
    public class DocumentPartRecord : ContentPartRecord
    {
        public virtual string Description { get; set; }
    }
}