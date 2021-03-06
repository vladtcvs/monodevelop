// DotNetProjectSubtype.cs
//
// Author:
//   Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
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
//
//

using System;
using System.Linq;
using Mono.Addins;
using MonoDevelop.Projects.MSBuild;
using System.Collections.Generic;
using MonoDevelop.Core;
using System.Threading.Tasks;

namespace MonoDevelop.Projects.Extensions
{
	public abstract class ProjectMigrationHandler
	{
		/// <summary>
		/// Returns a list of files that should be backed up during migration.
		/// </summary>
		/// <returns>Files to backup.</returns>
		/// <param name="filename">Project path</param>
		public virtual IEnumerable<string> FilesToBackup (string filename)
		{
			yield break;
		}

		/// <summary>
		/// Executes the proejct migration
		/// </summary>
		/// <param name="monitor">Progress monitor</param>
		/// <param name="project">MSBuild project instance, if available</param>
		/// <param name="fileName">Project path</param>
		/// <param name="language">Language name (for .NET projects).</param>
		public abstract Task<bool> Migrate (ProjectLoadProgressMonitor monitor, MSBuildProject project, string fileName, string language);

		/// <summary>
		/// If it returns true, PromptForMigration will be called to confirm the migration.
		/// </summary>
		public virtual bool CanPromptForMigration {
			get { return false; }
		}

		/// <summary>
		/// Ask the use for confirmation of the migration
		/// </summary>
		/// <returns>Use answer</returns>
		/// <param name="monitor">Progress monitor</param>
		/// <param name="project">MSBuild project instance, if available</param>
		/// <param name="fileName">Project path</param>
		/// <param name="language">Language name (for .NET projects).</param>
		public virtual Task<MigrationType> PromptForMigration (ProjectLoadProgressMonitor monitor, MSBuildProject project, string fileName, string language)
		{
			throw new NotImplementedException ();
		}
	}
	
	public enum MigrationType {
		Ignore,
		Migrate,
		BackupAndMigrate,
	}
}
