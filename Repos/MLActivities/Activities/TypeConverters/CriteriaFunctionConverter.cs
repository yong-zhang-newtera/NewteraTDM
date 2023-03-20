using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Newtera.MLActivities.Activities.TypeConverters
{
    public class CriteriaFunctionConverter : StringConverter
    {
        private static StandardValuesCollection svc;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ArrayList values = new ArrayList();
            values.Add("Logistic");
            values.Add("CrossEntropyWithSoftmax");
            values.Add("CrossEntropy");

            svc = new StandardValuesCollection(values);

            return svc;
        }
    }
}
