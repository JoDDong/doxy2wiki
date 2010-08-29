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

using System.IO;
using System.Xml;

namespace MarcelJoachimKloubert.doxy2wiki.DoxyGen
{
    /// <summary>
    /// Stores data of a DoxyGen compound's member.
    /// </summary>
    public struct DoxyCompoundMemberParameter
    {
        #region Data Members (16)

        private string _description;
        private FileInfo _file;
        private bool? _hasThisKeyword;
        private string _name;
        private int _ordinal;
        private IDoxyType _paramType;
        private DoxyCompoundMember _parent;
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
        /// Gets if that parameter has the 'this' keyword as prefix (true) or not (false).
        /// </summary>
        public bool HasThisKeyword
        {
            get
            {
                if (this._hasThisKeyword.HasValue == false)
                {
                    IDoxyType paramType = this.ParameterType;
                }

                return this._hasThisKeyword.Value;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>
        /// Gets the position of that parameter.
        /// </summary>
        public int Ordinal
        {
            get { return this._ordinal; }
        }

        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        public IDoxyType ParameterType
        {
            get
            {
                if (this._paramType == null)
                {
                    this._hasThisKeyword = false;

                    XmlNode memberDefNode = this.Xml.ParentNode.ParentNode.ParentNode.ParentNode;
                    foreach (XmlNode paramNode in memberDefNode.SelectNodes("param"))
                    {
                        XmlNode nameNode = paramNode.SelectSingleNode("declname");
                        if (nameNode.InnerText != this.Name)
                        {
                            continue;
                        }

                        XmlNode typeNode = paramNode.SelectSingleNode("type");
                        string type = typeNode.InnerText;

                        XmlNode refNode = typeNode.SelectSingleNode("ref");
                        if (refNode != null)
                        {
                            this._paramType = this.Parent.Parent.Parent.GetCompoundById(refNode.Attributes["refid"].InnerText);
                        }

                        if (this._paramType == null)
                        {
                            if (type.Trim().StartsWith("this "))
                            {
                                this._hasThisKeyword = true;
                                type = type.Trim().Substring(5).Trim();
                            }

                            this._paramType = new DoxyUnknownType(type);
                        }

                        break;
                    }
                }

                return this._paramType;
            }
        }

        /// <summary>
        /// Gets the parent member.
        /// </summary>
        public DoxyCompoundMember Parent
        {
            get { return this._parent; }
        }

        /// <summary>
        /// Gets the underlying XML data.
        /// </summary>
        public XmlNode Xml
        {
            get { return this._xml; }
        }

        #endregion Data Members

        #region Methods (3)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="ordinal">The position of the parameter.</param>
        /// <param name="xml">The underlying XML data.</param>
        /// <param name="file">The underlying file.</param>
        public DoxyCompoundMemberParameter(DoxyCompoundMember parent, int ordinal, XmlNode xml, FileInfo file)
        {
            this._parent = parent;
            this._ordinal = ordinal;
            this._xml = xml;
            this._file = file;

            this._hasThisKeyword = null;
            this._paramType = null;
            this._name = null;
            this._description = null;

            this.InitMe();
        }

        /// <summary>
        /// Inits that value.
        /// </summary>
        private void InitMe()
        {
            // name
            XmlNode nameNode = this.Xml.SelectSingleNode("parameternamelist/parametername");
            this._name = nameNode.InnerText.Trim();

            // description
            XmlNode descNode = this.Xml.SelectSingleNode("parameterdescription/para");
            if (descNode != null)
            {
                this._description = descNode.InnerText.Trim();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="object.ToString()" />
        public override string ToString()
        {
            return this.Name;
        }

        #endregion Methods
    }
}
