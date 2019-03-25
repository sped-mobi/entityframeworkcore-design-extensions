using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    public class EfDesignerOperationReporter : IOperationReporter
    {
        private IOperationReportHandler _handler;

        public EfDesignerOperationReporter(IOperationReportHandler handler)
        {
            _handler = handler;
        }

        public void WriteError(string message)
        {
            _handler.OnError(message);
        }

        public void WriteWarning(string message)
        {
            _handler.OnWarning(message);
        }

        public void WriteInformation(string message)
        {
            _handler.OnInformation(message);
        }

        public void WriteVerbose(string message)
        {
            _handler.OnVerbose(message);
        }
    }
}
