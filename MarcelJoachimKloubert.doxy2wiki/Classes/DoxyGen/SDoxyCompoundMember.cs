// $Id: SDoxyCompoundMember.cs 21 2010-08-29 22:43:29Z mkloubert $

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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace MarcelJoachimKloubert.doxy2wiki.DoxyGen
{
    /// <summary>
    /// Stores data of a DoxyGen compound's member.
    /// </summary>
    public struct DoxyCompoundMember
    {
        #region Data Members (28)

        private string _description;
        private DoxyExample<DoxyCompoundMember>[] _examples;
        private FileInfo _file;
        private bool _isExplicit;
        private bool _isStatic;
        private string _name;
        private DoxyCompoundMemberParameter[] _params;
        private object _parent;
        private IDoxyType _resultType;
        private DoxyCompoundMemberType _type;
        private DoxyVisibility _visibility;
        private XmlNode _xml;
        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get { return this._description; }
        }

        /// <summary>
        /// Gets the underlying file.
        /// </summary>
        public FileInfo File
        {
            get { return this._file; }
        }

        /// <summary>
        /// Gets if that member is a constructor (true) or not (false).
        /// </summary>
        public bool IsConstructor
        {
            get
            {
                return this.Type == DoxyCompoundMemberType.Function &&
                       this.Name == this.Parent.Name;
            }
        }

        /// <summary>
        /// Gets if that member is explicit (true) or not (false).
        /// </summary>
        public bool IsExplicit
        {
            get { return this._isExplicit; }
        }

        /// <summary>
        /// Gets if that compound represents an extension method (true) or not (false).
        /// </summary>
        public bool IsExtensionMethod
        {
            get
            {
                return this.Type == DoxyCompoundMemberType.Function &&
                       this.Parameters.LongLength > 0 &&
                       this.Parameters[0].HasThisKeyword;
            }
        }

        /// <summary>
        /// Gets if that member represents an operator (true) or not (false).
        /// </summary>
        public bool IsOperator
        {
            get
            {
                return this.Type == DoxyCompoundMemberType.Function &&
                       this._name.StartsWith("operator");
            }
        }

        /// <summary>
        /// Gets if that member is static (true) or not (false).
        /// </summary>
        public bool IsStatic
        {
            get { return this._isStatic; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                string name = this._name;
                if (this.IsOperator)
                {
                    name = string.Concat(name.Skip(8).Cast<object>().ToArray());
                }

                return name.Trim();
            }
        }

        /// <summary>
        /// Gets the parameters of that member.
        /// </summary>
        public DoxyCompoundMemberParameter[] Parameters
        {
            get { return this._params; }
        }

        /// <summary>
        /// Gets the parent object.
        /// </summary>
        public DoxyCompound Parent
        {
            get { return (DoxyCompound)this._parent; }
        }

        /// <summary>
        /// Gets the result type.
        /// </summary>
        public IDoxyType ResultType
        {
            get
            {
                // result type
                if (this._resultType == null)
                {
                    XmlNode resultTypeNode = this.Xml.SelectSingleNode("type");

                    XmlNode refNode = resultTypeNode.SelectSingleNode("ref");
                    if (refNode != null)
                    {
                        this._resultType = this.Parent.Parent.GetCompoundById(refNode.Attributes["refid"].InnerText);
                    }

                    if (this._resultType == null)
                    {
                        this._resultType = new DoxyUnknownType(resultTypeNode.InnerText);
                    }
                }

                return this._resultType;
            }
        }

        /// <summary>
        /// Returns a parameter by its name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The parameter.</returns>
        public DoxyCompoundMemberParameter this[string name]
        {
            get { return this.GetParameterByName(name).Value; }
        }

        /// <summary>
        /// Returns a parameter by its ordinal value.
        /// </summary>
        /// <param name="ordinal">The ordinal value of the parameter.</param>
        /// <returns>The parameter.</returns>
        public DoxyCompoundMemberParameter this[int ordinal]
        {
            get { return this.Parameters[ordinal]; }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public DoxyCompoundMemberType Type
        {
            get { return this._type; }
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

        #endregion Data Members

        #region Methods (5)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="xml">The underlying XML data.</param>
        /// <param name="file">The underlying file.</param>
        public DoxyCompoundMember(DoxyCompound parent, XmlNode xml, FileInfo file)
        {
            this._parent = parent;
            this._xml = xml;
            this._file = file;

            this._name = null;
            this._description = null;
            this._type = DoxyCompoundMemberType.Unknown;
            this._visibility = DoxyVisibility.Unknown;
            this._isStatic = false;
            this._isExplicit = false;
            this._params = null;
            this._resultType = null;
            this._examples = null;

            this.InitMe();
        }

        /// <summary>
        /// Returns a parameter by its name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The parameter or (null) if not found.</returns>
        public DoxyCompoundMemberParameter? GetParameterByName(string name)
        {
            return this.Parameters
                       .Cast<DoxyCompoundMemberParameter?>()
                       .LastOrDefault(x => x.Value.Name == name);
        }

        /// <summary>
        /// Inits that value.
        /// </summary>
        private void InitMe()
        {
            XmlNode nameNode = this.Xml.SelectSingleNode("name");
            this._name = nameNode.InnerText;

            XmlAttribute staticAttrib = this.Xml.Attributes["static"];
            this._isStatic = staticAttrib.InnerText == "yes";

            XmlAttribute explicitAttrib = this.Xml.Attributes["explicit"];
            if (explicitAttrib != null)
            {
                this._isExplicit = explicitAttrib.InnerText == "yes";
            }

            // visibility
            XmlAttribute visibilityAttrib = this.Xml.Attributes["prot"];
            if (visibilityAttrib != null && TMHelpers.TryParseEnum<DoxyVisibility>(visibilityAttrib.InnerText, out this._visibility, true) == false)
            {
                this._visibility = DoxyVisibility.Unknown;
            }

            XmlNode descriptionNode = this.Xml.SelectSingleNode("briefdescription/para");
            if (descriptionNode != null && descriptionNode.InnerText.IsNullOrWhitespace() == false)
            {
                this._description = descriptionNode.InnerText.Trim();
            }

            List<DoxyCompoundMemberParameter> @params = new List<DoxyCompoundMemberParameter>();
            {
                int i = 0;
                foreach (XmlNode paramNode in this.Xml.SelectNodes("detaileddescription/para/parameterlist[@kind=\"param\"]/parameteritem"))
                {
                    DoxyCompoundMemberParameter newParam = new DoxyCompoundMemberParameter(this, i++, paramNode, new FileInfo(this.File.FullName));
                    @params.Add(newParam);
                }

                this._params = @params.OrderBy(x => x.Ordinal)
                                      .ToArray();
            }

            // examples
            List<DoxyExample<DoxyCompoundMember>> examples = new List<DoxyExample<DoxyCompoundMember>>();
            foreach (XmlNode programlistingNode in this.Xml.SelectNodes("detaileddescription/para/programlisting"))
            {
                DoxyExample<DoxyCompoundMember> newExample = new DoxyExample<DoxyCompoundMember>(
                    this,
                    programlistingNode,
                    new FileInfo(this.File.FullName));

                examples.Add(newExample);
            }
            this._examples = examples.ToArray();

            // type
            if (TMHelpers.TryParseEnum<DoxyCompoundMemberType>(this.Xml.Attributes["kind"].InnerText, out this._type, true) == false)
            {
                this._type = DoxyCompoundMemberType.Unknown;
            }
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
            return this.Parent.ToString(separator) + separator + this.Name;
        }

        #endregion Methods
        /// <summary>
        /// Gets all examples.
        /// </summary>
        public DoxyExample<DoxyCompoundMember>[] Examples
        {
            get { return this._examples; }
        }
    }
}
