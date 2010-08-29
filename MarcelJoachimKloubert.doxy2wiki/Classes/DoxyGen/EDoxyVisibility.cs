// $Id: EDoxyVisibility.cs 21 2010-08-29 22:43:29Z mkloubert $

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
    /// List of possible visibilities.
    /// </summary>
    public enum DoxyVisibility
    {
        /// <summary>
        /// Internal
        /// </summary>
        Internal,

        /// <summary>
        /// Private
        /// </summary>
        Private,

        /// <summary>
        /// Public
        /// </summary>
        Public,

        /// <summary>
        /// Unknown visibility
        /// </summary>
        Unknown,
    }
}