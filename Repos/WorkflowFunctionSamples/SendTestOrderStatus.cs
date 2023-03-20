using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SkyServiceStub;

using Newtera.Common.Core;
using Newtera.Common.Wrapper;

namespace WorkflowFunctionSamples
{
    /// <summary>
    /// 向天舟系统试验场景的状态为“已开始”
    /// </summary>
    public class SendTestOrderStatus : ICustomFunction
    {
        /// <summary>
        /// Execute an custom function
        /// </summary>
        /// <param name="instance">The instance of IInstanceWrapper 
        /// interface that represents the data instance bound to a
        /// workflow instance.</param>
        /// <returns>A string return from custome function</returns>
        public string Execute(IInstanceWrapper instance)
        {
            try
            {
                string tSISceneID = instance.GetString("SceneID"); // 试验项目的虚拟属性返回试验场景的ID
   
                SkyServiceProxy serviceProxy = new SkyServiceProxy();
                serviceProxy.SetServerIP();

                string rs = string.Empty;
                string tDMStatus = "已开始";//试验项目结束时请设置为“完成”
                string tDMUrl = "";
                rs = serviceProxy.SetTestStatus(tSISceneID, tDMStatus, tDMUrl);
                return rs;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine("SendTestOrderStatus has error:" + ex.Message);
                return "";
            }
        }

    }
}
