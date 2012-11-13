using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace Infusion.Documents.Models
{
    /*
     * Column<int>("DocumentPartRecord_Id")
                    .Column<int>("DocumentContentPartRecord_Id")
     */
    public class DocumentDocumentContentsRecord : ContentPartRecord
    {
        public virtual int DocumentPartRecord_Id { get; set; }
        public virtual int DocumentContentPartRecord_Id { get; set; }
    }
}