using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Newtera.Common.Core;
using Newtera.Common.Wrapper;

namespace Newtera.Test
{
    class CustomFunctionTest : ICustomFunction
    {
        public string Execute(IInstanceWrapper instance)
        {
            ErrorLog.Instance.WriteLine("CustomFunctionTest called");
            return "";
        }
    }
}
