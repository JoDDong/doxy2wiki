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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace MarcelJoachimKloubert.doxy2wiki.DoxyGen
{
    /// <summary>
    /// A DoxyGen project.
    /// </summary>
    public sealed class DoxyProject
    {
        #region Constructors (1)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectFile">The underlying project file.</param>
        /// <param name="name">The name of the project.</param>
        public DoxyProject(FileInfo projectFile, string name)
        {
            this.File = projectFile;
            this.Name = name;

            this.InitMe();
        }

        #endregion Constructors

        #region Properties (8)

        /// <summary>
        /// Gets all classes.
        /// </summary>
        public DoxyCompound[] Classes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all enums.
        /// </summary>
        public DoxyCompound[] Enums
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the underlying project file.
        /// </summary>
        public FileInfo File
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all interfaces.
        /// </summary>
        public DoxyCompound[] Interfaces
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of that project.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all value types.
        /// </summary>
        public DoxyCompound[] Structs
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns all compounds by their name.
        /// </summary>
        /// <param name="name">The name of the compounds.</param>
        /// <returns>The compounds.</returns>
        public DoxyCompound[] this[string name]
        {
            get { return this.GetCompoundsByName(name); }
        }

        /// <summary>
        /// Gets the XML data of that project.
        /// </summary>
        public XmlNode Xml
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods  (7)

        /// <summary>
        /// Appends a list of compounds to that project.
        /// </summary>
        /// <param name="compounds">The compunds to add.</param>
        internal void AppendCompounds(params DoxyCompound[] compounds)
        {
            this.AppendCompounds((IEnumerable<DoxyCompound>)compounds);
        }

        /// <summary>
        /// Appends a list of compounds to that project.
        /// </summary>
        /// <param name="compounds">The compunds to add.</param>
        internal void AppendCompounds(IEnumerable<DoxyCompound> compounds)
        {
            List<DoxyCompound> temp = null;

            // classes
            temp = new List<DoxyCompound>(this.Classes);
            temp.AddRange(compounds.Where(x => x.Type == DoxyCompoundType.Class));
            this.Classes = FilterAndSort(temp);

            // interfaces
            temp = new List<DoxyCompound>(this.Interfaces);
            temp.AddRange(compounds.Where(x => x.Type == DoxyCompoundType.Interface));
            this.Interfaces = FilterAndSort(temp);

            // structs
            temp = new List<DoxyCompound>(this.Structs);
            temp.AddRange(compounds.Where(x => x.Type == DoxyCompoundType.Struct));
            this.Structs = FilterAndSort(temp);

            // enums
            temp = new List<DoxyCompound>(this.Enums);
            temp.AddRange(compounds.Where(x => x.Type == DoxyCompoundType.Enum));
            this.Enums = FilterAndSort(temp);
        }

        /// <summary>
        /// Filters and sorts a sequence of compunds.
        /// </summary>
        /// <param name="compounds">The compounds to filter and sort.</param>
        /// <returns>The filtered and sorted list.</returns>
        private static DoxyCompound[] FilterAndSort(IEnumerable<DoxyCompound> compounds)
        {
            return compounds.OrderBy(x => x.FullName)
                            .Distinct(DoxyCompound.DoxyCompoundComparer.Instance)
                            .ToArray();
        }

        /// <summary>
        /// Returns a list of all known compounds.
        /// </summary>
        /// <returns>The list of all known compounds.</returns>
        public DoxyCompound[] GetAllCompounds()
        {
            return this.Classes
                       .Concat(this.Interfaces)
                       .Concat(this.Structs)
                       .Concat(this.Enums)
                       .OrderBy(x => x.FullName)
                       .ToArray();
        }

        /// <summary>
        /// Returns a compound by its ID.
        /// </summary>
        /// <param name="id">The ID to search for.</param>
        /// <returns>The compound or (null) if not found.</returns>
        public DoxyCompound GetCompoundById(string id)
        {
            // search all compounds
            foreach (DoxyCompound compound in this.GetAllCompounds())
            {
                if (compound.Id == id)
                {
                    return compound;
                }
            }

            // not found
            return null;
        }

        /// <summary>
        /// Returns all compounds by their name.
        /// </summary>
        /// <param name="name">The name of the compounds.</param>
        /// <returns>The compounds.</returns>
        public DoxyCompound[] GetCompoundsByName(string name)
        {
            return this.GetAllCompounds()
                       .Where(x => x.Name == name)
                       .ToArray();
        }

        /// <summary>
        /// Inits that object.
        /// </summary>
        private void InitMe()
        {
            this.Classes = new DoxyCompound[0];
            this.Interfaces = new DoxyCompound[0];
            this.Structs = new DoxyCompound[0];
            this.Enums = new DoxyCompound[0];

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.File.FullName);

            this.Xml = xmlDoc;

            List<DoxyCompound> compounds = new List<DoxyCompound>();
            foreach (XmlNode compoundNode in this.Xml.SelectNodes("//doxygenindex/compound"))
            {
                string kind = null;
                XmlAttribute kindAttrib = compoundNode.Attributes["kind"];
                if (kindAttrib != null)
                {
                    kind = kindAttrib.InnerText;
                }

                switch (kind)
                {
                    case "namespace":
                        {
                            string enumMemberXPath = "member[@kind=\"enum\"]";
                            foreach (XmlNode enumMemberNode in compoundNode.SelectNodes(enumMemberXPath))
                            {
                                string enumId = enumMemberNode.Attributes["refid"].InnerText;

                                string compoundId = enumMemberNode.ParentNode.Attributes["refid"].InnerText;
                                FileInfo enumFile = new FileInfo(Path.Combine(this.File.Directory.FullName, compoundId + ".xml"));

                                XmlDocument compoundDoc = new XmlDocument();
                                compoundDoc.Load(enumFile.FullName);

                                string enumXPath = string.Format("//doxygen/compounddef[@id=\"{0}\"]/sectiondef[@kind=\"enum\"]/memberdef[@id=\"{1}\"]", compoundId, enumId);
                                foreach (XmlNode enumNode in compoundDoc.SelectNodes(enumXPath))
                                {
                                    DoxyCompound newCompound = new DoxyCompound(this, enumNode, new FileInfo(enumFile.FullName));
                                    compounds.Add(newCompound);
                                }
                            }
                        }
                        break;

                    default:
                        {
                            string refId = compoundNode.Attributes["refid"].InnerText;
                            FileInfo file = new FileInfo(Path.Combine(this.File.Directory.FullName, refId + ".xml"));

                            DoxyCompound newCompound = new DoxyCompound(this, compoundNode, file);
                            compounds.Add(newCompound);
                        }
                        break;
                }
            }

            this.AppendCompounds(compounds.Where(x => x.Type != DoxyCompoundType.Unknown));
        }

        #endregion Methods
    }
}
