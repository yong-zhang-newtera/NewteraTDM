using System;
using System.Xml;
using System.IO;
using System.Threading;


namespace Newtera.Registry
{ 
    public delegate void ErrorDelegate(Exception e);

	public class XmlRegistryKey
	{
		private XmlElement Elem;
		private XmlRegistry Reg;

		public string Name { get { return Elem.Name; } }


        public void SetValue(object val)
        {
            SetValue("value", val);
        }

        public void SetValue(string name, object val)
		{
            bool forceSave = false;

			XmlAttribute at = Elem.Attributes[name];
            if (at == null)
            {
                // brand new attribute and brand new vlaue, force to save it
                Elem.Attributes.Append(at = Elem.OwnerDocument.CreateAttribute(name));
                forceSave = true;
            }
            else if (string.IsNullOrEmpty(at.Value))
            {
                // fisrt assignment, force save the doc
                forceSave = true;
            }

            at.Value = val.ToString();

			Reg.Save(forceSave);
		}

        public bool GetBooleanValue()
        {
            return GetBooleanValue("value", false);
        }

        public bool GetBooleanValue(string name, bool defaultval)
		{
			XmlAttribute at = Elem.Attributes[name];
			if (at == null)
				return defaultval;
			else
			{
				string val = at.Value.ToUpper();
				return val == "YES" || val == "TRUE";
			}
		}

        public string GetStringValue()
        {
            return GetStringValue("value", "");
        }

        public string GetStringValue(string name, string defaultval)
		{
			XmlAttribute at = Elem.Attributes[name];
			if (at == null)
				return defaultval;
			else
				return at.Value;
		}

        public int GetIntValue()
        {
            return GetIntValue("value", 0);
        }

        public int GetIntValue(string name, int defaultval)
		{
			XmlAttribute at = Elem.Attributes[name];
			if (at == null)
				return defaultval;
			else
				return int.Parse(at.Value);
		}

        public XmlRegistryKey[] GetSubKeys()
		{
			int keycount = Elem.ChildNodes.Count;
			if (keycount == 0)
				return null;
			XmlRegistryKey[] keys = new XmlRegistryKey[keycount];
			for (int i=0; i < keycount; i++)
				keys[i] = new XmlRegistryKey((XmlElement)Elem.ChildNodes[i], Reg);
			return keys;
		}


		public XmlRegistryKey GetSubKey(string path, bool createpath)
		{
			XmlElement e = Elem, parent = null;
			for (int len, start=0; start < path.Length; start += len+1)
			{
				len = path.IndexOf('/', start);
				if (len == -1)
					len = path.Length;
				len -= start;
				string node = path.Substring(start, len);
				parent = e;
				e = e[node];
				if (e == null)
					if (createpath)
						parent.AppendChild(e = Elem.OwnerDocument.CreateElement(node));
					else
						return null;
			}
			return new XmlRegistryKey(e, Reg);
		}


		public XmlRegistryKey this[string path, bool createpath]
		{
			get
			{
				return GetSubKey(path, createpath);
			}
		}

        public string[] GetSubKeyNames()
        {
            string[] names = new string[Elem.ChildNodes.Count];

            for (int i = 0; i < Elem.ChildNodes.Count; i++)
            {
                names[i] = Elem.ChildNodes[i].Name;
            }

            return names;
        }

        public void DeleteSubKeyTree(string name)
        {
            XmlNode child = Elem[name];
            if (child != null)
            {
                Elem.RemoveChild(child);

                Reg.Save(true);
            }
        }

        public void Close()
        {

        }

		internal XmlRegistryKey(XmlElement e, XmlRegistry r)
		{
			Elem = e;
			Reg = r;
		}
	}


	public class XmlRegistry
	{
		private FileInfo Fileinfo;
		private XmlDocument Doc = new XmlDocument();
		private const string MsgPrefix = "XmlRegistry:";
		private ErrorDelegate ErrorHandler=null;
		private int WriteDelay;
		internal bool LazyWrite = true;
        internal int Threshhold = 100;
        private int SaveCount = 0;
		private Exception _LastException = null;

		public Exception LastException { get { return _LastException; } }


		private void SaveDelegate()
		{
			Thread.Sleep(WriteDelay);
			lock (this)
			{
				try
				{
					Doc.Save(Fileinfo.FullName);
				}
				catch (Exception e)
				{
					_LastException = e;
					if (ErrorHandler != null)
						ErrorHandler(e);
				}
			}
		}


		public void Save(bool forceSave)
		{
            if (LazyWrite && !forceSave)
            {
                SaveCount++;
                if (SaveCount > Threshhold)
                {
                    SaveCount = 0; // start over

                    Doc.Save(Fileinfo.FullName);
                }
            }
            else
            {
                Doc.Save(Fileinfo.FullName);
            }
		}


		public XmlRegistryKey RootKey
		{
			get { return new XmlRegistryKey(Doc.DocumentElement, this); }
		}


		public XmlRegistry(string filename, ErrorDelegate errhandler, int writedelay)
			: this(filename, null, null, errhandler, writedelay)
		{
		}


		public XmlRegistry(string filename, string rootkeyname, string encoding, ErrorDelegate errhandler, int writedelay)
			: this(filename, rootkeyname, encoding)
		{
			ErrorHandler = errhandler;
			WriteDelay = writedelay;
			LazyWrite = false;
		}
		

		public XmlRegistry(string filename, string rootkeyname, string encoding)
		{
			Fileinfo = new FileInfo(filename);
			if (Fileinfo.Exists)
			{
				Doc.Load(Fileinfo.FullName);
				if (rootkeyname != null && Doc.DocumentElement.Name != rootkeyname)
				{
					string msg = string.Format("{0} Specified root node name '{1}' does not match root name '{2}' in loaded document.", MsgPrefix, rootkeyname, Doc.DocumentElement.Name);
					throw new Exception(msg);
				}
			}
			else
			{
				if (rootkeyname == null)
					rootkeyname = "NewteraRegistry";
				XmlDeclaration dec = Doc.CreateXmlDeclaration("1.0", encoding, null);
				Doc.AppendChild(dec);
				XmlElement root = Doc.CreateElement(rootkeyname);
				Doc.AppendChild(root);
			}
		}


		public XmlRegistry(string filename) : this(filename, null, null)
		{
		}

	}

    /// <summary>
    /// This is the single xml registry instance for the app.
    /// </summary>
    public class XmlRegistryManager
    {
        // Static cache object, all invokers will use this cache object.
        private static XmlRegistry theRegistry;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private XmlRegistryManager()
        {
        }

        /// <summary>
        /// Gets the XmlRegistry instance.
        /// </summary>
        /// <returns> The XmlRegistry instance.</returns>
        static public XmlRegistry Instance
        {
            get
            {
                return theRegistry;
            }
        }

        static XmlRegistryManager()
        {
            // Initializing the registry.
            {
                string registryFilePath = GetServerRootDir();

				if (registryFilePath.Contains("EbaasServer"))
				{
					// debug env. remove \EbaasServer if exist
					int pos = registryFilePath.LastIndexOf(@"\EbaasServer");
					if (pos > 0)
					{
						registryFilePath = registryFilePath.Substring(0, pos);
					}

					if (!registryFilePath.EndsWith(@"\"))
					{
						registryFilePath += @"\";
					}

				}
				registryFilePath += @"Config\Registry.xml";

                theRegistry = new XmlRegistry(registryFilePath);
            }
        }

        public static string GetServerRootDir()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~");
            if (path == null)
            {
                var uriPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                path = new Uri(uriPath).LocalPath;

                // remove \bin if exist
                int pos = path.LastIndexOf(@"\bin");
                if (pos > 0)
                {
                    path = path.Substring(0, pos);
                }

                if (!path.EndsWith(@"\"))
                {
                    path += @"\";
                }
            }

            return path;
        }
    }
}
