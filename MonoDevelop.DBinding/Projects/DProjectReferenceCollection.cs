//
// DProjectReferenceCollection.cs
//
// Author:
//       Alexander Bothe <info@alexanderbothe.com>
//
// Copyright (c) 2013 Alexander Bothe
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using MonoDevelop.D.Projects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MonoDevelop.Projects;

namespace MonoDevelop.D.Projects
{
	public enum ReferenceType
	{
		Project,
		Package,
	}

	public abstract class DProjectReferenceCollection
	{
		public readonly AbstractDProject Owner;

		public virtual bool CanDelete {get{ return true; }}
		public virtual bool CanAdd{ get {return true;}}

		public virtual IEnumerable<string> Includes {get {return Owner.LocalIncludes; }}
		public virtual IEnumerable<string> ReferencedProjectIds {get { return new string[0];}}
		public virtual bool HasReferences {get { return Owner.LocalIncludes.Count > 0; }}

		public DProjectReferenceCollection(AbstractDProject owner)
		{
			Owner = owner;
		}

		public virtual void DeleteInclude(string path)
		{
			Owner.LocalIncludes.Remove (path);
		}
		public abstract void DeleteProjectRef(string projectId);

		public abstract event EventHandler Update;
		public abstract void FireUpdate();

		public abstract bool AddReference();
	}
}
