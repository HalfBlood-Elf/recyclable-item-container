using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecyclableItemContainer
{
    public class BottomToTopRecycler : RecyclerAbstract
    {
        protected override int direction => -1;

        protected override CellInList ActiveCellInBegining => activeCellList.Last.Value;

        protected override CellInList ActiveCellInEnd => activeCellList.First.Value;


        /// <summary>
        /// Recycles bottom cells out of bounds to top
        /// </summary>
        /// <returns>Content displacement in world units</returns>
        public BottomToTopRecycler(LinkedList<CellInList> activeCellList, RectTransform content) : base(activeCellList, content)
        {
        }

        protected override void SetActiveCellSiblingIndex(CellInList cell)
        {
            base.SetActiveCellSiblingIndex(cell);
            cell.cell.RectTransform.SetAsFirstSibling();
            activeCellList.RemoveLast();
            activeCellList.AddFirst(cell);
        }

        protected override Vector2 SetCellAndGetOffset(CellInList cell, IRecycleSystemDataSource dataSource)
        {
            // order of get size and setCell matters
            dataSource.SetCell(cell.cell, cell.index);
            return GetCellSize(cell);
        }
    }
}