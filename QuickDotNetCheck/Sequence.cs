using System;
using System.Collections.Generic;
using QuickDotNetCheck.NotInTheRoot;

namespace QuickDotNetCheck
{
	public class Sequence
	{
		public int NumberToRun { get; set; }
		public List<Func<IFixture>> FixtureFuncs { get; set; }

		public Sequence()
		{
			FixtureFuncs = new List<Func<IFixture>>();
		}

		public Sequence Register<TFixture>()
			where TFixture : IFixture
		{
			FixtureFuncs.Add(() => (TFixture)Activator.CreateInstance(typeof(TFixture)));
			return this;
		}

		public Sequence Register(params Func<IFixture>[] funcs)
		{
			foreach (var fixtureFunc in funcs)
			{
				FixtureFuncs.Add(fixtureFunc);
			}
			return this;
		}

		public Sequence Register(params Type[] types)
		{
			foreach (var type in types)
			{
				var typeCopy = type;
				FixtureFuncs.Add(() => (IFixture)Activator.CreateInstance(typeCopy));
			}
			return this;
		}
	}
}