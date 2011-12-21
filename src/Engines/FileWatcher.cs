/// 

/// This filewatcher works around the 'issues'
/// with Window's file watcher .NET FileSystemWatcher
/// or Win32::ChangeNotify).  This code is written for
/// WPF, but can be tweaked to work with Winforms as well.
/// 
/// It solves these two main problems:
/// * The FileSystemWatcher calls multiple times on
///   a single change.
/// * If my process changes the file, the
///   FileSystemWatcher calls me.
/// 
/// It solves the former by using a 100 ms timer to
/// collapse multiple calls into a single call.  It
/// solves the latter by storing the file size and
/// time when this process writes a file, and
/// comparing this to the values when notified by
/// FileSystemWatcher.
/// 
/// Usage is straightforward, except that you must
/// call CloseFileInThisProcess when you are closing
/// the file that this watcher is watching.  It will
/// carefully close the file in such a way that it
/// can later tell if the change was by this process
/// or another.
/// 
/// 

using System;
using System.IO;

namespace ManagedFusion.Rewriter.Engines
{
	internal class FileWatcher : FileSystemWatcher
	{
		public delegate void FileChangedHandler(string filepath);

		private string FilePath;
		private FileChangedHandler Handler;

		public FileWatcher(string filepath, FileChangedHandler handler) :
			base(System.IO.Path.GetDirectoryName(filepath), System.IO.Path.GetFileName(filepath))
		{
			FilePath = filepath;
			Handler = handler;
			NotifyFilter =
				NotifyFilters.FileName |
				NotifyFilters.Attributes |
				NotifyFilters.LastAccess |
				NotifyFilters.LastWrite |
				NotifyFilters.Security |
				NotifyFilters.Size;
			Changed += new FileSystemEventHandler(delegate(object sender, FileSystemEventArgs e) {
				Handler(FilePath);
			});
			EnableRaisingEvents = true;
		}
	}
}