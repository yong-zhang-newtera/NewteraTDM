#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       ASPxPivotGrid                                 }
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
using DevExpress.Utils.About;
using DevExpress.Web.ASPxClasses.Design;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxPivotGrid;
using System.Web.UI.Design;
using System.ComponentModel;
namespace DevExpress.Web.ASPxPivotGrid.Design {
	public class PivotGridAboutDialogHelper : AboutDialogHelperBase {
		private static bool fTrialAboutShown = false;
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxPivotGrid), ProductKind.ASPxPivotGrid,
				GetProductInfoStage(typeof(ASPxPivotGrid)), "DevExpress.Web.ASPxPivotGrid.about.png", 298, 171);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if (IsTrial(typeof(ASPxPivotGrid)) && !fTrialAboutShown) {
				ShowAbout(provider);
				fTrialAboutShown = true;
			}
		}
	}
	public class FieldNameTypeConverter : DataSourceViewSchemaConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context, Type typeFilter) {
			if(context.Instance is PivotGridField) {
				ASPxPivotGrid grid = ((PivotGridField)context.Instance).PivotGrid;
				if(grid != null && !String.IsNullOrEmpty(grid.OLAPConnectionString)) {
					grid.Data.OLAPConnectionString = grid.OLAPConnectionString;
					return new StandardValuesCollection(grid.Data.GetFieldList());
				}
			}
			return base.GetStandardValues(context, typeFilter);
		}
	}
}
