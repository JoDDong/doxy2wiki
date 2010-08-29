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
    internal static class Program
    {
        #region Methods  (1)

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>Exit code for the operating system.</returns>
        private static int Main(string[] args)
        {
            int result = -1;

            try
            {
                string doxygenXmlFile = @"C:\Users\Marcel Kloubert\Desktop\doxygen_out\xml\index.xml";
                string output = @"mediawiki:C:\Users\Marcel Kloubert\Desktop\doxygen_out\test.xml";

                OutputFormat format = (OutputFormat)Enum.Parse(
                    typeof(OutputFormat),
                    output.Substring(0, output.IndexOf(":")).Trim(),
                    true);

                result = 0;
            }
            catch (Exception ex)
            {
                result = 1;

                Console.WriteLine(ex);
            }

#if DEBUG
            Console.WriteLine("===== ENTER =====");
            Console.ReadLine();
#endif

            return result;
        }

        #endregion Methods
    }
}
