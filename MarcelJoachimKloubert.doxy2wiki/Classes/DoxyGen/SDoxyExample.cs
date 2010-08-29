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
    /// Stores data of an example.
    /// </summary>
    /// <typeparam name="TParent">Type of the parent object.</typeparam>
    public struct DoxyExample<TParent>
    {
        #region Data Members (8)

        private string _code;
        private FileInfo _file;
        private TParent _parent;
        private XmlNode _xml;
        /// <summary>
        /// Gets the code that example.
        /// </summary>
        public string Code
        {
            get { return this._code; }
        }

        /// <summary>
        /// Get the underlying file.
        /// </summary>
        public FileInfo File
        {
            get { return this._file; }
        }

        /// <summary>
        /// Gets the parent object.
        /// </summary>
        public TParent Parent
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

        #region Methods (5)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="xml">The underlying XML data.</param>
        /// <param name="file">The underlying file.</param>
        public DoxyExample(TParent parent, XmlNode xml, FileInfo file)
        {
            this._parent = parent;
            this._xml = xml;
            this._file = file;
            this._code = null;

            this.InitMe();
        }

        /// <summary>
        /// Inits that value.
        /// </summary>
        private void InitMe()
        {
            this._code = ParseCodelines(this.Xml.SelectNodes("codeline"));
        }

        private static string ParseCodeline(XmlNode line)
        {
            string result = string.Empty;

            if (line.HasChildNodes)
            {
                foreach (XmlNode childNode in line.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "sp":
                            result += " ";
                            break;

                        default:
                            result += ParseCodeline(childNode);
                            break;
                    }
                }
            }
            else
            {
                result = line.InnerText;
            }

            return result;
        }

        private static string ParseCodelines(XmlNodeList codeLines)
        {
            string result = string.Empty;
            foreach (XmlNode line in codeLines)
            {
                result += string.Format("{0}\n", ParseCodeline(line));
            }

            return TrimCode(result);
        }

        private static string TrimCode(string code)
        {
            string[] lines = code.Split(new string[] { "\n" }, StringSplitOptions.None);

            int spaceCount = lines.Select(x => x.Length)
                                  .Max();

            foreach (string line in lines)
            {
                spaceCount = Math.Min(
                    spaceCount,
                    line.SkipWhile(x => char.IsWhiteSpace(x))
                        .Count());
            }

            IEnumerable<string> parsedLines = lines.Select(
                x =>
                {
                    return string.Concat(x.Skip(spaceCount)
                                          .Cast<object>()
                                          .ToArray()).TrimEnd();
                });

            return parsedLines.JoinAsString("\n");
        }

        #endregion Methods
    }
}
