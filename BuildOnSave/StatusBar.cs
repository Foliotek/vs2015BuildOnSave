using Microsoft.VisualStudio.Shell.Interop;
using System.Timers;

namespace BuildOnSave
{
	public class StatusBar
	{
		private const int DotIntervalMilliseconds = 650;
		private IVsStatusbar statusBar { get; set; }
		private short _icon { get; set; }
		private Timer _dotTimer { get; set; }
		private string _dots { get; set; }
		//private uint _progressBarTotalSteps { get; set; }
		//private uint _progressBarStepCount { get; set; }
		//private uint statusBarCookie = 0;

		public StatusBar()
		{ 
			statusBar = Utilities.GetStatusBar();
			//var projects = Utilities.GetProjects().ToList();
			//_progressBarStepCount = 0;
			//_progressBarTotalSteps = (uint)projects.Count;
			//OnBuildDone += StatusBar_OnBuildDone;
		}

		/// <summary>
		/// Puts the status bar into build mode
		/// </summary>
		public void BuildStart()
		{
			if (_icon != Icons.Build)
				SetIcon(Icons.Build);
			//InitializeProgressBar();
			SetText("Building", _icon);
			StartDots();
			Freeze();
		}

		/// <summary>
		/// Takes the status bar out of build mode
		/// </summary>
		public void BuildFinish()
		{
			//ClearProgressBar();
			StopDots();
			Unfreeze();
			StopIcon();
			SetText("Build finished");
		}

		#region Events
		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			IncrementDots();
			Unfreeze();
			SetText(CurrentText.Replace(".", "") + _dots);
			Freeze();
		}
		#endregion

		#region Basic Functions
		private string CurrentText
		{
			get
			{
				string text;
				statusBar.GetText(out text);
				return text;
			}
		}

		//private void InitializeProgressBar()
		//{
		//	Unfreeze();
		//	statusBar.Progress(ref statusBarCookie, 1, CurrentText, _progressBarStepCount, _progressBarTotalSteps);
		//	Freeze();
		//}
		//private void ClearProgressBar()
		//{
		//	Unfreeze();
		//	statusBar.Progress(ref statusBarCookie, 0, CurrentText, _progressBarStepCount, _progressBarTotalSteps);
		//	Freeze();
		//}
		//private void IncrementProgressBar()
		//{
		//	Unfreeze();
		//	statusBar.Progress(ref statusBarCookie, 1, CurrentText, _progressBarStepCount, _progressBarTotalSteps);
		//	Freeze();
		//}
		private void Clear()
		{
			statusBar.Clear();
		}
		private void SetText(string text)
		{
			int frozen;
			statusBar.IsFrozen(out frozen);
			if (frozen == 0)
			{
				statusBar.SetText(text);
			}
		}
		private void SetText(string text, short icon)
		{
			SetIcon(icon);
			StartIcon();
			SetText(text);
		}
		private void StartIcon()
		{
			statusBar.Animation(1, _icon);
		}
		private void StopIcon()
		{
			statusBar.Animation(0, _icon);
		}
		private void SetIcon(short icon)
		{
			_icon = icon;
		}
		private void Freeze()
		{
			statusBar.FreezeOutput(1);
		}
		private void Unfreeze()
		{
			statusBar.FreezeOutput(0);
		}
		private void StartDots()
		{
			_dotTimer = new Timer(DotIntervalMilliseconds);
			_dotTimer.Elapsed += timer_Elapsed;
			_dotTimer.Start();
		}
		private void StopDots()
		{
			_dotTimer.Stop();
			_dotTimer.Dispose();
		}
		private void IncrementDots()
		{
			if (_dots == null || _dots.Length == 3)
				_dots = ".";
			else
				_dots += ".";
		}
		#endregion
	}
}
