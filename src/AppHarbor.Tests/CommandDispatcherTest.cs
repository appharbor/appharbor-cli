using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace AppHarbor.Tests
{
	public class CommandDispatcherTest
	{
		public class FooCommand : ICommand
		{
			public virtual void Execute(string[] arguments)
			{
			}
		}
	}
}
