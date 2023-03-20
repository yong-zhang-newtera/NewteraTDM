#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       ASPxGridView                                 }
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
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxGridView.Helper {
	public class CallbackInfo {
		int callbackId;
		string eventArgument;
		public CallbackInfo(int callbackId, string eventArgument) {
			this.callbackId = callbackId;
			this.eventArgument = eventArgument;
		}
		public int CallbackId { get { return callbackId; } }
		public string EventArgument { get { return eventArgument; } }
	}
	public class CallbackArgumentsReader {
		const char PostPrefixChar = '|';
		const char PostPrefixLengthChar = ';';
		Dictionary<string, string> callbackArgs = new Dictionary<string, string>();
		public CallbackArgumentsReader(string arguments, string[] prefixes) {
			this.callbackArgs = new Dictionary<string, string>();
			ReadCallbacksArgs(arguments, prefixes);
		}
		public string this[string prefix] {
			get { return CallbackArgs.ContainsKey(prefix) ? CallbackArgs[prefix] : null;  }
		}
		public int GetIndexValue(string prefix) {
			string res = this[prefix];
			if(res == null) return -1;
			int num;
			return int.TryParse(res, out num) ? num : -1;
		}
		protected Dictionary<string, string> CallbackArgs { get { return callbackArgs; } }
		void ReadCallbacksArgs(string eventArguments, string[] prefixes) {
			while(ReadCallbackArgs(ref eventArguments, prefixes)) {
			}
		}
		protected bool ReadCallbackArgs(ref string eventArguments, string[] prefixes) {
			foreach(string prefix in prefixes) {
				string testPrefix = prefix + PostPrefixChar;
				if(eventArguments.StartsWith(testPrefix)) {
					int pos = eventArguments.IndexOf(PostPrefixLengthChar, testPrefix.Length);
					int length = -1;
					if(!int.TryParse(eventArguments.Substring(testPrefix.Length, pos - testPrefix.Length), out length)) {
						length = -1;
					}
					if(length > -1) {
						callbackArgs[prefix] = eventArguments.Substring(pos + 1, length);
						eventArguments = eventArguments.Substring(pos + length + 2);
						return true;
					}
				}
			}
			return false;
		}
		public static List<Unit> ReadUnitArray(string widthsString) {
			return ReadUnitArray(widthsString, '|');
		}
		public static List<Unit> ReadUnitArray(string widthsString, char dividedChar) {
			List<Unit> res = new List<Unit>();
			if(string.IsNullOrEmpty(widthsString)) return res;
			string[] widthList = widthsString.Split(dividedChar);
			for(int i = 0; i < widthList.Length; i++) {
				res.Add(ReadUnit(widthList[i]));
			}
			return res;
		}
		static Unit ReadUnit(string widthString) {
			if(string.IsNullOrEmpty(widthString)) return Unit.Empty;
			bool isPercent = widthString.IndexOf('%') > -1;
			string numeric = string.Empty;
			foreach(char ch in widthString) {
				if(char.IsDigit(ch)) numeric += ch;
			}
			int width = 0;
			if(!int.TryParse(numeric, out width)) return Unit.Empty;
			return new Unit(width, isPercent ? UnitType.Percentage : UnitType.Pixel);
		}
	}
	public class EditorValueInfo {
		int columnIndex;
		string value;
		public EditorValueInfo(int columnIndex, string value) {
			this.columnIndex = columnIndex;
			this.value = value;
		}
		public int ColumnIndex { get { return columnIndex; } }
		public string Value { get { return value; } }
	}
	public class GridViewCallBackEditorValuesReader {
		List<EditorValueInfo> list;
		string text;
		int pos;
		public GridViewCallBackEditorValuesReader(string text) {
			this.text = text;
			this.list = new List<EditorValueInfo>();
		}
		protected string Text { get { return text; } }
		protected int Pos { get { return pos; } set { pos = value; } }
		public List<EditorValueInfo> Values { get { return list; } }
		public void Proccess() {
			Pos = 0;
			int count = GetNumber(';');
			if(count <= 0) return;
			for(int i = 0; i < count; i++) {
				if(!ReadEditorValue()) return;
			}
		}
		bool ReadEditorValue() {
			int columnIndex = GetNumber();
			if(columnIndex < 0) return false;
			int length = GetNumber();
			if(length < 0) {
				list.Add(new EditorValueInfo(columnIndex, null));
				Pos++;
			}
			else {
				string value = Text.Substring(Pos, length);
				Pos += length + 1;
				list.Add(new EditorValueInfo(columnIndex, value));
			}
			return true;
		}
		protected int GetNumber() {
			return GetNumber(',');
		}
		protected int GetNumber(char separator) {
			int startPos = Pos;
			Pos = Text.IndexOf(separator, Pos);
			if(Pos < 0) return -1;
			int res = 0;
			if(!int.TryParse(Text.Substring(startPos, Pos - startPos), out res)) return -1;
			Pos ++;
			return res;
		}
	}
	public class GridCallbackArgumentsReader : CallbackArgumentsReader {
		public const string GridCallbackPrefix = "GB";
		public const string FunctionCallbackPrefix = "FB";
		public const string CallbackStatePrefix = "CB";
		public const string EditValuesPrefix = "EV";
		public const string SelectionRowsPrefix = "SR";
		public const string FocusedRowPrefix = "FR";
		public const string ColumnResizingPrefix = "CR";
		public GridCallbackArgumentsReader(string arguments)
			: base(arguments, new string[] {GridCallbackPrefix, FunctionCallbackPrefix,
			CallbackStatePrefix, EditValuesPrefix, SelectionRowsPrefix, FocusedRowPrefix, ColumnResizingPrefix}) {
		}
		public string CallbackArguments   { get { return this[GridCallbackArgumentsReader.GridCallbackPrefix]; } }
		public int InternalCallbackIndex { get { return GetIndexValue(GridCallbackArgumentsReader.FunctionCallbackPrefix); } }
		public string PageSelectionResult { get { return this[GridCallbackArgumentsReader.SelectionRowsPrefix]; } }
		public string ColumnResizingResult { get { return this[GridCallbackArgumentsReader.ColumnResizingPrefix]; } }
		public int FocusedRowIndex { get { return GetIndexValue(GridCallbackArgumentsReader.FocusedRowPrefix); } }
		public string CallbackState { get { return this[GridCallbackArgumentsReader.CallbackStatePrefix]; } }
		public string EditValues { get { return this[GridCallbackArgumentsReader.EditValuesPrefix]; } }
	}
}
