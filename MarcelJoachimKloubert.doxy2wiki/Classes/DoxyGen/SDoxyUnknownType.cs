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

namespace MarcelJoachimKloubert.doxy2wiki.DoxyGen
{
    /// <summary>
    /// Stores data of a unknown type.
    /// </summary>
    public struct DoxyUnknownType : IDoxyType
    {
        #region Data Members (3)

        private string _name;
        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IDoxyType.FullName" />
        public string FullName
        {
            get { return this.Name; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IDoxyType.Name" />
        public string Name
        {
            get { return this._name; }
        }

        #endregion Data Members

        #region Methods (1)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the type.</param>
        public DoxyUnknownType(string name)
        {
            this._name = name;
        }

        #endregion Methods
    }
}