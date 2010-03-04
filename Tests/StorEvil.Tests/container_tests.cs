using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Funq;
using NUnit.Framework;

namespace StorEvil
{

    [TestFixture]
    public class funq_easy_registration
    {
        [Test]
        public void should_be_able_to_get_service_impl()
        {
            var c = new Container();
            c.EasyRegister<IFoo, Foo>();

            Assert.IsInstanceOfType(typeof(Foo), c.Resolve<IFoo>());
        }

        [Test]
        public void should_be_able_to_inject_dependency()
        {
            var c = new Container();
            c.EasyRegister<IFoo, Foo>();
            c.EasyRegister<IBar, Bar>();

            var bar = c.Resolve<IBar>() as Bar;

            Assert.IsNotNull(bar.Foo);
        }

        [Test]
        public void should_be_able_to_chain_dependencies()
        {
            var c = new Container();
            var testFoo = new Foo();
            c.Register<IFoo>(testFoo);
            c.EasyRegister<IBar, Bar>();
            c.EasyRegister<IBaz, Baz>();
            var baz = c.Resolve<IBaz>() as Baz;

            var bar = baz.Bar as Bar;
            Assert.AreSame(bar.Foo, testFoo);
        }

     
    }

    public interface IFoo { }
    public class Foo : IFoo { }

    public interface IBar { }
    public class Bar : IBar
    {
        public IFoo Foo { get; set; }

        public Bar(IFoo foo)
        {
            Foo = foo;
        }
    }

    public interface IBaz { }
    public class Baz : IBaz
    {
        public IBar Bar { get; set; }

        public Baz(IBar bar)
        {
            Bar = bar;
        }
    }

    
}