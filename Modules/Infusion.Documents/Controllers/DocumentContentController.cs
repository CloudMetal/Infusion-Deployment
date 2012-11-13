using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infusion.Documents.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.Themes;

namespace Infusion.Documents.Controllers
{
    [Themed]
    public class DocumentContentController : Controller
    {
        private readonly IDocumentContentService _documentContentService;
        private IOrchardServices _orchardServices;

        public DocumentContentController(IDocumentContentService documentContentService,
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory) {
            _documentContentService = documentContentService;
            _orchardServices = orchardServices;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
        }

        dynamic Shape { get; set; }
        protected ILogger Logger { get; set; }
    }
}