namespace FoxOffice
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Reflection;
    using AutoFixture;
    using AutoFixture.Kernel;

    public class ImmutableArrayCustomization : ICustomization
    {
        public void Customize(IFixture builder)
            => builder.Customizations.Add(new ImmutableArrayBuilder(builder));

        private class ImmutableArrayBuilder : ISpecimenBuilder
        {
            private readonly IFixture _builder;

            public ImmutableArrayBuilder(IFixture builder)
                => _builder = builder;

            public object Create(object request, ISpecimenContext context)
            {
                switch (request)
                {
                    case Type type when IsImmutableArrayType(type):
                        return GenerateImmutableArrayInstance(type);

                    default: return new NoSpecimen();
                }
            }

            private bool IsImmutableArrayType(Type type)
                => type.IsValueType
                && type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(ImmutableArray<>);

            private object GenerateImmutableArrayInstance(Type type)
            {
                Type elemType = type.GenericTypeArguments[0];
                MethodInfo template = typeof(ImmutableArrayBuilder).GetMethod(
                    nameof(GenerateImmutableArray),
                    BindingFlags.NonPublic | BindingFlags.Instance);
                return template.MakeGenericMethod(elemType).Invoke(this, null);
            }

            private ImmutableArray<T> GenerateImmutableArray<T>()
            {
                IEnumerable<T> elements = _builder.CreateMany<T>();
                return ImmutableArray.CreateRange(elements);
            }
        }
    }
}
