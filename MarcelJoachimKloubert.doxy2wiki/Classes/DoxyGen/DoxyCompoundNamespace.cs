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

namespace MarcelJoachimKloubert.doxy2wiki.DoxyGen
{
    /// <summary>
    /// Stores data for a namepspace.
    /// </summary>
    public sealed class DoxyCompoundNamespace : IComparable<DoxyCompoundNamespace>, IEquatable<DoxyCompoundNamespace>
    {
        #region Fields (2)

        private object _compound;
        private string[] _parts;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compound">The underlying compound.</param>
        /// <param name="parts">The namespace parts.</param>
        public DoxyCompoundNamespace(DoxyCompound compound, IEnumerable<string> parts)
        {
            this._compound = compound;
            this._parts = parts.ToArray();
        }

        #endregion Constructors

        #region Properties (4)

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string FullName
        {
            get { return this.ToString(); }
        }

        /// <summary>
        /// Gets if that namespace represents a non-namespace (zero).
        /// </summary>
        public bool IsZero
        {
            get { return this.Parts.LongLength < 1; }
        }

        /// <summary>
        /// Gets the underlying compound.
        /// </summary>
        public DoxyCompound Parent
        {
            get { return (DoxyCompound)this._compound; }
        }

        /// <summary>
        /// Gets all parts of that namespace.
        /// </summary>
        public string[] Parts
        {
            get { return this._parts; }
        }

        #endregion Properties

        #region Methods  (8)

        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Are equal (false) or not (true).</returns>
        public static bool operator !=(DoxyCompoundNamespace left, DoxyCompoundNamespace right)
        {
            return (left == right) == false;
        }

        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Are equal (true) or not (false).</returns>
        public static bool operator ==(DoxyCompoundNamespace left, DoxyCompoundNamespace right)
        {
            // the object cast is to prevent a StackOverflowException
            if ((object)left != null)
            {
                return left.Equals(right);
            }

            // the object cast is to prevent a StackOverflowException
            if ((object)right != null)
            {
                return right.Equals(left);
            }

            // both are (null)
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IComparable&lt;T&gt;.CompareTo(T)" />
        public int CompareTo(DoxyCompoundNamespace other)
        {
            return StringComparer.CurrentCulture.Compare(this.ToString(), other.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            if (obj is DoxyCompoundNamespace)
            {
                return this.Equals((DoxyCompoundNamespace)obj);
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="IEquatable&lt;T&gt;.Equals(T)" />
        public bool Equals(DoxyCompoundNamespace other)
        {
            if (other == null)
            {
                return false;
            }

            return object.Equals(this.Parent.Parent, other.Parent.Parent) &&
                   other.FullName == this.FullName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="object.GetHashCode()" />
        public override int GetHashCode()
        {
            return this.FullName == null ? 0 : this.FullName.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="object.ToString()" />
        public override string ToString()
        {
            return this.ToString(".");
        }

        /// <summary>
        /// Joins the parts of that namespace to a 
        /// </summary>
        /// <param name="separator"></param>
        public string ToString(string separator)
        {
            if (this.IsZero)
            {
                return null;
            }

            return this.Parts.JoinAsString(separator);
        }

        #endregion Methods

        #region Nested Classes (1)


        /// <summary>
        /// A comparer for DoxyCompoundNamespace values.
        /// </summary>
        public sealed class DoxyCompoundNamespaceComparer : IEqualityComparer<DoxyCompoundNamespace>
        {
            #region Fields (1)

            private static DoxyCompoundNamespaceComparer _instance = null;

            #endregion Fields

            #region Constructors (1)

            /// <summary>
            /// 
            /// </summary>
            private DoxyCompoundNamespaceComparer()
            {

            }

            #endregion Constructors

            #region Properties (1)

            /// <summary>
            /// Gets the singleton instance.
            /// </summary>
            public static DoxyCompoundNamespaceComparer Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new DoxyCompoundNamespaceComparer();
                    }

                    return _instance;
                }
            }

            #endregion Properties

            #region Methods  (2)

            /// <summary>
            /// 
            /// </summary>
            /// <see cref="IEqualityComparer&lt;T&gt;.Equals(T, T)" />
            public bool Equals(DoxyCompoundNamespace x, DoxyCompoundNamespace y)
            {
                return x == y;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <see cref="IEqualityComparer&lt;T&gt;.GetHashCode(T)" />
            public int GetHashCode(DoxyCompoundNamespace obj)
            {
                return obj != null ? obj.GetHashCode() : 0;
            }

            #endregion Methods
        }
        #endregion Nested Classes
    }
}
