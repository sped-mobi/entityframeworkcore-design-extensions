using System.Text;

namespace Microsoft.EntityFrameworkCore.Design.Impl
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
