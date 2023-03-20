/*
* @(#)CustomAction.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
    using System.Xml;
    using System.Text;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing.Design;

    using Newtera.Common.MetaData;

	/// <summary>
	/// A class represents a custom action for a task
	/// </summary>
	/// <version>1.0.0 24 Aug 2008 </version>
    [Serializable]
    public class CustomAction : CustomTypeDescriptorBase
	{
        private string _id;
        private string _name;
        private FormAction _formAction;
        private bool _needConfirm;
        private bool _needToCloseTask;
        private string _confirmMessage;
        private string _code; // in-line action code
        private string _apiName; // api to be called
        private string _schemaId; // runtime use, do not show and serialize
        private string _className; // runtime use, do not show and serialize
        private string _externalHandler; // external action DLL def
        private bool _isBatchAction; // is it a batch action
        private StringCollection _grantedRoles; // roles that are granted the access to the action


        // no param constructor required by workflow runtime
        public CustomAction()
        {
            _id = null;
            _name = null;
            _formAction = FormAction.None; // default to Submite
            _needConfirm = false;
            _needToCloseTask = false;
            _confirmMessage = null;
            _code = null;
            _apiName = null;
            _schemaId = null;
            _className = null;
            _externalHandler = null;
            _isBatchAction = false;
            _grantedRoles = null;
        }

        public CustomAction(string name)
        {
            _id = "CA" + Guid.NewGuid().ToString("N");
            _name = name;
            _formAction = FormAction.None; // default to Submit
            _needConfirm = false;
            _needToCloseTask = false;
            _confirmMessage = null;
            _code = null;
            _apiName = null;
            _schemaId = null;
            _className = null;
            _externalHandler = null;
            _isBatchAction = false;
            _grantedRoles = null;
        }

        /// <summary>
        /// Initiating an instance of CustomAction class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal CustomAction(XmlElement xmlElement)
		{
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets the unique id of the custom action
        /// </summary>
        [BrowsableAttribute(false)]
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the custom action
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The name of the action"),
            ReadOnlyAttribute(true)
        ]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the form action after the custom action is performed 
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The action to the task form after the custom action is performed "),
        ]
        public FormAction FormAction
        {
            get
            {
                return _formAction;
            }
            set
            {
                _formAction = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicates whether the custom action needs to close the workflow task after it is performed
        /// </summary>
        [
            CategoryAttribute("System"),
            DefaultValueAttribute(false),
            DescriptionAttribute("whether the custom action needs to close the workflow task after it is performed"),
        ]
        public bool CloseTask
        {
            get
            {
                return _needToCloseTask;
                ;
            }
            set
            {
                _needToCloseTask = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicates whether the custom action needs to be confirmed before it is performed 
        /// </summary>
        [BrowsableAttribute(false)]
        /*
        [
            CategoryAttribute("System"),
            DefaultValueAttribute(false),
            DescriptionAttribute("whether the custom action needs to be confirmed before it is performed"),
        ]
        */
        public bool NeedConfirm
        {
            get
            {
                return _needConfirm;
                ;
            }
            set
            {
                _needConfirm = value;
            }
        }

        /// <summary>
        /// Gets or sets the customized confirm message 
        /// </summary>
        [BrowsableAttribute(false)]
        public string ConfirmMessage
        {
            get
            {
                return _confirmMessage;
                ;
            }
            set
            {
                _confirmMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets the data type of the parameter
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The code of action"),
            EditorAttribute("WorkflowStudio.ActionCodeEditor, WorkflowStudio", typeof(UITypeEditor)),
        ]
        public string ActionCode
        {
            get
            {
                return _code;
            }
            set
            {
                if (!string.IsNullOrEmpty(_code) && _code != value)
                {
                    // The custom code has been changed,
                    // regenerate the ID so that the new code can be recompiled
                    _id = "CA" + Guid.NewGuid().ToString("N");
                }

                _code = value;
            }
        }

        /// <summary>
        /// Gets or sets the external code in form of "NameSpace.ClassName,LibName"
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The external action dll"),
            EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))
        ]
        public string ExternalAction
        {
            get
            {
                return _externalHandler;
            }
            set
            {
                _externalHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of an api associated with the action
        /// </summary>
        /*
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The name of an api associated with the action"),
            EditorAttribute("WorkflowStudio.ApiNamePropertyEditor, WorkflowStudio", typeof(UITypeEditor)),
            DefaultValueAttribute("None")
        ]
        */
        [BrowsableAttribute(false)]
        public string ApiName
        {
            get
            {
                return _apiName;
            }
            set
            {
                _apiName = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the action is a batch action
        /// </summary>
        [BrowsableAttribute(false)]
        public bool IsBatchAction
        {
            get
            {
                return _isBatchAction;
            }
            set
            {
                _isBatchAction = value;
            }
        }

        /// <summary>
        /// Gets or sets roles that are granted access to the action. When not specified, access to the action is granted to everyone
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("Roles granted the access to the action"),
            DefaultValueAttribute(null),
            EditorAttribute("WorkflowStudio.GrantedRolesPropertyEditor, WorkflowStudio", typeof(UITypeEditor))
        ]
        public StringCollection GrantedRoles
        {
            get
            {
                return _grantedRoles;
            }
            set
            {
                _grantedRoles = value;
            }
        }

        /// <summary>
        /// Gets or sets the schema id indicating the database schema of a binding class
        /// </summary>
        [BrowsableAttribute(false)]
        public string SchemaId
        {
            get
            {
                return _schemaId;
            }
            set
            {
                _schemaId = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the binding class
        /// </summary>
        [BrowsableAttribute(false)]
        public string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }

        /// <summary>
        /// Clone the CustomAction object
        /// </summary>
        /// <returns>The cloned object</returns>
        public CustomAction Clone()
        {
            CustomAction clone = new CustomAction(Name);
            clone.ID = ID;
            clone.ActionCode = ActionCode;
            clone.FormAction = FormAction;
            clone.SchemaId = SchemaId;
            clone.ClassName = ClassName;
            clone.NeedConfirm = NeedConfirm;
            clone.ConfirmMessage = ConfirmMessage;
            clone.CloseTask = CloseTask;
            clone.ApiName = ApiName;
            clone.ExternalAction = ExternalAction;
            clone.IsBatchAction = IsBatchAction;
            clone.GrantedRoles = GrantedRoles;
            return clone;
        }

        /// <summary>
        /// sets the element members from a XML element.
        /// </summary>
        /// <param name="parent">An xml element</param>
        internal void Unmarshal(XmlElement parent)
        {
            // set value of _id member
            _id = parent.GetAttribute("ID");

            // set value of _name member
            _name = parent.GetAttribute("Name");

            // set the value of _formAction
            try
            {
                _formAction = (FormAction)Enum.Parse(typeof(FormAction), parent.GetAttribute("FormAction"));
            }
            catch (Exception)
            {
                _formAction = FormAction.None;
            }

            // set _needConfirm property
            string str = parent.GetAttribute("NeedConfirm");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _needConfirm = true;
            }
            else
            {
                _needConfirm = false;
            }

            _confirmMessage = parent.GetAttribute("ConfirmMessage");

            // set _needToCloseTask property
            str = parent.GetAttribute("CloseTask");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _needToCloseTask = true;
            }
            else
            {
                _needToCloseTask = false;
            }

            str = parent.InnerText;

            if (str != null && str.Length > 0)
            {
                // hack, replace \n with \r\n. For some reason \r\n in the script
                // was changed to \n when saved to database
                if (str.IndexOf("\r\n") < 0)
                {
                    // do it once
                    _code = str.Replace("\n", "\r\n");
                }
                else
                {
                    _code = str;
                }
            }
            else
            {
                _code = null;
            }

            str = parent.GetAttribute("externalAction");
            if (!string.IsNullOrEmpty(str))
            {
                _externalHandler = str;
            }
            else
            {
                _externalHandler = null;
            }

            // set _isBatchAction property
            str = parent.GetAttribute("isBatch");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                _isBatchAction = true;
            }
            else
            {
                _isBatchAction = false;
            }

            // set _apiName property
            str = parent.GetAttribute("api");
            if (!string.IsNullOrEmpty(str))
            {
                _apiName = str;
            }
            else
            {
                _apiName = null;
            }

            // set _grantedRoles property
            str = parent.GetAttribute("grantedRoles");
            if (!string.IsNullOrEmpty(str))
            {
                string[] roles = str.Split(';');
                StringCollection grantedRoles = new StringCollection();
                for (int i = 0; i < roles.Length; i++)
                {
                    grantedRoles.Add(roles[i]);
                }
                _grantedRoles = grantedRoles;
            }
            else
            {
                _grantedRoles = null;
            }
        }

        /// <summary>
        /// Write values of members to an xml element
        /// </summary>
        /// <param name="parent">An xml element for the element</param>
        internal void Marshal(XmlElement parent)
        {
            // write the _id member
            parent.SetAttribute("ID", _id);

            // write the _name member
            parent.SetAttribute("Name", _name);


            // write needConfirm member
            if (_needConfirm)
            {
                parent.SetAttribute("NeedConfirm", "true");
            }

            if (!string.IsNullOrEmpty(_confirmMessage))
            {
                parent.SetAttribute("ConfirmMessage", _confirmMessage);
            }

            // write _needToCloseTask member
            if (_needToCloseTask)
            {
                parent.SetAttribute("CloseTask", "true");
            }

            // write the _formAction member
            try
            {
                parent.SetAttribute("FormAction", Enum.GetName(typeof(FormAction), _formAction));
            }
            catch (Exception)
            {
            }

            if (!string.IsNullOrEmpty(_externalHandler))
            {
                parent.SetAttribute("externalAction", _externalHandler);
            }

            if (_isBatchAction)
            {
                parent.SetAttribute("isBatch", "true");
            }

            if (!string.IsNullOrEmpty(_apiName))
            {
                parent.SetAttribute("api", _apiName);
            }

            if (_grantedRoles != null && _grantedRoles.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (string role in _grantedRoles)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(";");
                    }

                    builder.Append(role);
                }

                parent.SetAttribute("grantedRoles", builder.ToString());
            }

            if (!string.IsNullOrEmpty(_code))
            {
                // Set the _code member
                parent.InnerText = _code;
            }
        }
	}

    /// <summary>
    /// Define what to do with the task form after the action is performed
    /// </summary>
    public enum FormAction
    {
        None,
        Submit,
        SubmitAndClose,
    }
}