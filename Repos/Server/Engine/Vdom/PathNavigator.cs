/*
* @(#)PathNavigator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom
{
	using System;
	using System.Xml;
	using System.Xml.XPath;
	using System.Collections;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Vdom.Common;
	using Newtera.Server.Engine.Interpreter;

	/// <summary>
	///  Navigate through a xml documnet given a pathEnumerator object
	/// </summary>
	/// <version>1.0.0 11 Jul 2003</version>
	/// <author> Yong Zhang</author>
	public class PathNavigator
	{
		/// <summary>
		/// Initializing PathNavigator object
		/// </summary>
		public PathNavigator()
		{
		}

		/// <summary> 
		/// This method can be called recursively for each step in a path.
		/// </summary>
		/// <param name="currentNodeList">the current base node list.</param>
		/// <param name="pathEnumerator">the iteratror that steps throgh the path.</param>
		/// <returns>
		/// the ValueCollection containing the nodes selected by the current step.
		/// </returns>
		public ValueCollection Navigate(ValueCollection currentNodeList, PathEnumerator pathEnumerator)
		{	
			VDocument doc = null;
			Step currentStep;
			IEnumerator enumerator;

			// terminated if it is the end of path
			if (pathEnumerator.MoveNext())
			{
				currentStep = (Step) pathEnumerator.Current;
			}
			else
			{
				return currentNodeList;
			}
			
			ValueCollection newNodeCollection = new ValueCollection();
			foreach (XNode xnode in currentNodeList)
			{
				XmlNode node = xnode.ToNode();
				doc = (VDocument) node.OwnerDocument;

				switch (currentStep.AxisCode)
				{
					
					case Axis.CHILD: 
						enumerator = doc.GetChildAxisEnumerator(node);
						break;
					
					case Axis.DESCENDANT: 
						enumerator = doc.GetDescendantAxisEnumerator(node);
						break;
					
					case Axis.PARENT: 
						enumerator = doc.GetParentAxisEnumerator(node);
						break;
					
					case Axis.ANCESTOR: 
						enumerator = doc.GetAncestorAxisEnumerator(node);
						break;
					
					case Axis.FOLLOWINGSIBLING: 
						enumerator = doc.GetFollowingSiblingAxisEnumerator(node);
						break;
					
					case Axis.PRECEDINGSIBLING: 
						enumerator = doc.GetPrecedingSiblingAxisEnumerator(node);
						break;
					
					case Axis.FOLLOWING: 
						enumerator = doc.GetFollowingAxisEnumerator(node);
						break;
					
					case Axis.PRECEDING: 
						enumerator = doc.GetPrecedingAxisEnumerator(node);
						break;
					
					case Axis.ATTRIBUTE: 
						enumerator = doc.GetAttributeAxisEnumerator(node);
						break;
					
					case Axis.SELF: 
						enumerator = doc.GetSelfAxisEnumerator(node);
						break;
					
					case Axis.ANCESTORORSELF: 
						enumerator = doc.GetAncestorOrSelfAxisEnumerator(node);
						break;
					
					case Axis.DEREFERENCE: 
						enumerator = doc.GetDereferenceAxisEnumerator(node);
						break;
					
					case Axis.FUNCTION: 
						enumerator = doc.GetChildAxisEnumerator(node);
						break;
					
					default: 
						throw new UnsupportedAxisException("Unknow axis :" + currentStep.AxisCode);
				}
				
				/*
				* select and add those nodes that satisfies the name and qualifier defined for
				* the step.
				*/
				try
				{
					while (enumerator.MoveNext())
					{
						
						if (currentStep.Name != null)
						{
							XmlNode newNode = (XmlNode) enumerator.Current;
							if (currentStep.Name == newNode.LocalName &&
								(currentStep.Prefix == null || currentStep.Prefix == newNode.Prefix))
							{
                                // The nodes has been filtered by SQL statements
                                // generated for DB virtual documents, therefore,
                                // we only do the filtering for text document.
                                if (!doc.IsDB)
								{
									if (currentStep.Qualifier != null)
									{
										// TODO, set the current node as an context node
										if (currentStep.Qualifier.Eval().ToBoolean())
										{
											// filter the node with qualifier
											newNodeCollection.Add(new XNode(newNode));
										}
									}
									else
									{
										newNodeCollection.Add(new XNode(newNode));
									}
								}
								else
								{
                                    // obtain the value for a virtual attribute that is a part of final output
                                    // to avoid unnecessary cost of computing value of virtual attribute
                                    if (currentStep.AxisCode == Axis.CHILD &&
                                        doc.IsVirtualAttribute(newNode) &&
                                        newNode.InnerText.StartsWith("virtual_"))
                                    {
                                        newNode.InnerText = doc.ObtainVirualAttributeValue(newNode);
                                    }

                                    newNodeCollection.Add(new XNode(newNode));
								}
							}
						}
					}
				}
				catch (InterpreterException e)
				{
					throw new VDOMException(e.Message, e);
				}
			}
			
			return Navigate(newNodeCollection, pathEnumerator);
		}
	}
}