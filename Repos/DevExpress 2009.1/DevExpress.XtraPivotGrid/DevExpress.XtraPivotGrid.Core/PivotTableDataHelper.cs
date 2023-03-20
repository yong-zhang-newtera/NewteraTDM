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
using System.Collections;
using System.Data;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.Storage;
using System.IO;
using System.IO.Compression;
namespace DevExpress.Data.PivotGrid {
	public class PivotGridDataHelper : BaseDataControllerHelper {
		protected BaseDataControllerHelper fSource;
		protected DataStorageObjectComparer[] fStorage;
		protected int listRowCount;
		public PivotGridDataHelper(BaseDataControllerHelper source, DataControllerBase controller) : base(controller) {
			this.fSource = source;
			this.fStorage = null;
			this.listRowCount = 0;
		}
		public override void Dispose() {
			base.Dispose();
			ClearStorage();
			fStorage = null;
			fSource = null;
		}
		public BaseDataControllerHelper Source { get { return fSource; } }
		protected DataStorageObjectComparer[] Storage { get { return fStorage; } }
		protected PivotDataController PivotController { get { return Controller as PivotDataController; } }
		public override ArrayList RePopulateColumns() {
			ArrayList columns = Source.RePopulateColumns();
			return columns;
		}
		public override void PopulateColumns() { 
			Source.PopulateColumns();
			PivotController.ClientPopulateColumns();
		}
		public override int Count { get { return listRowCount; } }
		public override object GetRowValue(int listSourceRow, int column) {
			return Storage != null ? Storage[column].GetNullableRecordValue(listSourceRow) : null;
		}
		public override void SetRowValue(int listSourceRow, int column, object val) {
			if(Source == null)
				return;
			Source.SetRowValue(listSourceRow, column, val);
			Storage[column].SetRecordValue(listSourceRow, val);
		}
		public bool HasNullValue(int column) {
			return Storage != null ? Storage[column].HasNullValue() : false;
		}
		public bool SupportComparerCache(int column) {
			return Storage != null ? Storage[column].SupportComparerCache : false;
		}
		public bool HasComparerCache(int column) {
			return Storage != null ? Storage[column].HasComparerCache : false;
		}
		public void SetComparerCache(int column, int[] cache, bool isAscending) {
			if(Storage != null) {
				Storage[column].SetComparerCache(cache, isAscending);
			}
		}
		public virtual void RefreshData() {
			ClearStorage();
			CreateStorage();
		}
		public virtual void ClearStorage() {
			if(Storage == null) return;
			for(int i = 0; i < Storage.Length; i ++)
				if(Storage[i] != null)
					Storage[i].ClearStorage();
			this.fStorage = null;
			this.listRowCount = 0;
		}
		protected virtual void CreateStorage() {
			this.listRowCount = Source.Count;
			if(Count == 0) return;
			VisibleListSourceRowCollection visibleRowCollection = new VisibleListSourceRowCollection(Controller);
			visibleRowCollection.CreateList(Count);
			this.fStorage = new DataStorageObjectComparer[Source.Columns.Count];
			try {
				for(int i = 0; i < Source.Columns.Count; i++) {
					Storage[i] = CreateColumnStorage(visibleRowCollection, i);
				}
			} catch {
				ClearStorage();
			}
		}
		protected virtual DataStorageObjectComparer CreateColumnStorage(VisibleListSourceRowCollection visibleRowCollection, int column) {
			DataStorageObjectComparer columnStorage = Source.Columns[column].StorageComparer;
			columnStorage.CreateStorage(visibleRowCollection, Source, column);
			return columnStorage;
		}
		public void SaveToStream(Stream stream, bool compress) {
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(PivotDataController.DataStreamSign);
			writer.Write(compress ? 2 : 1);
			long startPosition = stream.Position;
			writer.Write(0L);
			if(compress) {
				using(DeflateStream compressStream = new DeflateStream(stream, CompressionMode.Compress, true)) 
					SaveToStreamCore(compressStream);
			} else SaveToStreamCore(stream);
			long endPosition = stream.Position;
			stream.Position = startPosition;
			writer.Write(endPosition);
			stream.Position = endPosition;
		}
		private void SaveToStreamCore(Stream stream) {
			BinaryWriter writer = new BinaryWriter(stream);
			int storageLength = Storage == null ? 0 : Storage.Length;
			writer.Write(storageLength);
			writer.Write(Count);
			for(int i = 0; i < storageLength; i++)
				Storage[i].SaveToStream(stream);
		}		
	}
}
