using System;

namespace Base.Helpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IncludeAttribute : Attribute{}
}
