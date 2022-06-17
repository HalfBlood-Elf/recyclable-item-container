using System.Collections;
using UnityEngine;

namespace RecyclableItemContainer
{
    public interface IRecycleSystemDataSource
    {
        public event System.Action ItemCountChanged;
        public int ItemCount { get; }
        void SetCell(ICell cell, int index);
    }
}