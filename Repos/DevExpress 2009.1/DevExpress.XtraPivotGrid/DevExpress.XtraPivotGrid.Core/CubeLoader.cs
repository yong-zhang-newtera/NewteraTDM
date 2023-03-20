#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                        }
{                                                                   }
{       Copyright (c) 2000-2009 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2009 Developer Express Inc.

using System;
using System.Globalization;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.IO;
using DevExpress.Data.Helpers;
using DevExpress.Data.Storage;
using DevExpress.Data;
using System.IO.Compression;
using System.Collections.Generic;
namespace DevExpress.Data.PivotGrid {
	public class PivotFileDataSourceHelper {
		public static void SeekToData(Stream stream) {
			stream.Position = 0;
			BinaryReader reader = new BinaryReader(stream);
			reader.ReadString();
			reader.ReadInt32();
			stream.Position = reader.ReadInt64();
		}
		public static void SeekToFields(Stream stream) {
			stream.Position = 0;
			BinaryReader reader = new BinaryReader(stream);
			reader.ReadString();
			reader.ReadInt32();
			stream.Position = reader.ReadInt64();
			reader.ReadString();
			reader.ReadInt32();
			stream.Position = reader.ReadInt64();
		}
	}
	internal class PivotFilePropertyDescriptor : PropertyDescriptor {
		readonly Type dataType;
		readonly PivotFileDataSource dataSource;
		readonly int index;
		public PivotFilePropertyDescriptor(PivotFileDataSource dataSource, int index, string name, Type dataType, string displayName)
			: base(name, new Attribute[1] { new DisplayNameAttribute(displayName) }) {
			this.dataSource = dataSource;
			this.index = index;
			this.dataType = dataType;
		}
		public override bool CanResetValue(object component) { return false; }
		public override Type ComponentType { get { return typeof(DataColumn); } }
		public override object GetValue(object component) {
			return dataSource.GetRowValue((int)component, index);
		}
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType { get { return dataType; } }
		public override void ResetValue(object component) { throw new Exception("The operation is not supported."); }
		public override void SetValue(object component, object value) { throw new Exception("The peration is not supported."); }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	public class PivotFileDataSource : ITypedList, IList {
		PropertyDescriptorCollection itemProperties;
		string fileName;
		Stream stream;
		DataStorageObjectComparer[] storage;
		int rowCount;
		protected PropertyDescriptorCollection ItemProperties { get { return itemProperties; } }
		protected DataStorageObjectComparer[] Storage { get { return storage; } }
		protected int RowCount { get { return rowCount; } }
		public string FileName { get { return fileName; } set { fileName = value; } }
		public Stream Stream { get { return stream; } }
		public bool CreatedFromStream { get { return stream != null; } }
		public PivotFileDataSource() { }
		public PivotFileDataSource(string fileName) {
			this.fileName = fileName;
			using(FileStream stream = new FileStream(FileName, FileMode.Open, FileAccess.Read)) {
				LoadItemProperties(stream);
				LoadStorageColumns(stream);
			}
		}
		public PivotFileDataSource(Stream stream) {
			this.stream = stream;
			LoadItemProperties(stream);
			LoadStorageColumns(stream);
		}
		void LoadItemProperties(Stream stream) {
			stream.Position = 0;
			BinaryReader reader = new BinaryReader(stream);
			string sign = reader.ReadString();
			if(sign != PivotDataController.StreamSign) throw new Exception("Corrupted data file!");
			int version = reader.ReadInt32();
			reader.ReadInt64(); 
			if(version == 1) {
				int colCount = reader.ReadInt32();
				PropertyDescriptor[] descriptors = new PropertyDescriptor[colCount];
				for(int i = 0; i < colCount; i++) {
					string name = reader.ReadString(),
						displayName = reader.ReadString(),
						typeName = reader.ReadString();
					descriptors[i] = new PivotFilePropertyDescriptor(this, i, name, Type.GetType(typeName, true), displayName);
				}
				itemProperties = new PropertyDescriptorCollection(descriptors);
			} else {
				throw new Exception("Unknown data file version!");
			}
		}
		void LoadStorageColumns(Stream stream) {
			PivotFileDataSourceHelper.SeekToData(stream);
			BinaryReader reader = new BinaryReader(stream);
			string sign = reader.ReadString();
			if(sign != PivotDataController.DataStreamSign) throw new Exception("Data corrupted!");
			int version = reader.ReadInt32();
			reader.ReadInt64();
			switch(version) {
				case 2:
					using(DeflateStream decompressStream = new DeflateStream(stream, CompressionMode.Decompress, true))
						LoadStorageColumnsCore(decompressStream);
					break;
				case 1:
					LoadStorageColumnsCore(stream);
					break;
			}
		}
		void LoadStorageColumnsCore(Stream stream) {
			BinaryReader reader = new BinaryReader(stream);
			int storageCount = Math.Min(reader.ReadInt32(), ItemProperties.Count); ;
			rowCount = reader.ReadInt32();
			storage = new DataStorageObjectComparer[storageCount];
			for(int i = 0; i < storageCount; i++) {
				storage[i] = DataStorageObjectComparer.CreateComparer(ItemProperties[i].PropertyType);
				storage[i].LoadFromStream(stream);
			}
		}
		#region ITypedList Members
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return ItemProperties;
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return "File Cube";
		}
		internal object GetRowValue(int rowIndex, int columnIndex) {
			if(rowIndex >= 0 && rowIndex < RowCount && columnIndex >= 0 && columnIndex < Storage.Length)
				return Storage[columnIndex].GetNullableRecordValue(rowIndex);
			return null;
		}
		#endregion
		#region IList Members
		public int Add(object value) {
			throw new Exception("The method is not supported.");
		}
		public void Clear() {
			throw new Exception("The method is not supported.");
		}
		public bool Contains(object value) {
			if(!(value is int)) return false;
			int index = (int)value;
			return index >= 0 && index < RowCount;
		}
		public int IndexOf(object value) {
			return value is int ? (int)value : -1;
		}
		public void Insert(int index, object value) {
			throw new Exception("The method is not supported.");
		}
		public bool IsFixedSize { get { return true; } }
		public bool IsReadOnly { get { return true; } }
		public void Remove(object value) {
			throw new Exception("The method is not supported.");
		}
		public void RemoveAt(int index) {
			throw new Exception("The method is not supported.");
		}
		public object this[int index] {
			get { return index; }
			set { throw new Exception("The method is not supported."); }
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			throw new Exception("The method is not supported.");
		}
		public int Count {
			get { return RowCount; }
		}
		public bool IsSynchronized { get { return false; } }
		public object SyncRoot { get { return null; } }
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			return new FileEnumerator(this);
		}
		#endregion
		class FileEnumerator : IEnumerator {
			readonly PivotFileDataSource dataSource;
			int index;
			public FileEnumerator(PivotFileDataSource dataSource) {
				this.dataSource = dataSource;
				this.index = -1;
			}
			#region IEnumerator Members
			public object Current { get { return index; } }
			public bool MoveNext() {
				return ++index < dataSource.Count;
			}
			public void Reset() {
				index = -1;
			}
			#endregion
		}
	}
}
