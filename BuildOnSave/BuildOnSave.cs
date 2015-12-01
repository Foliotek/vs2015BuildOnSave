//------------------------------------------------------------------------------
// <copyright file="BuildOnSave.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using EnvDTE80;
using EnvDTE;
using System.Linq;

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
    [Guid(BuildOnSave.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    public sealed class BuildOnSave : Package
    {
        /// <summary>
        /// BuildOnSave GUID string.
        /// </summary>
        public const string PackageGuidString = "a3315629-609a-4a43-a68e-99bb1552569e";

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

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            _dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            _statusBar = Package.GetGlobalService(typeof(SVsStatusbar)) as IVsStatusbar;
            SetupEvents();
        }
        #endregion


        #region Custom Foliotek Code

        private DTE2 _dte { get; set; }
        private Events _dteEvents { get; set; }
        private DocumentEvents _docEvents { get; set; }
        private BuildEvents _buildEvents { get; set; }
        private IVsStatusbar _statusBar { get; set; }

        private int? _estimatedBuildMilliSeconds { get; set; }
        private DateTime _buildStart { get; set; }

        private void SetupEvents()
        {
            _dteEvents = _dte.Events;
            _docEvents = _dteEvents.DocumentEvents;
            _buildEvents = _dteEvents.BuildEvents;

            _docEvents.DocumentSaved += DocumentEvents_DocumentSaved;

            _buildEvents.OnBuildBegin += BuildEvents_OnBuildBegin;
            _buildEvents.OnBuildDone += BuildEvents_OnBuildDone;
        }

        private void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            _estimatedBuildMilliSeconds = DateTime.Now.Subtract(_buildStart).Milliseconds;
            UpdateStatusBar("Build Finished");
            //StatusBarProgressFinish();
        }

        private void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            _buildStart = DateTime.Now;
            UpdateStatusBar("Building");
            //StatusBarProgressStart();
        }

        private void DocumentEvents_DocumentSaved(Document document)
        {
            string[] knownBuildExtensions = new string[] { "cs", "config" };
            if (knownBuildExtensions.Any(e => document.Name.EndsWith("." + e)))
            {
                var sln = _dte.Solution;
                var config = sln.SolutionBuild.ActiveConfiguration.Name;
                var project = document.ProjectItem.ContainingProject;
                sln.SolutionBuild.BuildProject(config, project.UniqueName);
            }
        }

        //private void StatusBarProgressFinish()
        //{
        //    uint cookie = 0;
        //    _statusBar.Progress(ref cookie, 1, "Build Finished", 1, 1);
        //}
        //private void StatusBarProgressStart()
        //{
        //    new System.Threading.Thread(new ThreadStart(_StartBar)).Start();
        //}
        //private void _StartBar()
        //{
        //    uint cookie = 0;
        //    _statusBar.Progress(ref cookie, 1, "", 0, 0);
        //    uint max = (uint)(_estimatedBuildMilliSeconds.HasValue ? (_estimatedBuildMilliSeconds.Value / 100) : 5);
        //    for (uint i = 1; i <= max; i++)
        //    {
        //        _statusBar.Progress(ref cookie, 1, "Building", i, max);
        //        System.Threading.Thread.Sleep(1000);
        //    }
        //}

        private void UpdateStatusBar(string text)
        {
            int frozen;
            _statusBar.IsFrozen(out frozen);
            if (frozen == 0)
            {
                _statusBar.SetText(text);
            }
        }

        #endregion
    }
}
