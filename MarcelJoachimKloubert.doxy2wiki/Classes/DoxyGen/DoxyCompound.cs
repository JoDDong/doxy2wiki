// $Id$

// doxy2wiki - Command line tool to convert DoxyGen XML documents to a known wiki engine import format.
// Copyright (C) 2010 Marcel Joachim Kloubert
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace MarcelJoachimKloubert.doxy2wiki.DoxyGen
{
    /// <summary>
    /// Stores data of a DoxyGen compound.
    /// </summary>
    public sealed class DoxyCompound : IEnumerable<DoxyCompoundMember>, IEquatable<DoxyCompound>, IDoxyType
    {
        #region Fields (18)

        private DoxyCompound[] _childCompounds;
        private DoxyCompoundMember[] _constructors;
        private string _description;
        private DoxyExample<DoxyCompound>[] _examples;
        private DoxyCompoundMember[] _fields;
        private FileInfo _file;
        private string _id;
        private DoxyCompoundMember[] _methods;
        private string _name;
        private DoxyCompoundNamespace _namespace;
        private DoxyCompoundMember[] _operators;
        private DoxyProject _parent;
        private object _parentCompound;
        private DoxyCompoundMember[] _properties;
        private DoxyCompoundType _type;
        private DoxyCompoundValue[] _values;
        private DoxyVisibility _visibility;
        private XmlNode _xml;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="xml">The XML data.</param>
        /// <param name="file">The underlying file.</param>
        /// <param name="parentCompound">The parent compound.</param>
        public DoxyCompound(DoxyProject parent, XmlNode xml, FileInfo file, DoxyCompound parentCompound = null)
        {
            this._parent = parent;
            this._xml = xml;
            this._file = file;
            this._parentCompound = parentCompound;

            this._id = null;
            this._namespace = null;
            this._name = null;
            this._type = DoxyCompoundType.Unknown;
            this._visibility = DoxyVisibility.Unknown;
            this._description = null;

            this._examples = new DoxyExample<DoxyCompound>[0];
            this._childCompounds = new DoxyCompound[0];
            this._constructors = new DoxyCompoundMember[0];
            this._properties = new DoxyCompoundMember[0];
            this._methods = new DoxyCompoundMember[0];
            this._fields = new DoxyCompoundMember[0];
            this._operators = new DoxyCompoundMember[0];

            this._values = new DoxyCompoundValue[0];

            this.InitMe();
        }

        #endregion Constructors

        #region Properties (19)

        /// <summary>
        /// Gets the list of child compounds.
        /// </summary>
        public DoxyCompound[] ChildCompounds
        {
            get { return this._childCompounds; }
        }

        /// <summary>
        /// Gets all constructors.
        /// </summary>
        public DoxyCompoundMember[] Constructors
        {
            get { return this._constructors; }
        }

        /// <summary>
        /// Gets the description text for that compound.
        /// </summary>
        public string Description
        {
            get { return this._description; }
        }

        /// <summary>
        /// Gets all fields.
        /// </summary>
        public DoxyCompoundMember[] Fields
        {
            get { return this._fields; }
        }

        /// <summary>
        /// Gets the underlying file.
        /// </summary>
        public FileInfo File
        {
            get { return this._file; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IDoxyType.FullName" />
        public string FullName
        {
            get { return this.ToString(); }
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public string Id
        {
            get { return this._id; }
        }

        /// <summary>
        /// Gets all methods.
        /// </summary>
        public DoxyCompoundMember[] Methods
        {
            get { return this._methods; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IDoxyType.Name" />
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        public DoxyCompoundNamespace Namespace
        {
            get { return this._namespace; }
        }

        /// <summary>
        /// Gets all operators.
        /// </summary>
        public DoxyCompoundMember[] Operators
        {
            get { return this._operators; }
        }

        /// <summary>
        /// Gets the parent object.
        /// </summary>
        public DoxyProject Parent
        {
            get { return this._parent; }
        }

        /// <summary>
        /// Gets the parent compund.
        /// </summary>
        public DoxyCompound ParentCompound
        {
            get { return (DoxyCompound)this._parentCompound; }
        }

        /// <summary>
        /// Gets all properties.
        /// </summary>
        public DoxyCompoundMember[] Properties
        {
            get { return this._properties; }
        }

        /// <summary>
        /// Returns compound members by their name.
        /// </summary>
        /// <param name="name">The name of the members.</param>
        /// <returns>The members.</returns>
        public DoxyCompoundMember[] this[string name]
        {
            get { return this.GetMembersByName(name); }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public DoxyCompoundType Type
        {
            get { return this._type; }
        }

        /// <summary>
        /// Gets all values.
        /// </summary>
        public DoxyCompoundValue[] Values
        {
            get { return this._values; }
        }

        /// <summary>
        /// Gets the visibility.
        /// </summary>
        public DoxyVisibility Visibility
        {
            get { return this._visibility; }
        }

        /// <summary>
        /// Gets the underlying XML data.
        /// </summary>
        public XmlNode Xml
        {
            get { return this._xml; }
        }

        #endregion Properties

        #region Methods  (13)

        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Are equal (false) or not (true).</returns>
        public static bool operator !=(DoxyCompound left, DoxyCompound right)
        {
            return (left == right) == false;
        }

        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Are equal (true) or not (false).</returns>
        public static bool operator ==(DoxyCompound left, DoxyCompound right)
        {
            // the object cast is to prevent a StackOverflowException
            if ((object)left != null)
            {
                return left.Equals(right);
            }

            // the object cast is to prevent a StackOverflowException
            if ((object)right != null)
            {
                return right.Equals(left);
            }

            // both are (null)
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            if (obj is DoxyCompound)
            {
                return this.Equals((DoxyCompound)obj);
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IEquatable&lt;T&gt;.Equals(T)" />
        public bool Equals(DoxyCompound other)
        {
            if (other == null)
            {
                return false;
            }

            return object.Equals(this.Parent, other.Parent) &&
                   this.FullName == other.FullName;
        }

        /// <summary>
        /// Gets all members of that compound.
        /// </summary>
        /// <returns>The list of members of that compound.</returns>
        public DoxyCompoundMember[] GetAllMembers()
        {
            return this.Constructors
                       .Concat(this.Methods)
                       .Concat(this.Fields)
                       .Concat(this.Properties)
                       .OrderBy(x => x.Name)
                       .ThenBy(x => x.Type)
                       .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IEnumerable&lt;T&gt;.GetEnumerator()" />
        public IEnumerator<DoxyCompoundMember> GetEnumerator()
        {
            return this.GetAllMembers()
                       .Cast<DoxyCompoundMember>()
                       .GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="object.GetHashCode()" />
        public override int GetHashCode()
        {
            return this.FullName == null ? 0 : this.FullName.GetHashCode();
        }

        /// <summary>
        /// Returns compound members by their name.
        /// </summary>
        /// <param name="name">The name of the members.</param>
        /// <returns>The members.</returns>
        public DoxyCompoundMember[] GetMembersByName(string name)
        {
            return this.GetAllMembers()
                       .Where(x => x.Name == name)
                       .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IEnumerable&lt;T&gt;.GetEnumerator()" />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Inits that value.
        /// </summary>
        private void InitMe()
        {
            if (this.Xml.Name == "memberdef")
            {
                this.InitMeEnum();
                return;
            }

            XmlNode nameNode = null;
            XmlNode kindAttrib = null;

            this._namespace = null;
            this._name = null;

            if (this.Xml.Name == "compound")
            {
                nameNode = this.Xml.SelectNodes("name")[0];
                kindAttrib = this.Xml.Attributes["kind"];

                this._id = this.Xml.Attributes["refid"].InnerText;
            }
            else if (this.Xml.Name == "compounddef")
            {
                nameNode = this.Xml.SelectNodes("compoundname")[0];
                kindAttrib = this.Xml.Attributes["kind"];

                this._id = this.Xml.Attributes["id"].InnerText;
            }

            if (nameNode.InnerText.Contains("::"))
            {
                IEnumerable<string> parts = nameNode.InnerText.Split(new string[] { "::" }, StringSplitOptions.None)
                                                              .Where(x => x.Trim() != string.Empty)
                                                              .Select(x => x.Trim());

                this._namespace = new DoxyCompoundNamespace(this, parts.Take(parts.Count() - 1));
                this._name = string.Concat(parts.Skip(parts.Count() - 1).Cast<object>().ToArray());
            }
            else
            {
                if (nameNode.InnerText.IsNullOrWhitespace() == false)
                {
                    this._name = nameNode.InnerText.Trim();
                }
            }

            if (TMHelpers.TryParseEnum<DoxyCompoundType>(kindAttrib.InnerText, out this._type, true) == false)
            {
                this._type = DoxyCompoundType.Unknown;
            }

            XmlDocument compoundDoc = new XmlDocument();
            compoundDoc.Load(this.File.FullName);

            // examples
            string exampleXPath = string.Format("//doxygen/compounddef[@id=\"{0}\"]/detaileddescription/para/programlisting", this.Id);
            List<DoxyExample<DoxyCompound>> examples = new List<DoxyExample<DoxyCompound>>();
            foreach (XmlNode programlistingNode in compoundDoc.SelectNodes(exampleXPath))
            {
                DoxyExample<DoxyCompound> newExample = new DoxyExample<DoxyCompound>(
                    this,
                    programlistingNode,
                    new FileInfo(this.File.FullName));

                examples.Add(newExample);
            }
            this._examples = examples.ToArray();

            // description
            string compoundDescXPath = string.Format("//doxygen/compounddef[@id=\"{0}\"]/briefdescription/para", this.Id);
            XmlNode descriptionNode = compoundDoc.SelectSingleNode(compoundDescXPath);
            if (descriptionNode != null && descriptionNode.InnerText.IsNullOrWhitespace() == false)
            {
                this._description = descriptionNode.InnerText.Trim();
            }

            // visibility
            XmlAttribute visibilityAttrib = compoundDoc.SelectSingleNode(string.Format("//doxygen/compounddef[@id=\"{0}\"]", this.Id)).Attributes["prot"];
            if (visibilityAttrib != null && TMHelpers.TryParseEnum<DoxyVisibility>(visibilityAttrib.InnerText, out this._visibility, true) == false)
            {
                this._visibility = DoxyVisibility.Unknown;
            }

            // members
            List<DoxyCompoundMember> members = new List<DoxyCompoundMember>();
            {
                string memberXPath = string.Format("//doxygen/compounddef[@id=\"{0}\"]/sectiondef/memberdef", this.Id);
                foreach (XmlNode memberNode in compoundDoc.SelectNodes(memberXPath))
                {
                    DoxyCompoundMember newMember = new DoxyCompoundMember(this, memberNode, new FileInfo(this.File.FullName));
                    members.Add(newMember);
                }

                members = new List<DoxyCompoundMember>(members.Where(x => x.Type != DoxyCompoundMemberType.Unknown)
                                                              .OrderBy(x => x.Name));
                // collect data
                {
                    this._constructors = members.Where(x => x.IsConstructor).ToArray();

                    this._methods = members.Where(x => x.Type == DoxyCompoundMemberType.Function &&
                                                       x.IsOperator == false &&
                                                       x.IsConstructor == false).ToArray();

                    this._operators = members.Where(x => x.IsOperator).ToArray();

                    this._properties = members.Where(x => x.Type == DoxyCompoundMemberType.Property).ToArray();

                    this._fields = members.Where(x => x.Type == DoxyCompoundMemberType.Variable).ToArray();
                }
            }

            // sub compunds
            List<DoxyCompound> subCompounds = new List<DoxyCompound>();
            {
                string innerclassXPath = string.Format("//doxygen/compounddef[@id=\"{0}\"]/innerclass", this.Id);
                foreach (XmlNode innerclassNode in compoundDoc.SelectNodes(innerclassXPath))
                {
                    XmlAttribute refAttrib = innerclassNode.Attributes["refid"];
                    string subCompRef = refAttrib.InnerText.Trim();

                    FileInfo subCompundFile = new FileInfo(Path.Combine(this.File.Directory.FullName, subCompRef + ".xml"));

                    XmlDocument subCompundDoc = new XmlDocument();
                    subCompundDoc.Load(subCompundFile.FullName);

                    string subCompXPath = string.Format("//doxygen/compounddef[@id=\"{0}\"]", subCompRef);
                    DoxyCompound subCompound = new DoxyCompound(
                        this.Parent,
                        subCompundDoc.SelectSingleNode(subCompXPath),
                        subCompundFile,
                        this);

                    subCompounds.Add(subCompound);
                }

                this._childCompounds = subCompounds.OrderBy(x => x.FullName).ToArray();
                this.Parent.AppendCompounds(this.ChildCompounds);
            }
        }

        /// <summary>
        /// Inits that value for an enumeration compound.
        /// </summary>
        private void InitMeEnum()
        {
            XmlNode nameNode = this.Xml.SelectNodes("name")[0];
            XmlAttribute kindAttrib = this.Xml.Attributes["kind"];

            this._namespace = null;
            this._name = nameNode.InnerText;
            this._id = this.Xml.Attributes["id"].InnerText;

            if (TMHelpers.TryParseEnum<DoxyCompoundType>(kindAttrib.InnerText, out this._type, true) == false)
            {
                this._type = DoxyCompoundType.Unknown;
            }

            string @namespace = this.Xml.ParentNode.ParentNode.SelectSingleNode("compoundname").InnerText;
            if (@namespace.IsNullOrWhitespace() == false)
            {
                IEnumerable<string> parts = @namespace.Split(new string[] { "::" }, StringSplitOptions.None)
                                                      .Where(x => x.Trim() != string.Empty)
                                                      .Select(x => x.Trim());

                this._namespace = new DoxyCompoundNamespace(this, parts);
            }

            // values
            List<DoxyCompoundValue> values = new List<DoxyCompoundValue>();
            {
                int valueOrdinal = 0;

                string enumValueXPath = string.Format("enumvalue[@id=\"{0}\"]", this.Id);
                foreach (XmlNode enumValueNode in this.Xml.SelectNodes(enumValueXPath))
                {
                    DoxyCompoundValue newValue = new DoxyCompoundValue(
                        this,
                        valueOrdinal++,
                        enumValueNode,
                        new FileInfo(this.File.FullName));

                    values.Add(newValue);
                }
            }
            this._values = values.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="object.ToString()" />
        public override string ToString()
        {
            return this.ToString(".");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToString(string separator)
        {
            if (this.Namespace == null)
            {
                return this.Name;
            }

            return this.Namespace.Parts
                                 .Concat(new object[] { this.Name })
                                 .JoinAsString(separator);
        }

        #endregion Methods

        #region Nested Classes (1)


        /// <summary>
        /// A comparer for DoxyCompound values.
        /// </summary>
        public sealed class DoxyCompoundComparer : IEqualityComparer<DoxyCompound>
        {
            #region Fields (1)

            private static DoxyCompoundComparer _instance = null;

            #endregion Fields

            #region Constructors (1)

            /// <summary>
            /// 
            /// </summary>
            private DoxyCompoundComparer()
            {

            }

            #endregion Constructors

            #region Properties (1)

            /// <summary>
            /// Gets the singleton instance.
            /// </summary>
            public static DoxyCompoundComparer Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new DoxyCompoundComparer();
                    }

                    return _instance;
                }
            }

            #endregion Properties

            #region Methods  (2)

            /// <summary>
            /// 
            /// </summary>
            /// <see cref="IEqualityComparer&lt;T&gt;.Equals(T, T)" />
            public bool Equals(DoxyCompound x, DoxyCompound y)
            {
                return x == y;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <see cref="IEqualityComparer&lt;T&gt;.GetHashCode(T)" />
            public int GetHashCode(DoxyCompound obj)
            {
                return obj != null ? obj.GetHashCode() : 0;
            }

            #endregion Methods
        }
        #endregion Nested Classes
        /// <summary>
        /// Gets all examples of that compound.
        /// </summary>
        public DoxyExample<DoxyCompound>[] Examples
        {
            get { return this._examples; }
        }
    }
}
