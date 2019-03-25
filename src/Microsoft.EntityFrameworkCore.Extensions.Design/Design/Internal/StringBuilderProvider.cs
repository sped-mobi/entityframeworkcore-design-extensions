using System.Text;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    public class StringBuilderProvider : IStringBuilderProvider
    {
        private StringBuilder _builder;

        public StringBuilder Builder
        {
            get
            {
                return _builder ?? (_builder = new StringBuilder());
            }
        }
    }
}
