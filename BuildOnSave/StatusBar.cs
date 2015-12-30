using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Timers;
using System.Linq;

namespace BuildOnSave
{
	public class StatusBar
	{
		private const int DotIntervalMilliseconds = 650;
		private IVsStatusbar statusBar { get; set; }
		private object _icon { get; set; }
		private Timer _timer { get; set; }
		private string _dots { get; set; }
		private int _progressBarSteps { get; set; }
		private int _currentProgressBarStep { get; set; }
		private int _progressBarPercent
		{
			get
			{
				return (int)Math.Round((decimal)((_currentProgressBarStep / _progressBarSteps) * 100));
			}
		}

		public StatusBar()
		{
			statusBar = Utilities.GetStatusBar();
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

		public void BuildStart()
		{
			SetIcon(Icons.Build);
			SetText("Building", _icon);
			StartDots();
			Freeze();
		}

		public void BuildFinish()
		{
			StopDots();
			Unfreeze();
			SetText("Build finished");
			StopAnimation();
		}

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

		private void InitializeProgressBar()
		{
			var projects = Utilities.GetProjects().ToList();
			_progressBarSteps = projects.Count;
		}
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
		private void SetText(string text, object icon)
		{
			SetIcon(icon);
			StartAnimation();
			SetText(text);
		}
		private void StartAnimation()
		{
			statusBar.Animation(1, _icon);
		}
		private void StopAnimation()
		{
			statusBar.Animation(0, _icon);
		}
		private void SetIcon(object icon)
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
			_timer = new System.Timers.Timer(DotIntervalMilliseconds);
			_timer.Elapsed += timer_Elapsed;
			_timer.Start();
		}
		private void StopDots()
		{
			_timer.Stop();
			_timer.Dispose();
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
