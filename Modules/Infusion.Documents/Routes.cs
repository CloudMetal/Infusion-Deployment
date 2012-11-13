using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Infusion.Documents
{
    public class Routes : IRouteProvider
    {

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Documents/Create",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "DocumentAdmin"},
                                                                                      {"action", "Create"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Documents/{documentId}/Edit",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "DocumentAdmin"},
                                                                                      {"action", "Edit"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Documents/{documentId}/Remove",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "DocumentAdmin"},
                                                                                      {"action", "Remove"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Documents/{documentId}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "DocumentAdmin"},
                                                                                      {"action", "Item"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Documents/{documentId}/DocumentContents/Create",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "DocumentContentAdmin"},
                                                                                      {"action", "Create"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Documents/{documentId}/DocumentContents/{contentId}/Edit",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "DocumentContentAdmin"},
                                                                                      {"action", "Edit"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Documents/{documentId}/DocumentContents/{contentId}/Delete",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "DocumentContentAdmin"},
                                                                                      {"action", "Delete"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Documents/{documentId}/DocumentContents/{contentId}/Publish",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "DocumentContentAdmin"},
                                                                                      {"action", "Publish"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Documents/{documentId}/DocumentContents/{contentId}/Unpublish",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "DocumentContentAdmin"},
                                                                                      {"action", "Unpublish"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Documents",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "DocumentAdmin"},
                                                                                      {"action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Documents",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"},
                                                                                      {"controller", "Document"},
                                                                                      {"action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Infusion.Documents"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }
    }
}