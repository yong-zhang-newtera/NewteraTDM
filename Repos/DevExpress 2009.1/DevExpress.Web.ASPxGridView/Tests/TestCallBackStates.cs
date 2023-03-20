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

#if DEBUGTEST
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Web.UI;
namespace DevExpress.Web.ASPxGridView.Tests {
	[System.ComponentModel.ToolboxItem(false)]
	public class CallbackControlTester : DevExpress.Web.ASPxGridView.ASPxGridView {
		public CallbackControlTester() {
			TrackViewState();
		}
		public int MyProperty {
			get { return (int)GetCallbackPropertyValue("MyProperty", 0); }
			set {
				SetCallbackPropertyValue("MyProperty", 0, value);
			}
		}
		string callbackSerializedText = string.Empty;
		public void SaveCallbackState() {
			callbackSerializedText = GetCallbackStateString();
		}
		public void LoadCallbackState() {
			SetCallbackStateString(callbackSerializedText);
		}
	}
	[TestFixture]
	public class CallBackStateTest {
		[Test]
		public void PropertyGetSetTest() {
			CallbackControlTester tester = new CallbackControlTester();
			tester.MyProperty = 1;
			Assert.AreEqual(1, tester.MyProperty);
		}
		[Test]
		public void SaveCallBackStateTest() {
			CallbackControlTester tester = new CallbackControlTester();
			tester.MyProperty = 1;
			tester.SaveCallbackState();
			tester.MyProperty = 2;
			Assert.AreEqual(2, tester.MyProperty);
			tester.LoadCallbackState();
			Assert.AreEqual(1, tester.MyProperty);
		}
		[Test]
		public void EmptyCallBackTest() {
			CallbackControlTester tester = new CallbackControlTester();
			tester.SaveCallbackState();
			tester.MyProperty = 2;
			Assert.AreEqual(2, tester.MyProperty);
			tester.LoadCallbackState();
			Assert.AreEqual(0, tester.MyProperty);
		}
	}
}
#endif
