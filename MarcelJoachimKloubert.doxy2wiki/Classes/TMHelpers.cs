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

namespace MarcelJoachimKloubert.doxy2wiki
{
    /// <summary>
    /// Helper class.
    /// </summary>
    public static class TMHelpers
    {
        #region Methods  (1)

        /// <summary>
        /// Tries to parse a string to a enum value.
        /// </summary>
        /// <typeparam name="TEnum">Type of the enum.</typeparam>
        /// <param name="str">The string to parse.</param>
        /// <param name="value">The variable to write the parsed value to. If the string could not been parsed, the variable contains the default value of the enum type.</param>
        /// <param name="ignoreCase">Ignore case (true) or not (false).</param>
        /// <returns>String was parsed (true) or not (false).</returns>
        public static bool TryParseEnum<TEnum>(string str, out TEnum value, bool ignoreCase = false)
        {
            Type enumType = typeof(TEnum);
            if (enumType.IsEnum == false)
            {
                throw new ArgumentException("TEnum");
            }

            value = default(TEnum);

            if (str.IsNullOrWhitespace())
            {
                return false;
            }

            try
            {
                value = (TEnum)Enum.Parse(enumType, str, ignoreCase);
            }
            catch (ArgumentException)
            {
                // 'str' does not contain a now value inside the enum
                return false;
            }

            return true;
        }

        #endregion Methods
    }
}
