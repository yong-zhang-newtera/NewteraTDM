/*
* @(#)SubjectEntry.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
    using System;
    using System.Xml;
    using System.Collections.Specialized;

    /// <summary>
    /// Describing an entry in the substitute model. For an user or role who has defined substitute
    /// info, a subject entry is added to the top level TaskSubstituteModel
    /// </summary>
    /// <version>1.0.6 25 Oct 2008</version>
    public class SubjectEntry : WFModelElementBase
	{
        private string _subjectCaption;
        private SubjectType _type;
        private SubstituteEntryCollection _substitutes;

        /// <summary>
        /// Initiating an instance of SubjectEntry class
        /// </summary>
        /// <param name="subjectName">Name of the subject.</param>
        public SubjectEntry(string subjectName, string subjectCaption)
            : this(subjectName, subjectCaption, SubjectType.User)
        {
        }

		/// <summary>
		/// Initiating an instance of SubjectEntry class
		/// </summary>
        /// <param name="subjectName">Name of the subject.</param>
        public SubjectEntry(string subjectName, string subjectCaption, SubjectType type) : base(subjectName)
		{
            _subjectCaption = subjectCaption;
            _type = type;
            _substitutes = new SubstituteEntryCollection();
		}

		/// <summary>
		/// Initiating an instance of SubjectEntry class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SubjectEntry(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Display text of the subject
        /// </summary>
        public string SubjectCaption
        {
            get
            {
                return _subjectCaption;
            }
        }

        /// <summary>
        /// Gets the substitute entry collection
        /// </summary>
        public SubstituteEntryCollection SubstituteEntries
        {
            get
            {
                return _substitutes;
            }
        }

        /// <summary>
        /// Gets the type of element
        /// </summary>
        /// <value>One of ElementType values</value>
        public override ElementType ElementType
        {
            get
            {
                return ElementType.SubjectEntry;
            }
        }

        /// <summary>
        /// Gets a list of substitute users to perform a task owned by an user
        /// </summary>
        /// <param name="projectName">The name of project in question</param>
        /// <param name="projectVersion">The version of the project in question</param>
        /// <returns>A list of substitute users, null if there are no substitute users found</returns>
        public StringCollection GetSubstituteUsers(string projectName, string projectVersion)
        {
            StringCollection substituteUserList = null;

            SubstituteEntryCollection matchedEntries = GetMatchedSubstituteEntries(projectName, projectVersion);

            if (matchedEntries != null && matchedEntries.Count > 0)
            {
                substituteUserList = new StringCollection();
                foreach (SubstituteEntry substituteEntry in matchedEntries)
                {
                    foreach (string substituteUser in substituteEntry.SubstituteUsers)
                    {
                        if (!substituteUserList.Contains(substituteUser))
                        {
                            substituteUserList.Add(substituteUser);
                        }
                    }
                }
            }

            return substituteUserList;
        }

        public bool Contains(SubstituteEntry substituteEntry)
        {
            bool status = false;

            foreach (SubstituteEntry entry in _substitutes)
            {
                if (entry == substituteEntry)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

        public void AddSubstituteEntry(SubstituteEntry substituteEntry)
        {
            this._substitutes.Add(substituteEntry);
        }

        public void RmoveSubstituteEntry(SubstituteEntry substituteEntry)
        {
            this._substitutes.Remove(substituteEntry);
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IWFModelElementVisitor visitor)
		{
			visitor.VisitSubjectEntry(this);
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            _subjectCaption = parent.GetAttribute("caption");

            _type = (SubjectType)Enum.Parse(typeof(SubjectType), parent.GetAttribute("type"));

            // then a collection of  substitute entries
            _substitutes = (SubstituteEntryCollection)ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            parent.SetAttribute("caption", _subjectCaption);

            parent.SetAttribute("type", Enum.GetName(typeof(SubjectType), _type));

            // write the _substitutes
            XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_substitutes.ElementType));
            _substitutes.Marshal(child);
            parent.AppendChild(child);
		}

        public override string ToString()
        {
            return this.Name + " (" + this.SubjectCaption + ")";
        }

        private SubstituteEntryCollection GetMatchedSubstituteEntries(string projectName, string projectVersion)
        {
            SubstituteEntryCollection matchedEntries = new SubstituteEntryCollection();

            foreach (SubstituteEntry substituteEntry in _substitutes)
            {
                if (substituteEntry.EffectiveStatus != EffecitiveStatus.Disabled)
                {
                    if (substituteEntry.EffectiveStatus == EffecitiveStatus.Duration &&
                        !InsideDuration(substituteEntry))
                    {
                        continue; // skip the rest of code
                    }

                    if (substituteEntry.TaskScope == TaskScope.AllProjects)
                    {
                        matchedEntries.Add(substituteEntry);
                    }
                    else if (substituteEntry.TaskScope == TaskScope.SpecificProject)
                    {
                        // compare the project name and version
                        string projectId = projectName + " " + projectVersion;

                        // Note the format of substituteEntry.ProjectName is name + space + version
                        if (!string.IsNullOrEmpty(substituteEntry.ProjectName) &&
                            substituteEntry.ProjectName == projectId)
                        {
                            matchedEntries.Add(substituteEntry);
                        }
                    }
                }
            }

            return matchedEntries;
        }

        private bool InsideDuration(SubstituteEntry substituteEntry)
        {
            bool status = false;

            if (substituteEntry.StartDate != DateTime.MinValue)
            {
                if (DateTime.Today >= substituteEntry.StartDate)
                {
                    if (substituteEntry.EndDate != DateTime.MinValue)
                    {
                        if (DateTime.Today <= substituteEntry.EndDate)
                        {
                            status = true;
                        }
                    }
                    else
                    {
                        // end date not specified, which means no ending
                        status = true;
                    }
                }
            }

            return status;
        }
	}

    public enum SubjectType
    {
        User,
        Role
    }
}