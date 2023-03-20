using System;
using System.Data;
using System.Configuration;
using System.Web;

using Newtera.Common.Wrapper;
using Newtera.Common.MetaData.DataView;

namespace Newtera.WebForm
{
    /// <summary>
    /// Summary description for ICustomCommandEventHandler
    /// </summary>
    public interface ICustomCommandEventHandler
    {
        /// <summary>
        /// Gets or sets the InstanceWrapper that represents the instance that the custom command is associated with.
        /// </summary>
        IInstanceWrapper MasterInstance {get; set;}
        
        /// <summary>
        /// Gets or sets the database connection string.
        /// </summary>
        string ConnectionString { get; set;}

        /// <summary>
        /// Invoked when initializing an instance
        /// </summary>
        /// <param name="instance">The instance that represents a record</param>
        void OnInit(IInstanceWrapper instance);

        /// <summary>
        /// Invoked before the instance is inserted into database
        /// </summary>
        /// <param name="instance">The instance that represents a record</param>
        void BeforeInsert(IInstanceWrapper instance);

        /// <summary>
        /// Invoked before the instance is updated to database
        /// </summary>
        /// <param name="instance">The instance that represents a record</param>
        void BeforeUpdate(IInstanceWrapper instance);

        /// <summary>
        /// Invoked after a set of data instances related to a master instance have been imported 
        /// </summary>
        /// <param name="instance">The master instance</param>
        /// <param name="ds">The converted data set that have been imported to the database</param>
        void AfterImport(IInstanceWrapper instance, DataSet ds);
    }
}
