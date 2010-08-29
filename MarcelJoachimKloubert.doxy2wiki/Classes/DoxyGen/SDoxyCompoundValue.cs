// $Id: SDoxyCompoundValue.cs 21 2010-08-29 22:43:29Z mkloubert $

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
    /// Stores data for a compound's value.
    /// </summary>
    public struct DoxyCompoundValue
    {
        #region Data Members (15)

        private string _description;
        private FileInfo _file;
        private string _name;
        private int _ordinal;
        private DoxyCompound _parent;
        private string _value;
        private XmlNode _xml;
        /// <summary>
        /// Gets the description text.
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
        /// Gets the fullname.
        /// </summary>
        public string FullName
        {
            get { return this.ToString(); }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>
        /// Gets the ordinal value.
        /// </summary>
        public int Ordinal
        {
            get { return this._ordinal; }
        }

        /// <summary>
        /// Gets the parent compound.
        /// </summary>
        public DoxyCompound Parent
        {
            get { return this._parent; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value
        {
            get { return this._value; }
        }

        /// <summary>
        /// Gets the underlying XML data.
        /// </summary>
        public XmlNode Xml
        {
            get { return this._xml; }
        }

        #endregion Data Members

        #region Methods (4)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent">The parent compound.</param>
        /// <param name="ordinal">The ordinal value.</param>
        /// <param name="xml">The underlying XML data.</param>
        /// <param name="file">The underlying file.</param>
        public DoxyCompoundValue(DoxyCompound parent, int ordinal, XmlNode xml, FileInfo file)
        {
            this._parent = parent;
            this._ordinal = ordinal;
            this._xml = xml;
            this._file = file;

            this._name = null;
            this._value = null;
            this._description = null;

            this.InitMe();
        }

        /// <summary>
        /// Inits that value.
        /// </summary>
        private void InitMe()
        {
            XmlNode nameNode = this.Xml.SelectSingleNode("name");
            this._name = nameNode.InnerText.Trim();

            XmlNode valueNode = this.Xml.SelectSingleNode("initializer");
            if (valueNode != null)
            {
                this._value = valueNode.InnerText.Trim();
            }

            XmlNode descNode = this.Xml.SelectSingleNode("briefdescription/para");
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
            return this.ToString(".");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToString(string separator)
        {
            return this.Parent.FullName + separator + this.Name;
        }

        #endregion Methods
    }
}
