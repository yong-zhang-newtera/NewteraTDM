/*
* @(#)ProjectModelComparator.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.WorkflowModel
{
	using System;

	using Newtera.Server.DB;
    using Newtera.WFModel;

	/// <summary>
	/// Compare two ProjectModel instances and return a comparng result that
	/// stores differences between the ProjectModel instances. The result is then
	/// used to determine the necessary updating actions to the project model stored
	/// in the database.
	/// </summary>
	/// <version>  	1.0.0 15 Dec 2006 </version>
	public class ProjectModelComparator
	{
		private IDataProvider _dataProvider;

		/// <summary>
		/// Initializes a new instance of the ProjectModelComparator class
		/// </summary>
		/// <param name="dataProvider">Data Provider</param>
		public ProjectModelComparator(IDataProvider dataProvider)
		{
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Compare two ProjectModel instances and return the comparison results
		/// containing the actions to sync with project model in database with the new
		/// project model.
		/// </summary>
		/// <param name="newProjectModel">The new project model</param>
		/// <param name="oldProjectModel">The old project model</param>
		/// <returns>The comparison result</returns>
		public ProjectModelCompareResult Compare(ProjectModel newProjectModel,
			ProjectModel oldProjectModel)
		{
			ProjectModelCompareResult result = new ProjectModelCompareResult(newProjectModel, oldProjectModel);

			if (oldProjectModel == null)
			{
				// the new project model is a brand new one, all the elements are considered
				// to be added ones
				IWFModelElementVisitor visitor = new FindAdditionVisitor(newProjectModel,
					oldProjectModel, result, _dataProvider);
				newProjectModel.Accept(visitor);
			}
			else if (newProjectModel == null)
			{
				// delete the old project model, all the elements in the project model are
				// considered to be deleted ones
				IWFModelElementVisitor visitor = new FindDeletionVisitor(newProjectModel,
					oldProjectModel, result, _dataProvider);
				oldProjectModel.Accept(visitor);
			}
			else
			{
				// Find added workflow model elements and add corresponding actions to the result
				IWFModelElementVisitor visitor = new FindAdditionVisitor(newProjectModel,
					oldProjectModel, result, _dataProvider);
				newProjectModel.Accept(visitor);

				// Find deleted workflow model elements and add corresponding actions to the result
				visitor = new FindDeletionVisitor(newProjectModel,
					oldProjectModel, result, _dataProvider);
				oldProjectModel.Accept(visitor);
			}

			return result;
		}
	}
}