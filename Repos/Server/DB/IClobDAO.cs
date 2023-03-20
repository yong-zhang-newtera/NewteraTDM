/*
* @(#) IClobDAO.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.IO;
	using System.Data;

	using Newtera.Common.Attachment;

	/// <summary>
	/// A common interface for clob data access objects.
	/// </summary>
	/// <version> 	1.0.1	12 Jul 2003 </version>
	/// <author> 	Yong Zhang </author>
	public interface IClobDAO : IDisposable
	{
		/// <summary>
		/// Read from a clob column.
		/// </summary>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="rowId">The id identifies the row</param>
		/// <returns>A clob stream</returns>
		Stream ReadClob(string tableName, string columnName, string rowId);
		
		/// <summary>
		/// Read from a clob as text.
		/// </summary>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="rowId">The id identifies the row</param>
		/// <returns>A string</returns>
		string ReadClobAsText(string tableName, string columnName, string rowId);

		/// <summary>
		/// Read from a clob column.
		/// </summary>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param>
		/// <returns>A clob stream</returns>
		Stream ReadClob(string tableName, string columnName, string schemaName, string schemaVersion);

		/// <summary>
		/// Read from a clob column as text.
		/// </summary>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param>
		/// <returns>A string</returns>
		string ReadClobAsText(string tableName, string columnName, string schemaName, string schemaVersion);

		/// <summary>
		/// Read from a clob column as text.
		/// </summary>
		/// <param name="dataReader">The data reader</param>
		/// <param name="columnIndex">The zero-based column index.</param>
		/// <returns></returns>
		string ReadClobAsText(IDataReader dataReader, int columnIndex);

		/// <summary>
		/// Write to a clob column.
		/// </summary>
		/// <param name="stream">A stream providing data to write.</param>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param> 
		void WriteClob(Stream stream, string tableName, string columnName, string schemaName, string schemaVersion);

		/// <summary>
		/// Write to a clob column.
		/// </summary>
		/// <param name="xmlString">The xml string that is written to a clob.</param>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param> 
		void WriteClob(string xmlString, string tableName, string columnName, string schemaName, string schemaVersion);

        /// <summary>
        /// Write a text to a clob column.
        /// </summary>
        /// <param name="text">The text that is written to a clob.</param>
        /// <param name="tableName">The table name of clob column </param>
        /// <param name="columnName">The name of clob column</param>
        /// <param name="rowId">The row id</param>
        void WriteClob(string text, string tableName, string columnName, string rowId);

		/// <summary>
		/// Write to a clob column of an database instance indicated by table name,
		/// column name, and obj_id.
		/// </summary>
		/// <param name="cmd">An external provided database command</param>
		/// <param name="dataString">The data string to be written to a clob.</param>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="objId">The schema name</param>
		void WriteClob(IDbCommand cmd, string dataString, string tableName, string columnName, string objId);

        /// <summary>
        /// Read from a blob column.
        /// </summary>
        /// <param name="tableName">The table name of clob column </param>
        /// <param name="columnName">The name of clob column</param>
        /// <param name="idName">The name of id column</param>
        /// <param name="rowId">The id identifies the row</param>
        /// <returns>A byte array from the blob</returns>
        byte[] ReadBlob(string tableName, string columnName, string idName, string rowId);

        /// <summary>
        /// Write a stream to a blob column.
        /// </summary>
        /// <param name="tableName">The table name of clob column </param>
        /// <param name="columnName">The name of clob column</param>
        /// <param name="idName">The name of id column</param>
        /// <param name="rowId">The row id</param>
        /// <param name="data">The byte array to write to the blob</param>
        void WriteBlob(string tableName, string columnName, string idName, string rowId, byte[] data);

		/// <summary>
		/// Read an attachment Blob.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentTypeEnum values.</param>
		/// <param name="itemId">The id of the item that an attachment belongs to.</param>
		/// <param name="attachName">The unique name of an attachment</param>
		/// <returns>A blob stream</returns>
		Stream ReadAttachmentBlob(AttachmentType attachmentType, string itemId, string attachName);

		/// <summary>
		/// Read an attachment Blob into a provided stream.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentTypeEnum values.</param>
		/// <param name="stream">The provided stream</param>
		/// <param name="itemId">The id of the item that an attachment belongs to.</param>
		/// <param name="attachName">The unique name of an attachment</param>
		/// <returns>A blob stream</returns>
		void ReadAttachmentBlob(AttachmentType attachmentType, Stream stream, string itemId, string attachName);

		/// <summary>
		/// Read an attachment Blob.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentTypeEnum values.</param>
		/// <param name="attachmentId">The attachment id.</param>
		/// <returns>A blob stream</returns>
		Stream ReadAttachmentBlob(AttachmentType attachmentType, string attachmentId);

		/// <summary>
		/// Write an attachment as Blob to database.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentTypeEnum values.</param>
		/// <param name="stream">A stream providing attachment data to write.</param>
		/// <param name="attchmentId">The attachment id</param>
		void WriteAttachmentBlob(AttachmentType attachmentType, Stream stream, string attchmentId);

		/// <summary>
		/// Get xml representing a chart by chart id.
		/// </summary>
		/// <param name="chartId">The unique id that identifies a chart.</param>
		/// <returns>Xml of a chart data.</returns>
		string ReadChartXml(string chartId);

		/// <summary>
		/// Save the xml representimng chart data into the repository given the id.
		/// </summary>
		/// <param name="chartId">An unique id for the chart</param>
		/// <param name="xml">Xml for chart data.</param>
		void WriteChartXml(string chartId, string xml);

        /// <summary>
        /// Get xml representing a chart template by template id.
        /// </summary>
        /// <param name="templateId">The unique id that identifies a template.</param>
        /// <returns>Xml of a chart template data.</returns>
        string ReadChartTemplateXml(string templateId);

        /// <summary>
        /// Save the xml representimng chart template data into the repository given the id.
        /// </summary>
        /// <param name="templateId">An unique id for the chart template</param>
        /// <param name="xml">Xml for chart template data.</param>
        void WriteChartTemplateXml(string templateId, string xml);

        /// <summary>
        /// Get xml representing a pivot layout by id.
        /// </summary>
        /// <param name="pivotLayoutId">The unique id that identifies a pivot layout.</param>
        /// <returns>Xml of a pivot layout data.</returns>
        string ReadPivotLayoutXml(string pivotLayoutId);

        /// <summary>
        /// Save the xml representimng a pivot layout into the repository given the id.
        /// </summary>
        /// <param name="pivotLayoutId">An unique id for the pivot layout.</param>
        /// <param name="xml">Xml for a pivot layout.</param>
        void WritePivotLayoutXml(string pivotLayoutId, string xml);
	}
}