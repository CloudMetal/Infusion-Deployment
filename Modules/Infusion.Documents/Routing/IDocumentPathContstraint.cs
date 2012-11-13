using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Orchard;

namespace Infusion.Documents.Routing
{
    public interface IDocumentPathConstraint : IRouteConstraint, ISingletonDependency
    {
        void SetPaths(IEnumerable<string> paths);
        string FindPath(string path);
        void AddPath(string path);
        void RemovePath(string path);
    }
}
