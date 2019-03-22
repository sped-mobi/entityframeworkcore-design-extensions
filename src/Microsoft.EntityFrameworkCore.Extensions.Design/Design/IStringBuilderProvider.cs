using System.Text;

namespace Microsoft.EntityFrameworkCore.Design
{
    public interface IStringBuilderProvider
    {
        StringBuilder Builder { get; }
    }
}