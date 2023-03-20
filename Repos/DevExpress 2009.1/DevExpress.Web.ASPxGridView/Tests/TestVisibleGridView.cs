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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Tests;
using System.Text.RegularExpressions;
namespace DevExpress.Web.ASPxGridView.Tests {
	[TestFixture]
	public class TestVisibleGridView : TestClassBase {
		[Test]
		public void ServerSide() {
			CheckRender(new ASPxGridView(), "table", true);
			CheckRender(new ASPxGridView(), "table", false);
		}
		public void CheckRender(ASPxGridView control, string rootTag, bool visible) {
			control.ID = "idControl";
			control.ClientVisible = visible;
			DoControlInit(control);
			DoControlPreRender(control);
			CheckVisibleRender(GetRenderResult(control), rootTag, control.ID, visible);
		}
		public void CheckVisibleRender(string contentHTML, string rootTag, string idControl, bool visible) {
			string expString = "<" + rootTag + ".*" + "id=\"" + idControl + "\".*" + "style=\"" + ".*" + "display:none" + ".*?>";
			Regex regExp = new Regex(expString);
			MatchCollection collection = regExp.Matches(contentHTML);
			if(visible)
				Assert.IsTrue(collection.Count == 0);
			else
				Assert.IsTrue(collection.Count == 1);
		}
	}
}
#endif
