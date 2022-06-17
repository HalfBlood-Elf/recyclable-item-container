using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecyclableItemContainer
{
    public abstract class RecyclerAbstract
    {
        protected LinkedList<CellInList> activeCellList = new();
        protected RectTransform content;

        public bool IsRecycling { get; protected set; }

        protected abstract int direction { get; }

        protected abstract CellInList ActiveCellInBegining{ get; }
        protected abstract CellInList ActiveCellInEnd { get; }

        public RecyclerAbstract(LinkedList<CellInList> activeCellList, RectTransform content)
        {
            this.activeCellList = activeCellList;
            this.content = content;
        }

        public Vector2 RecycleCell(IRecycleSystemDataSource dataSource)
        {
            var verticalDelta = Vector2.zero;
            if (!IsInRange(ActiveCellInEnd.index + direction, dataSource.ItemCount)) return verticalDelta;

            IsRecycling = true;

            var cell = ActiveCellInBegining;
            SetActiveCellSiblingIndex(cell);

            var cellDelta = SetCellAndGetOffset(cell, dataSource);

            //note: if content's y pos is growing - it moves upwards
            content.anchoredPosition += cellDelta;
            verticalDelta += cellDelta;

            IsRecycling = false;
            return verticalDelta;
        }

        protected virtual bool IsInRange(int index, int itemCount)
        {
            return index < itemCount && index >= 0;
        }

        protected virtual void SetActiveCellSiblingIndex(CellInList cell)
        {
            cell.index = ActiveCellInEnd.index + direction;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>Vector2 to offset content position to remain in the same place</returns>
        protected abstract Vector2 SetCellAndGetOffset(CellInList cell, IRecycleSystemDataSource dataSource);
        
        protected virtual Vector2 GetCellSize(CellInList cell)
        {
            return new Vector2(0, cell.cell.GetSize().y * -1 * direction);
        }
    }
}