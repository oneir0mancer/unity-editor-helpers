using UnityEngine;

namespace Oneiromancer.EditorHelpers.Attributes
{
    /// Attribute that allows to expand ScriptableObject references in editor. Do not use it on System.Serializable classes.
    public class ExpandableAttribute : PropertyAttribute
    { }
}