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

namespace System.Collections
{
    /// <summary>
    /// Extensions for System.Collections namespace.
    /// </summary>
    public static class TMExtensionsSystemCollections
    {
        #region Methods  (1)

        /// <summary>
        /// Joins the string representations of a sequence's items to one string.
        /// </summary>
        /// <param name="list">The sequence.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The generated string.</returns>
        public static string JoinAsString(this IEnumerable list, string separator = "")
        {
            string result = string.Empty;

            long i = 0;
            foreach (object item in list)
            {
                if (i++ > 0)
                {
                    result += separator;
                }

                result += item == null ? string.Empty : item.ToString();
            }

            return result;
        }

        #endregion Methods
    }
}