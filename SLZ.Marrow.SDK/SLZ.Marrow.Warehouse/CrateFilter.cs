using UnityEngine;

namespace SLZ.Marrow.Warehouse
{
    public interface ICrateFilter<in T> where T : Crate
    {
        bool Filter(T crate);
    }
}