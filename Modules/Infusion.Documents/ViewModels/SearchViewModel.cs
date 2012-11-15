using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infusion.Documents.ViewModels
{
    public class SearchViewModel : Orchard.Search.ViewModels.SearchViewModel
    {

        public IList<dynamic> ElevatorPitches { get; set; }
        public IList<dynamic> PainPoints { get; set; }
        public IList<dynamic> Faq { get; set; }
        public IList<dynamic> Benefits { get; set; }
        public IList<dynamic> WhitePapers { get; set; }
        public IList<dynamic> CaseStudies { get; set; }
        public IList<dynamic> Presentations { get; set; }
        public IList<dynamic> RSS { get; set; }
        public IList<dynamic> Comments { get; set; }
    }
}