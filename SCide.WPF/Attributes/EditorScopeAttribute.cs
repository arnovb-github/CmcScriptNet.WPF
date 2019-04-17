using System;

namespace SCide.WPF.Attributes
{
    /// <summary>
    /// Mark a configuration property to affect all other configuration properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditorScopeAttribute : Attribute
    {
        public EditorScopeAttribute(bool global)
        {
            Global = global;
        }

        public bool Global { get; set; }
    }
}
