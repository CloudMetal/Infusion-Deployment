using System.Collections.Generic;

namespace Infusion.Documents.ViewModels {
    public class SearchViewModel : Orchard.Search.ViewModels.SearchViewModel {
        
        public IList<dynamic> ElevatorPitches { get; set; }
        public IList<dynamic> PainPoints { get; set; }
        public IList<dynamic> Faq { get; set; }
        public IList<dynamic> Benefits { get; set; }
        public IList<dynamic> WhitePapers { get; set; }
        public IList<dynamic> CaseStudies { get; set; }
        public IList<dynamic> Presentations { get; set; }

        public SearchViewModel()
        {
            ElevatorPitches = new List<dynamic>();
            PainPoints = new List<dynamic>();
            Faq = new List<dynamic>();
            Benefits = new List<dynamic>();
            WhitePapers = new List<dynamic>();
            CaseStudies = new List<dynamic>();
            Presentations = new List<dynamic>();
        }
    }
}