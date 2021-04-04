using System;

namespace XVagas.Models
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IncludeAttribute : Attribute{}
}
