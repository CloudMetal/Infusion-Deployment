﻿using System;
using System.Web.Mvc;
using Contrib.Taxonomies.Helpers;
using Contrib.Taxonomies.Services;
using Orchard.DisplayManagement;
using Orchard.Events;
using Orchard.Localization;

namespace Contrib.Taxonomies.Projections {
    public interface IFormProvider : IEventHandler {
        void Describe(dynamic context);
    }

    public class TermsFilterForms : IFormProvider {
        private readonly ITaxonomyService _taxonomyService;
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public TermsFilterForms(
            IShapeFactory shapeFactory,
            ITaxonomyService taxonomyService) {
            _taxonomyService = taxonomyService;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(dynamic context) {
            Func<IShapeFactory, object> form =
                shape => {

                    var f = Shape.Form(
                        Id: "SelectTerms",
                        _Terms: Shape.SelectList(
                            Id: "termids", Name: "TermIds",
                            Title: T("Terms"),
                            Description: T("Select some terms."),
                            Size: 10,
                            Multiple: true
                            ),
                        _Exclusion: Shape.FieldSet(
                            _OperatorOneOf: Shape.Radio(
                                Id: "operator-is-one-of", Name: "Operator",
                                Title: T("Is one of"), Value: "0", Checked: true
                                ),
                            _OperatorIsAllOf: Shape.Radio(
                                Id: "operator-is-all-of", Name: "Operator",
                                Title: T("Is all of"), Value: "1"
                                )
                            )
                        );

                    foreach (var taxonomy in _taxonomyService.GetTaxonomies()) {
                        f._Terms.Add(new SelectListItem { Value = String.Empty, Text = taxonomy.Name });
                        foreach (var term in _taxonomyService.GetTerms(taxonomy.Id)) {
                            var gap = new string('-', term.GetLevels());
                            
                            if(gap.Length > 0) {
                                gap += " ";
                            }

                            f._Terms.Add(new SelectListItem { Value = term.Id.ToString(), Text = gap + term.Name });
                        }
                    }

                    return f;
                };

            context.Form("SelectTerms", form);

        }
    }
}