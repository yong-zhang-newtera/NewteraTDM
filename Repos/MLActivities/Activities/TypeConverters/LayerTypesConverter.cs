using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Newtera.MLActivities.Activities.TypeConverters
{
    public class LayerTypesConverter : StringConverter
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
            values.Add("Sigmoid");
            values.Add("Tanh");
            values.Add("RectifiedLinear");

            svc = new StandardValuesCollection(values);

            return svc;
        }
    }
}
