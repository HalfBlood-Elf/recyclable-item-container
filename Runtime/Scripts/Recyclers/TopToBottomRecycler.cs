using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecyclableItemContainer
{
    public class TopToBottomRecycler : RecyclerAbstract
    {
        protected override int direction => 1;

        protected override CellInList ActiveCellInBegining => activeCellList.First.Value;
        protected override CellInList ActiveCellInEnd => activeCellList.Last.Value;

        /// <summary>
        /// Recycles top cell out of bounds to bottom
        /// </summary>
        /// <returns>Content displacement in world units</returns>
        public TopToBottomRecycler(LinkedList<CellInList> activeCellList, RectTransform content) : base(activeCellList, content)
        {
        }

        protected override void SetActiveCellSiblingIndex(CellInList cell)
        {
            base.SetActiveCellSiblingIndex(cell);
            cell.cell.RectTransform.SetAsLastSibling();
            activeCellList.RemoveFirst();
            activeCellList.AddLast(cell);
        }

        protected override Vector2 SetCellAndGetOffset(CellInList cell, IRecycleSystemDataSource dataSource)
        {
            // order of get size and setCell matters
            var cellDelta = GetCellSize(cell);
            dataSource.SetCell(cell.cell, cell.index);
            return cellDelta;
        }
    }
}