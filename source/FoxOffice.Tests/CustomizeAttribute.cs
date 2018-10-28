namespace FoxOffice
{
    using System;
    using System.Reflection;
    using AutoFixture;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class CustomizeAttribute : Attribute, IParameterCustomizationSource
    {
        public abstract ICustomization GetCustomization(ParameterInfo parameter);
    }
}
