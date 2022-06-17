using UnityEngine;
namespace RecyclableItemContainer
{
    public interface ICell
    {
        public RectTransform RectTransform { get; }

        public Vector2 GetSize();
    }

    public class CellInList
    {
        public ICell cell;
        public int index;
    }
}