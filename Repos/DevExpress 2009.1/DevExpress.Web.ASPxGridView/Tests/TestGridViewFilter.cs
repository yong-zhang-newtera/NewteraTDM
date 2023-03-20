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
using DevExpress.Web.ASPxGridView;
using DevExpress.Data;
using DevExpress.Web.Data;
using System.Reflection;
using DevExpress.Web.ASPxGridView.Rendering;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxClasses.Tests;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using DevExpress.Web.ASPxGridView.Helper;
using DevExpress.Data.Filtering;
namespace DevExpress.Web.ASPxGridView.Tests {
	public class BaseFilterHelperTester : BaseFilterHelper {
		public new string GetColumnAutoFilterText(AutoFilterCondition condition, string fieldName, CriteriaOperator currentFilter) {
			return base.GetColumnAutoFilterText(condition, fieldName, currentFilter);
		}
	}
	[TestFixture]
	public class TestGridViewFilter {
		[Test]
		public void CreateAutofilterLikeTests() {
			BaseFilterHelperTester filter = new BaseFilterHelperTester();
			Assert.AreEqual("%", filter.CreateAutofilterLike(AutoFilterCondition.EndsWith, ""));
			Assert.AreEqual("%", filter.CreateAutofilterLike(AutoFilterCondition.EndsWith, "%"));
			Assert.AreEqual("%%test", filter.CreateAutofilterLike(AutoFilterCondition.EndsWith, "*test"));
			Assert.AreEqual("%test", filter.CreateAutofilterLike(AutoFilterCondition.EndsWith, "test"));
			Assert.AreEqual("%_test", filter.CreateAutofilterLike(AutoFilterCondition.EndsWith, "?test"));
			Assert.AreEqual("%", filter.CreateAutofilterLike(AutoFilterCondition.BeginsWith, ""));
			Assert.AreEqual("%", filter.CreateAutofilterLike(AutoFilterCondition.BeginsWith, "%"));
			Assert.AreEqual("test%%", filter.CreateAutofilterLike(AutoFilterCondition.BeginsWith, "test*"));
			Assert.AreEqual("test%%", filter.CreateAutofilterLike(AutoFilterCondition.BeginsWith, "test%"));
			Assert.AreEqual("test%", filter.CreateAutofilterLike(AutoFilterCondition.BeginsWith, "test"));
			Assert.AreEqual("test_%", filter.CreateAutofilterLike(AutoFilterCondition.BeginsWith, "test_"));
			Assert.AreEqual("%test%%", filter.CreateAutofilterLike(AutoFilterCondition.Contains, "test*"));
			Assert.AreEqual("%test%%", filter.CreateAutofilterLike(AutoFilterCondition.Contains, "test%"));
			Assert.AreEqual("%test%", filter.CreateAutofilterLike(AutoFilterCondition.Contains, "test"));
			Assert.AreEqual("%test_%", filter.CreateAutofilterLike(AutoFilterCondition.Contains, "test_"));
			Assert.AreEqual("%", filter.CreateAutofilterLike(AutoFilterCondition.Contains, ""));
			Assert.AreEqual("%", filter.CreateAutofilterLike(AutoFilterCondition.Contains, "%"));
			Assert.AreEqual("%a%", filter.CreateAutofilterLike(AutoFilterCondition.Contains, "a"));
			Assert.AreEqual("%a%%", filter.CreateAutofilterLike(AutoFilterCondition.Contains, "a%"));
			Assert.AreEqual("%%a%", filter.CreateAutofilterLike(AutoFilterCondition.Contains, "%a"));
			Assert.AreEqual("%_%", filter.CreateAutofilterLike(AutoFilterCondition.Contains, "?"));
			Assert.AreEqual("a%", filter.CreateAutofilterLike(AutoFilterCondition.BeginsWith, "a"));
			Assert.AreEqual("_%", filter.CreateAutofilterLike(AutoFilterCondition.BeginsWith, "?"));
			Assert.AreEqual("%a", filter.CreateAutofilterLike(AutoFilterCondition.EndsWith, "a"));
			Assert.AreEqual("%_", filter.CreateAutofilterLike(AutoFilterCondition.EndsWith, "?"));
		}
		[Test]
		public void ExtractLikeStringTests() {
			BaseFilterHelperTester filter = new BaseFilterHelperTester();
			Assert.AreEqual("test", filter.ExtractLikeString(AutoFilterCondition.BeginsWith, "test%"));
			Assert.AreEqual("test_", filter.ExtractLikeString(AutoFilterCondition.BeginsWith, "test_%"));
			Assert.AreEqual("%test_", filter.ExtractLikeString(AutoFilterCondition.BeginsWith, "%test_%"));
			Assert.AreEqual("_test_", filter.ExtractLikeString(AutoFilterCondition.BeginsWith, "_test_%"));
			Assert.AreEqual("test", filter.ExtractLikeString(AutoFilterCondition.EndsWith, "%test"));
			Assert.AreEqual("test_", filter.ExtractLikeString(AutoFilterCondition.EndsWith, "%test_"));
			Assert.AreEqual("test%", filter.ExtractLikeString(AutoFilterCondition.EndsWith, "%test%"));
			Assert.AreEqual("_test", filter.ExtractLikeString(AutoFilterCondition.EndsWith, "%_test"));
			Assert.AreEqual("a", filter.ExtractLikeString(AutoFilterCondition.Contains, "%a%"));
			Assert.AreEqual("", filter.ExtractLikeString(AutoFilterCondition.Contains, "%"));
			Assert.AreEqual("a", filter.ExtractLikeString(AutoFilterCondition.EndsWith, "%a"));
			Assert.AreEqual("", filter.ExtractLikeString(AutoFilterCondition.EndsWith, "%"));
			Assert.AreEqual("", filter.ExtractLikeString(AutoFilterCondition.BeginsWith, "%"));
		}
		[Test]
		public void GetColumnAutoFilterTextTests() {
			BaseFilterHelperTester filter = new BaseFilterHelperTester();
			Assert.AreEqual("test", filter.GetColumnAutoFilterText(AutoFilterCondition.BeginsWith, "Test", new BinaryOperator("Test", "test%", BinaryOperatorType.Like)));
			Assert.AreEqual("", filter.GetColumnAutoFilterText(AutoFilterCondition.BeginsWith, "BadTest", new BinaryOperator("Test", "test%", BinaryOperatorType.Like)));
			Assert.AreEqual("test", filter.GetColumnAutoFilterText(AutoFilterCondition.Equals, "Test", new BinaryOperator("Test", "test", BinaryOperatorType.Equal)));
			Assert.AreEqual("test", filter.GetColumnAutoFilterText(AutoFilterCondition.Equals, "Test", new GroupOperator(new BinaryOperator("Test", "test", BinaryOperatorType.GreaterOrEqual), new BinaryOperator("Test", "test", BinaryOperatorType.GreaterOrEqual))));
			Assert.IsEmpty(filter.GetColumnAutoFilterText(AutoFilterCondition.Default, "Test", CriteriaOperator.Parse("[Test] in (1,2)")));
		}
	}
}
#endif
