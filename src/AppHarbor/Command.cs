using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppHarbor
{
	public abstract class Command
	{
		public abstract void Execute(string[] arguments);
	}
}
