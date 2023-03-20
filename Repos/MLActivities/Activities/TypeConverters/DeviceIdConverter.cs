using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Newtera.MLActivities.Activities.TypeConverters
{
    public class DeviceIdConverter : StringConverter
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
            values.Add("CPU");
            values.Add("GPU");
            values.Add("Auto");

            svc = new StandardValuesCollection(values);

            return svc;
        }
    }
}
