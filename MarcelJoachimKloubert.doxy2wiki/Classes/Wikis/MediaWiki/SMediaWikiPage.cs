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
using System.Text;
using System.Xml;

namespace MarcelJoachimKloubert.doxy2wiki.Wikis.MediaWiki
{
    /// <summary>
    /// Stores data of a mediawiki page.
    /// </summary>
    public struct MediaWikiPage
    {
        #region Data Members (5)

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public string Author
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comment text.
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public DateTime Timestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        #endregion Data Members

        #region Methods (3)

        /// <summary>
        /// Applies the data of that value to a XML node.
        /// </summary>
        /// <param name="rootNode">The node to apply the data to.</param>
        public void ApplyToXml(XmlNode rootNode)
        {
            XmlDocument xmlDoc = rootNode as XmlDocument;
            if (xmlDoc == null)
            {
                xmlDoc = rootNode.OwnerDocument;
            }

            XmlNode pageNode = xmlDoc.CreateElement("page");
            {
                XmlNode titleNode = xmlDoc.CreateElement("title");
                titleNode.InnerText = this.Title;
                pageNode.AppendChild(titleNode);

                XmlNode revisionNode = xmlDoc.CreateElement("revision");
                {
                    XmlNode timestampNode = xmlDoc.CreateElement("timestamp");
                    timestampNode.InnerText = this.Timestamp.ToString("s") + "Z";
                    revisionNode.AppendChild(timestampNode);

                    XmlNode contributorNode = xmlDoc.CreateElement("contributor");
                    contributorNode.InnerText = this.Author;
                    revisionNode.AppendChild(contributorNode);

                    XmlNode commentNode = xmlDoc.CreateElement("comment");
                    commentNode.InnerText = this.Comment;
                    revisionNode.AppendChild(commentNode);

                    XmlNode textNode = xmlDoc.CreateElement("text");
                    textNode.InnerText = this.Content;
                    revisionNode.AppendChild(textNode);
                }
                pageNode.AppendChild(revisionNode);
            }
            rootNode.AppendChild(pageNode);
        }

        /// <summary>
        /// Creates a new, empty value based on a specific timestamp.
        /// </summary>
        /// <param name="timeStamp">The timestamp to use.</param>
        /// <returns>The new, empty value.</returns>
        public static MediaWikiPage Create(DateTime timeStamp)
        {
            MediaWikiPage page = new MediaWikiPage();
            page.Timestamp = timeStamp.Kind != DateTimeKind.Utc ? timeStamp.ToUniversalTime() : timeStamp;

            return page;
        }

        /// <summary>
        /// Creates an empty import document.
        /// </summary>
        /// <param name="rootNode">The variable to write the root node to.</param>
        /// <returns>The created, empty document.</returns>
        public static XmlDocument CreateImportDocument(out XmlNode rootNode)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", Encoding.UTF8.WebName, null));

            XmlNode mediawikiNode = xmlDoc.CreateElement("mediawiki");
            xmlDoc.AppendChild(mediawikiNode);

            rootNode = mediawikiNode;
            return xmlDoc;
        }

        #endregion Methods
    }
}