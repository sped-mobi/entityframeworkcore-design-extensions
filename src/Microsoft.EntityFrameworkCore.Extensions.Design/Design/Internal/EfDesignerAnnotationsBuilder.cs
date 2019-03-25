// -----------------------------------------------------------------------
// <copyright file="AnnotationsBuilder.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    public class EfDesignerAnnotationsBuilder : IEfDesignerAnnotationsBuilder
    {
        private readonly ICSharpHelper _helper;

        public EfDesignerAnnotationsBuilder(EfDesignerHelper helper)
        {
            _helper = helper;
        }

        public static void Remove(ref List<IAnnotation> annotations, string annotationName)
        {
            annotations.Remove(annotations.SingleOrDefault(a => a.Name == annotationName));
        }

        public void RemoveAnnotation(ref List<IAnnotation> list, string annotationName)
        {
            list.Remove(list.SingleOrDefault(a => a.Name == annotationName));
        }

        public void BuildAnnotations(
            IEnumerable<IAnnotation> annotations,
            Action pushIndentAction,
            Action popIndentAction,
            Action<string> writeAction,
            Action<string> writeLineAction)
        {
            if (annotations.Count() == 1)
            {
                return;
            }

            if (annotations.Count() == 2)
            {
            }

            List<string> list = annotations.Select(GenerateAnnotation).ToList<string>();
            pushIndentAction();
            for (int i = 0; i < list.Count; i++)
            {
                string line = list[i];

                if (i == list.Count - 1)
                {
                    writeLineAction(line);
                }
                else
                {
                    writeAction(line);
                }
            }
            popIndentAction();
        }

        private string GenerateAnnotation(IAnnotation annotation)
        {
            return ".HasAnnotation(" + _helper.Literal(annotation.Name) + ", " + _helper.UnknownLiteral(annotation.Value) + ")";
        }

    }
}
