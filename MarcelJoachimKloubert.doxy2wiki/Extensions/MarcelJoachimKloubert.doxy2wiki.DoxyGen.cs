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
            newPage.Title = proj.GetMediaWikiPath();
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
                        compound.GetMediaWikiPath(),
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
            newPage.Title = compound.GetMediaWikiPath();
            newPage.Author = "doxy2wiki";
            newPage.Comment = "Import.";

            newPage.Content = string.Format(@"
== {0} ==

{1}

{2}
{3}
{4}
{5}
", compound.FullName
 , compound.CreateMediaWikiDeclarationBlock()
 , compound.Constructors.CreateMediaWikiPageChapter("Constructors")
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
                result += string.Format(
                    "{0}{1}",
                    "''(none)''",
                    Environment.NewLine);
            }
            else
            {
                foreach (DoxyCompoundMember member in members.OrderBy(x => x.Name))
                {
                    string memberName = member.Name;
                    string memberSuffix = string.Empty;

                    switch (member.Type)
                    {
                        case DoxyCompoundMemberType.Function:
                            {
                                if (member.IsConstructor)
                                {
                                    memberName = ".ctor";
                                }

                                memberSuffix = string.Format("({0})", member.JoinParamsAsString());
                            }
                            break;
                    }

                    result += string.Format(
                        "* [[{0}|{1}{2}]]{3}{4}{3}{3}{5}",
                        member.GetMediaWikiPath(),
                        memberName,
                        memberSuffix,
                        "<br />",
                        member.Description,
                        Environment.NewLine);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string GetMediaWikiPath(this DoxyCompoundMember member)
        {
            return string.Format(
                "{0}/{1}#{2}",
                member.Parent.GetMediaWikiPath(),
                member.Name,
                string.Format("{0}({1})", member.Name, member.JoinParamsAsString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compound"></param>
        /// <returns></returns>
        public static string GetMediaWikiPath(this DoxyCompound compound)
        {
            return string.Format(
                "{0}/{1}/{2}",
                compound.Parent.GetMediaWikiPath(),
                compound.Namespace.HasValue ? compound.Namespace.Value.FullName : "(none)",
                compound.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public static string GetMediaWikiPath(this DoxyProject proj)
        {
            return string.Format("doxy2wiki/Documentation/{0}", proj.Name);
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="example"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static string CreateMediaWikiCodeBlock<T>(this DoxyExample<T> example, string lang = "csharp")
        {
            return string.Format(@"<source lang=""{0}"">

{1}

</source>", lang
          , example.Code);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compound"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static string CreateMediaWikiDeclarationBlock(this DoxyCompound compound, string lang = "csharp")
        {
            string result = string.Format("<source lang=\"{0}\">{1}{1}", lang, Environment.NewLine);

            if (compound.Namespace.HasValue)
            {
                result += string.Format(
                    "namespace {0}{1}{{{1}",
                    compound.Namespace.Value.FullName,
                    Environment.NewLine);
            }

            result += string.Format(
                "    {0} {1} {2}{3}    {{{3}        // members{3}",
                compound.Visibility.ToString().ToLower(),
                compound.Type.ToString().ToLower(),
                compound.Name,
                Environment.NewLine);

            result += string.Format(
                "    }}{0}",
                Environment.NewLine);

            if (compound.Namespace.HasValue)
            {
                result += string.Format(
                    "}}{0}",
                    Environment.NewLine);
            }

            result += string.Format("{0}</source>", Environment.NewLine);
            return result;
        }

        #endregion Methods
    }
    #endregion
}
