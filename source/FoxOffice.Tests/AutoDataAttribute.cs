namespace FoxOffice
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using AutoFixture.Kernel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AutoDataAttribute : Attribute, ITestDataSource
    {
        private readonly IFixture _builder;

        public AutoDataAttribute() => _builder = CreateBuilder();

        private static IFixture CreateBuilder()
        {
            var customization =
                new CompositeCustomization(
                    new AutoMoqCustomization(),
                    new ImmutableArrayCustomization());

            return new Fixture().Customize(customization);
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            yield return methodInfo.GetParameters().Select(Resolve).ToArray();
        }

        private object Resolve(ParameterInfo parameter)
        {
            foreach (IParameterCustomizationSource attribute in parameter
                .GetCustomAttributes()
                .OfType<IParameterCustomizationSource>())
            {
                attribute.GetCustomization(parameter).Customize(_builder);
            }

            return new SpecimenContext(_builder).Resolve(request: parameter);
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            IEnumerable<string> args = methodInfo
                .GetParameters()
                .Zip(data, (param, arg) => $"{param.Name}: {arg}");

            return $"{methodInfo.Name}({string.Join(", ", args)})";
        }
    }
}
