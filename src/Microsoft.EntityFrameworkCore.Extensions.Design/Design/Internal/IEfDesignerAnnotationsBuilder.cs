using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    public interface IEfDesignerAnnotationsBuilder
    {
        void RemoveAnnotation(ref List<IAnnotation> list, string annotationName);
        void BuildAnnotations(IEnumerable<IAnnotation> annotations, Action pushIndentAction, Action popIndentAction, Action<string> writeAction, Action<string> writeLineAction);
    }
}
