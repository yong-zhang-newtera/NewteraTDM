using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;
using Word = Microsoft.Office.Interop.Word;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;

namespace Newtera.SmartWordUtil
{
    public interface IDocDataSource
    {
        /// <summary>
        /// Gets or sets the meta data model of the data source.
        /// </summary>
        MetaDataModel MetaData {get; set;}

        /// <summary>
        /// Gets or sets name of selected base class in the data source.
        /// </summary>
        string BaseClassName { get; set;}

        /// <summary>
        /// Gets information indicating whether the given class is an inherited class to the base class
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        bool IsInheitedClass(string className);

        /// <summary>
        /// Get the meta data element represented by attribute values of the view node
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="elementType"></param>
        /// <param name="taxonomyName"></param>
        /// <returns>IMetaDataElement element</returns>
        IMetaDataElement GetViewNodeMetaDataElement(string elementName, string elementType, string taxonomyName);

        /// <summary>
        /// Gets the base class name that is associated with a family node
        /// </summary>
        /// <param name="familyNode">The family node</param>
        /// <returns>The base class name of family node</returns>
        string GetViewClassName(Word.XMLNode familyNode, out string baseClassCaption);
 
        /// <summary>
        /// Gets an InstanceView representing a data instance that is related to the data instances
        /// selected in the result grid.
        /// </summary>
        /// <param name="familyNode">The family node that contains the related view node</param>
        /// <param name="viewNode">The view node presenting a related view</param>
        /// <returns>An InstanceView representing a data instance</returns>
        InstanceView GetInstanceView(Word.XMLNode familyNode, Word.XMLNode viewNode);
 
        /// <summary>
        /// Gets an InstanceView representing the selected instance of the base class.
        /// </summary>
        /// <returns>An InstanceView representing the data instance for the specified row</returns>
        InstanceView GetInstanceView();
 
        /// <summary>
        /// Gets a DataView representing a view node that is related to the base class.
        /// </summary>
        /// <param name="familyNode">The family node that contains the related view node</param>
        /// <param name="viewNode">The view node presenting a related view</param>
        /// <returns>A DataViewModel object representing a view node that is related to the base class</returns>
        DataViewModel GetDataView(Word.XMLNode familyNode, Word.XMLNode viewNode);
    }
}