using System.Text;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    public interface IStringBuilderProvider
    {
        StringBuilder Builder { get; }
    }
}