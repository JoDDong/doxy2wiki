// $Id: MarcelJoachimKloubert.doxy2wiki.DoxyGen.cs 21 2010-08-29 22:43:29Z mkloubert $

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
using System.Linq;
using System.Xml;
using MarcelJoachimKloubert.doxy2wiki.Wikis.MediaWiki;

namespace MarcelJoachimKloubert.doxy2wiki.DoxyGen
{
    #region MediaWiki
    /// <summary>
    /// Extension methods MarcelJoachimKloubert.doxy2wiki.DoxyGen namespace.
    /// </summary>
    public static partial class TMExtensionsMarcelJoachimKloubert_doxy2wiki_DoxyGen
    {
        #region Methods  (6)

        /// <summary>
        /// Creates a wiki page for a DoxyGen project.
        /// </summary>
        /// <param name="proj">The project.</param>
        /// <param name="rootNode">The root node.</param>
        /// <param name="timestamp">The timestamp to use. If value is (null), DateTime.Now is used.</param>
        public static void CreateMediaWikiPage(this DoxyProject proj, XmlNode rootNode, DateTime? timestamp = null)
        {
            DateTime time = timestamp.HasValue ? timestamp.Value : DateTime.Now;

            MediaWikiPage newPage = MediaWikiPage.Create(time);
            newPage.Title = proj.GetProjectMediaWikiPath();
            newPage.Author = "doxy2wiki";
            newPage.Comment = "Import.";

            IEnumerable<DoxyCompoundNamespace?> namespaces = proj.GetAllCompounds()
                                                                 .Select(x => x.Namespace)
                                                                 .Distinct(DoxyCompoundNamespace.DoxyCompoundNamespaceComparer.Instance)
                                                                 .OrderBy(x => x.HasValue ? x.Value.FullName : (string)null);

            List<string> namespaceChapters = new List<string>();
            foreach (DoxyCompoundNamespace? @namespace in namespaces)
            {
                string compoundList = string.Empty;
                foreach (DoxyCompound compound in proj.GetAllCompounds()
                                                      .Where(x => x.Namespace == @namespace))
                {
                    compoundList += string.Format(
                        "* [[{0}|{1}]]{2}",
                        compound.GetCompoundMediaWikiPath(),
                        compound.Name,
                        Environment.NewLine);

                    compound.CreateMediaWikiPage(rootNode, time);
                }

                string chapter = string.Format(@"=== {0} ===

{1}

", @namespace.HasValue ? @namespace.Value.FullName : "(none)"
 , compoundList);

                namespaceChapters.Add(chapter);
            }

            string content = string.Format(
                @"
== {0} ==

{1}

", proj.Name
 , namespaceChapters.JoinAsString(Environment.NewLine));

            newPage.Content = content;
            newPage.ApplyToXml(rootNode);
        }

        /// <summary>
        /// Creates a wiki page for a compound.
        /// </summary>
        /// <param name="compound">The compound.</param>
        /// <param name="rootNode">The root node.</param>
        /// <param name="timestamp">The timestamp to use. If value is (null), DateTime.Now is used.</param>
        public static void CreateMediaWikiPage(this DoxyCompound compound, XmlNode rootNode, DateTime? timestamp = null)
        {
            DateTime time = timestamp.HasValue ? timestamp.Value : DateTime.Now;

            MediaWikiPage newPage = MediaWikiPage.Create(time);
            newPage.Title = compound.GetCompoundMediaWikiPath();
            newPage.Author = "doxy2wiki";
            newPage.Comment = "Import.";

            newPage.Content = string.Format(@"
{0}
{1}
{2}
{3}
", compound.Constructors.CreateMediaWikiPageChapter("Constructors")
 , compound.Fields.CreateMediaWikiPageChapter("Fields")
 , compound.Methods.CreateMediaWikiPageChapter("Methods")
 , compound.Properties.CreateMediaWikiPageChapter("Properties"));

            newPage.ApplyToXml(rootNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="members"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string CreateMediaWikiPageChapter(this IEnumerable<DoxyCompoundMember> members, string title)
        {
            string result = string.Format("=== {0} ==={1}{1}", title, Environment.NewLine);

            if (members.LongCount() < 1)
            {
                result += string.Format("''(none)''{0}{0}", Environment.NewLine);
            }
            else
            {
                foreach (DoxyCompoundMember member in members.OrderBy(x => x.Name))
                {
                    string @params = string.Empty;

                    string memberName = member.Name;
                    switch (member.Type)
                    {
                        case DoxyCompoundMemberType.Function:
                            {
                                memberName = string.Format(
                                    "{0} ({1})",
                                    member.Name,
                                    member.JoinParamsAsString());

                                if (member.Parameters.LongLength < 1)
                                {
                                    @params = "''(none)''";
                                }
                                else
                                {
                                    @params = @"===== Parameters =====

{| class=""prettytable""

|- 
!&nbsp;'''#'''&nbsp;
!&nbsp;'''Name'''&nbsp;
!&nbsp;'''Type'''&nbsp;
!&nbsp;'''Description'''&nbsp;
";

                                    int ordinal = 0;
                                    foreach (DoxyCompoundMemberParameter param in member.Parameters)
                                    {
                                        string paramType = "???";
                                        if (param.ParameterType != null)
                                        {
                                            if (param.ParameterType is DoxyCompound)
                                            {
                                                DoxyCompound paramCompound = (DoxyCompound)param.ParameterType;
                                                paramType = string.Format(
                                                    "[[{0}|{1}]]",
                                                    paramCompound.GetCompoundMediaWikiPath(),
                                                    paramCompound.Name);
                                            }
                                            else
                                            {
                                                paramType = param.ParameterType.FullName;
                                            }
                                        }

                                        @params += string.Format(@"

|-
|&nbsp;{0}&nbsp;
|&nbsp;{1}&nbsp;
|&nbsp;{2}&nbsp;
|&nbsp;{3}&nbsp;
", ++ordinal
 , param.Name
 , paramType
 , param.Description);
                                    }

                                    @params += @"
|}";
                                }

                                @params += string.Format(
                                    "{0}{0}",
                                    Environment.NewLine);
                            }
                            break;
                    }

                    string desc = "''(none)''";
                    if (member.Description.IsNullOrWhitespace() == false)
                    {
                        desc = member.Description;
                    }

                    result += string.Format(
                        "==== {0} ===={1}{1}{2}{1}{1}{3}",
                        memberName,
                        Environment.NewLine,
                        desc,
                        @params);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compound"></param>
        /// <returns></returns>
        public static string GetCompoundMediaWikiPath(this DoxyCompound compound)
        {
            return string.Format(
                "{0}/{1}/{2}",
                compound.Parent.GetProjectMediaWikiPath(),
                compound.Namespace.HasValue ? compound.Namespace.Value.FullName : "(none)",
                compound.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public static string GetProjectMediaWikiPath(this DoxyProject proj)
        {
            return string.Format("Documentation/CSharp/{0}", proj.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string JoinParamsAsString(this DoxyCompoundMember member)
        {
            string result = string.Empty;

            int i = 0;
            foreach (DoxyCompoundMemberParameter param in member.Parameters)
            {
                result += i++ > 0 ? ", " : string.Empty;

                result += string.Format(
                    "{0} {1}",
                    param.ParameterType == null ? "???" : param.ParameterType.FullName, param.Name);
            }

            return result;
        }

        #endregion Methods
    }
    #endregion
}
