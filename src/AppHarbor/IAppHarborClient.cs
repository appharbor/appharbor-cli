using System;

namespace AppHarbor
{
	public interface IAppHarborClient
	{
		void CreateApplication(string name, string regionIdentifier);
	}
}
