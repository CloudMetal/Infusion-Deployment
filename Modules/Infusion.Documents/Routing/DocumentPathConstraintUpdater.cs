using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infusion.Documents.Services;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Environment;
using Orchard.Tasks;

namespace Infusion.Documents.Routing
{
    [UsedImplicitly]
    public class DocumentPathConstraintUpdater : IOrchardShellEvents, IBackgroundTask
    {
        private readonly IDocumentPathConstraint _documentPathConstraint;
        private readonly IDocumentService _documentService;

        public DocumentPathConstraintUpdater(IDocumentPathConstraint documentPathConstraint, 
            IDocumentService documentService)
        {
            _documentPathConstraint = documentPathConstraint;
            _documentService = documentService;
        }

        void IOrchardShellEvents.Activated()
        {
            Refresh();
        }

        void IOrchardShellEvents.Terminating()
        {
        }

        void IBackgroundTask.Sweep()
        {
            Refresh();
        }

        private void Refresh()
        {
            _documentPathConstraint.SetPaths(_documentService.Get().Select(b => b.As<IAliasAspect>().Path).ToList());
        }
    }
}