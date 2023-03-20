/*
* @(#)SubstituteEntry.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Xml;
    using System.IO;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing.Design;

	/// <summary>
	/// Describing an entry in the substitute model. An user may have multiple substitutes defined.
    /// Each substitute has an entry added to the user entry.
	/// </summary>
	/// <version>1.0.6 25 Oct 2008</version>
	public class SubstituteEntry : WFModelElementBase
	{
        private EffecitiveStatus _effectiveStatus = EffecitiveStatus.Disabled;
        private DateTime _startDate = DateTime.MinValue;
        private DateTime _endDate = DateTime.MinValue;
        private TaskScope _taskScope = TaskScope.AllProjects;
        private string _projectName; // valid when _taskScope is set to Specific project
        private StringCollection _substituteUsers = null; // the users appointed as substitutes

        /// <summary>
        /// Initiating an instance of SubstituteEntry class
        /// </summary>
        public SubstituteEntry(string name) : base(name)
		{
		}

		/// <summary>
		/// Initiating an instance of SubstituteEntry class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SubstituteEntry(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets the type of element
        /// </summary>
        /// <value>One of ElementType values</value>
        [BrowsableAttribute(false)]	
        public override ElementType ElementType
        {
            get
            {
                return ElementType.SubstituteEntry;
            }
        }

        /// <summary>
        /// Override so that it doesn't appear in the propertygrid
        /// </summary>
        /// <value> The name of the element</value>
        [BrowsableAttribute(false)]	
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// Override so that it doesn't appear in the propertygrid
        /// </summary>
        /// <value> The name of the element</value>
        [BrowsableAttribute(false)]	
        public override string Description
        {
            get
            {
                return base.Description;
            }
            set
            {
                base.Description = value;
            }
        }

        /// <summary>
        /// Gets or sets the effective status of the substitute rule.
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The effective status of the substitute rule"),
            DefaultValueAttribute(EffecitiveStatus.Disabled)
        ]
        public EffecitiveStatus EffectiveStatus
        {
            get
            {
                return _effectiveStatus;
            }
            set
            {
                _effectiveStatus = value;
            }
        }

        /// <summary>
        /// Gets or sets the start date of the the substitute rule when effective status is Duration.
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The start date of the the substitute rule when effective status is Duration"),
        ]
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the end date of the the substitute rule when effective status is Duration.
        /// </summary>
        /// <remarks>End date is optional which means the end date is not determined</remarks>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The end date of the the substitute rule when effective status is Duration"),
        ]
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the scope of the tasks to be performed by specified substitute user(s)
        /// </summary>
        /// <value>One of TaskScope enum values. default is AllProjects</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("the scope of the tasks to be performed by specified substitute user(s)"),
            DefaultValueAttribute(TaskScope.AllProjects)
        ]
        public TaskScope TaskScope
        {
            get
            {
                return _taskScope;
            }
            set
            {
                _taskScope = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of project when the TaskScope is set to SpecificProject. 
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("the name of project when the TaskScope is set to SpecificProject"),
            EditorAttribute("WorkflowStudio.ProjectNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor)), 
            DefaultValueAttribute(null)
        ]
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
            }
        }

        /// <summary>
        /// Gets or sets the users that are the substitutes for the task.
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("the users that are the substitutes for the task.."),
            EditorAttribute("WorkflowStudio.SubstituteUsersPropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        public StringCollection SubstituteUsers
        {
            get
            {
                return _substituteUsers;
            }
            set
            {
                _substituteUsers = value;
            }
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IWFModelElementVisitor visitor)
		{
			visitor.VisitSubstituteEntry(this);
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            string str;

            _effectiveStatus = (EffecitiveStatus)Enum.Parse(typeof(EffecitiveStatus), parent.GetAttribute("status"));

            str = parent.GetAttribute("start");
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    _startDate = DateTime.Parse(str);
                }
                catch (Exception)
                {
                }
            }

            str = parent.GetAttribute("end");
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    _endDate = DateTime.Parse(str);
                }
                catch (Exception)
                {
                }
            }

            str = parent.GetAttribute("project");
            if (!string.IsNullOrEmpty(str))
            {
                _projectName = str;
            }
            else
            {
                _projectName = null;
            }

            _taskScope = (TaskScope)Enum.Parse(typeof(TaskScope), parent.GetAttribute("scope"));

            str = parent.GetAttribute("substitutes");
            if (!string.IsNullOrEmpty(str))
            {
                string[] users = str.Split(';');
                _substituteUsers = new StringCollection();
                foreach (string usr in users)
                {
                    _substituteUsers.Add(usr);
                }
            }
            else
            {
                _substituteUsers = null;
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            parent.SetAttribute("status", Enum.GetName(typeof(EffecitiveStatus), _effectiveStatus));

            if (_startDate != null)
            {
                parent.SetAttribute("start", _startDate.ToShortDateString());
            }

            if (_endDate != null)
            {
                parent.SetAttribute("end", _endDate.ToShortDateString());
            }

            if (!string.IsNullOrEmpty(_projectName))
            {
                parent.SetAttribute("project", _projectName);
            }

            parent.SetAttribute("scope", Enum.GetName(typeof(TaskScope), _taskScope));

            if (_substituteUsers != null)
            {
                string userList = null;
                foreach (string usr in _substituteUsers)
                {
                    if (userList == null)
                    {
                        userList = usr;
                    }
                    else
                    {
                        userList = userList + ";" + usr;
                    }
                }

                parent.SetAttribute("substitutes", userList);
            }
		}

        public override string ToString()
        {
            return this.Name;
        }
	}

    /// <summary>
    /// Specify when to let substitutes peform the tasks
    /// </summary>
    public enum EffecitiveStatus
    {
        Disabled,
        Immediately,
        Duration
    }

    /// <summary>
    /// Specify the scope of tasks to be done by substitutes
    /// </summary>
    public enum TaskScope
    {
        AllProjects,
        SpecificProject
    }
}