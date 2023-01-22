using System;

namespace DS.Utils.DataStructures
{
	public static class DisposableExtensions
	{
		public static void AddTo(this IDisposable disposable, DisposableList disposableList)
		{
			disposableList.Add(disposable);
		}
	}
}