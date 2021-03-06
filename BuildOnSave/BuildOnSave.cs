﻿//------------------------------------------------------------------------------
// <copyright file="BuildOnSave.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace BuildOnSave
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell. These attributes tell the pkgdef creation
	/// utility what data to put into .pkgdef file.
	/// </para>
	/// <para>
	/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
	/// </para>
	/// </remarks>
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[Guid(PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	[ProvideAutoLoad(UIContextGuids80.NoSolution)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideOptionPage(typeof(OptionsPage), "Build On Save", "Settings", 0, 0, true)]
	public sealed class BuildOnSave : Package
	{
		private const string PackageGuidString = "a3315629-609a-4a43-a68e-99bb1552569e";

		#region Initialization
		/// <summary>
		/// Initializes a new instance of the <see cref="BuildOnSave"/> class.
		/// </summary>
		public BuildOnSave()
		{
			// Inside this method you can place any initialization code that does not require
			// any Visual Studio service because at this point the package object is created but
			// not sited yet inside Visual Studio environment. The place to do all the other
			// initialization is the Initialize method.
		}

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			// Get necessary utilities
			dte = Utilities.GetDTE();
			StatusBar = new StatusBar();

			// Get and bind to events
			SetupEvents();
		}

		/// <summary>
		///		Initializes all the events for this extension
		/// </summary>
		private void SetupEvents()
		{
			// Get necessary events opjects
			_dteEvents = dte.Events;
			_docEvents = _dteEvents.DocumentEvents;
			_buildEvents = _dteEvents.BuildEvents;

			// Set event handler for document save
			_docEvents.DocumentSaved += DocumentEvents_DocumentSaved;

			// Set build event handlers
			_buildEvents.OnBuildBegin += BuildEvents_OnBuildBegin;
			_buildEvents.OnBuildDone += BuildEvents_OnBuildDone;
			_buildEvents.OnBuildProjConfigBegin += BuildEvents_OnBuildProjConfigBegin;
			_buildEvents.OnBuildProjConfigDone += BuildEvents_OnBuildProjConfigDone;
		}
		#endregion

		#region Properties
		private DTE2 dte { get; set; }
		private Events _dteEvents { get; set; }
		private DocumentEvents _docEvents { get; set; }
		private BuildEvents _buildEvents { get; set; }
		private StatusBar StatusBar { get; set; }
		private bool BuildRunning { get; set; }
		#endregion

		#region Events
		private void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
		{
			// Ignore everything except for "build" action
			if (Action == vsBuildAction.vsBuildActionBuild)
				BuildStart();
		}
		private void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
		{
			// Ignore everything except for "build" action
			if (Action == vsBuildAction.vsBuildActionBuild)
				BuildFinish();
		}
		private void BuildEvents_OnBuildProjConfigBegin(string Project, string ProjectConfig, string Platform, string SolutionConfig)
		{
			// Increment progress bar when a project build starts
			if (Settings.BuildEntireSolution)
				StatusBar.IncrementProgressBar();
		}
		private void BuildEvents_OnBuildProjConfigDone(string Project, string ProjectConfig, string Platform, string SolutionConfig, bool Success)
		{
			// Increment progress bar when a project build ends
			if (Settings.BuildEntireSolution)
				StatusBar.IncrementProgressBar();
		}

		private void DocumentEvents_DocumentSaved(Document document)
		{
			// Don't do anything if we're not enabled
			if (Settings.AutoBuildEnabled == false)
				return;

			// If document extension is in array of buildable extensions, start build
			if (Settings.Extensions.ToArray().Any(e => document.Name.EndsWith("." + e)) && !BuildRunning)
			{
				// Decide if entire solution should be built, or just project containing modified document
				if (Settings.BuildEntireSolution)
					BuildSolution();
				else
					BuildProject(document);
			}
		}
		#endregion

		#region Methods
		private void BuildSolution()
		{
			var sln = dte.Solution;
			sln.SolutionBuild.Build();
		}
		private void BuildProject(Document document)
		{
			var sln = dte.Solution;
			var config = sln.SolutionBuild.ActiveConfiguration.Name;
			var project = document.ProjectItem.ContainingProject;
			sln.SolutionBuild.BuildProject(config, project.UniqueName);
		}

		private void BuildStart()
		{
			BuildRunning = true;
			StatusBar.BuildStart();
		}

		private void BuildFinish()
		{
			bool buildSuccess = Utilities.IntToBool(dte.Solution.SolutionBuild.LastBuildInfo, 0);
			StatusBar.BuildFinish(buildSuccess);
			BuildRunning = false;
		}
		#endregion
	}
}
