using UnityEngine;

namespace SLZ.Marrow.Utilities
{
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public bool includeChildren;
        public ReadOnlyAttribute(bool includeChildren = false)
        {
            this.includeChildren = includeChildren;
        }
    }
}