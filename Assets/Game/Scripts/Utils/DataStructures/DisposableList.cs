using System;
using System.Collections.Generic;

namespace DS.Utils.DataStructures
{
	public class DisposableList : List<IDisposable>, IDisposable
	{
		public void Dispose()
		{
			ForEach(x => x.Dispose());
		}
	}
}