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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;
using DevExpress.Data;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Web.ASPxPivotGrid.Data;
using System.Security;
[assembly: AllowPartiallyTrustedCallers()]
[assembly: AssemblyTitle("DevExpress.Web.ASPxPivotGrid")]
[assembly: AssemblyDescription("ASPxPivotGrid")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("DevExpress.Web.ASPxPivotGrid")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2009 Developer Express Inc.")]
[assembly: AssemblyTrademark("ASPxPivotGrid")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
#pragma warning disable 1699
[assembly: AssemblyKeyFile(@"..\..\..\Devexpress.Key\StrongKey.snk")]
#pragma warning restore 1699
[assembly: AssemblyKeyName("")]
[assembly: TagPrefix("DevExpress.Web.ASPxPivotGrid", "dxwpg")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.LoadingPanelImageName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.FieldValueCollapsedName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.FieldValueExpandedName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.HeaderSortDownName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.HeaderSortUpName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.HeaderFilterName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.HeaderActiveFilterName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.FilterWindowSizeGripName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.CustomizationFieldsBackgroundName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.CustomizationFieldsCloseName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.DragArrowDownName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.DragArrowUpName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.DragHideFieldName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.DataHeadersPopupName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.GroupSeparatorName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.SortByColumnName + ".gif", "image/gif")]
[assembly: WebResource(PivotGridWebData.PivotGridImagesResourcePath + PivotGridImages.PrefilterButtonName + ".png", "image/png")]
[assembly: WebResource(PivotGridWebData.PivotGridScriptResourceName, "application/x-javascript")]
[assembly: WebResource(PivotGridWebData.PivotGridCssResourceName, "text/css")]
