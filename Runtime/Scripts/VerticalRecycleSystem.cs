using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RecyclableItemContainer
{
    public class VerticalRecycleSystem : MonoBehaviour
    {
        protected IRecycleSystemDataSource DataSource;

        [SerializeField] protected ScrollRectExtended scrollRect;
        [SerializeField] protected Scrollbar scrollbar;
        [SerializeField] protected RectTransform prototypeCell;
        [SerializeField] protected Transform poolContainer;
        [SerializeField] protected int maxActiveElementsCount = 10;
        [SerializeField] protected float recyclingThreshold = .2f;

        protected LinkedList<CellInList> activeCellList = new();
        protected Stack<ICell> cellPool = new();
        protected float minPoolCoverage = 1.5f;
        protected float minCellSize = 10;
        /// <summary>
        /// Min is bottom-left. Max is top-right
        /// </summary>
        protected Bounds recyclableViewBounds = new();
        protected Vector2 lastContentPos;
        protected bool recycling;
        protected readonly Vector3[] corners = new Vector3[4];
        protected int topCellItemIndex = 0;
        protected bool initilized;

        protected RecyclerAbstract bottomToTopRecycler;
        protected RecyclerAbstract topToBottomRecycler;


        protected CellInList TopmostActiveCell => activeCellList.First.Value;
        protected CellInList BottommostActiveCell => activeCellList.Last.Value;

        private void Start()
        {
            if (initilized) return;
            scrollRect.onValueChanged.AddListener(ScrollRectValueChanged);
            SetRecyclingBounds();
            lastContentPos = scrollRect.normalizedPosition;
            var listToMove = new List<RectTransform>();
            var index = 0;
            foreach (Transform item in scrollRect.content)
            {
                var cell = item.GetComponent<ICell>();
                if (cell != null)
                {
                    if (DataSource != null && index < maxActiveElementsCount && index < DataSource.ItemCount)
                    {
                        activeCellList.AddLast(new CellInList() { cell = cell, index = index });
                    }
                    else
                    {
                        cellPool.Push(cell);
                        listToMove.Add(cell.RectTransform); // do not modify collection while iterating over it - cell.RectTransform.SetParent(poolContainer);
                    }
                }
                else Destroy(item.gameObject);
                index++;
            }
            //CoverArea();
            SetActiveItems();

            for (int i = 0; i < listToMove.Count; i++)
            {
                listToMove[i].SetParent(poolContainer);
            }

            initilized = true;
        }

        protected void SetActiveItems()
        {
            int dataSourceItemCount = DataSource != null ? DataSource.ItemCount : 0;
            if (activeCellList.Count > dataSourceItemCount)
            {
                //remove unnecessary items
                for (int i = 0; i < activeCellList.Count - dataSourceItemCount; i++)
                {
                    var lastCell = BottommostActiveCell;
                    activeCellList.RemoveLast();
                    cellPool.Push(lastCell.cell);
                    lastCell.cell.RectTransform.SetParent(poolContainer);

                }
            }
            else if(activeCellList.Count < maxActiveElementsCount && activeCellList.Count < dataSourceItemCount)
            {
                var countToSpawn = maxActiveElementsCount - activeCellList.Count;
                if(countToSpawn + activeCellList.Count > dataSourceItemCount) countToSpawn = dataSourceItemCount - activeCellList.Count;
                for (int i = 0; i < countToSpawn; i++)
                {
                    ICell cell;
                    if (cellPool.Count > 0)
                    {
                        cell = cellPool.Pop();
                        cell.RectTransform.SetParent(scrollRect.content);
                    }
                    else
                    {
                        cell = Instantiate(prototypeCell, scrollRect.content).GetComponent<ICell>();
                    }

                    cell.RectTransform.SetAsLastSibling();
                    var index = activeCellList.Count > 0 ? BottommostActiveCell.index + 1 : 0;
                    activeCellList.AddLast(new CellInList() { cell = cell, index = index });
                }
            }

            foreach (var item in activeCellList)
            {
                DataSource.SetCell(item.cell, item.index);
            }
        }

        public void SetNewDataSource(IRecycleSystemDataSource dataSource)
        {
            if (!initilized) Start();
            if (DataSource != null)
            {
                DataSource.ItemCountChanged -= DataSource_ItemCountChanged;
            }
            DataSource = dataSource;
            DataSource.ItemCountChanged += DataSource_ItemCountChanged;

            SetActiveItems();

        }


        private void DataSource_ItemCountChanged()
        {
            SetActiveItems();
        }

        private void ScrollRectValueChanged(Vector2 scrollRectNormalizedPosition)
        {
            if (activeCellList.Count == 0 || recycling) return;

            var direction = lastContentPos - scrollRect.content.anchoredPosition;
            if (Mathf.Abs(direction.y) < 1e-4f) return;

            //Debug.Log(direction.y > 0 ? "content went up" : "content went down");
            var delta = Vector2.zero;
            if (-direction.y > 0 && CellOutOfTopBounds(TopmostActiveCell.cell)) //content went up
            {
                topToBottomRecycler ??= new TopToBottomRecycler(activeCellList, scrollRect.content);
                delta = topToBottomRecycler.RecycleCell(DataSource);
            }
            else if (-direction.y < 0 && CellOutOfBottomBounds(BottommostActiveCell.cell)) //content went down
            {
                bottomToTopRecycler ??= new BottomToTopRecycler(activeCellList, scrollRect.content);
                delta = bottomToTopRecycler.RecycleCell(DataSource);
            }
            scrollRect.OnContentPositionChanged(delta);
            lastContentPos = scrollRect.content.anchoredPosition;
        }

        protected bool CellOutOfTopBounds(ICell cell) => cell.RectTransform.MinY() > recyclableViewBounds.max.y;
        protected bool CellOutOfBottomBounds(ICell cell) => cell.RectTransform.MaxY() < recyclableViewBounds.min.y;

        private void CoverArea()
        {
            var node = activeCellList.First;
            ICell item;
            float currentPoolCoverage = 0;
            int poolSize = activeCellList.Count;
            float requriedCoverage = minPoolCoverage * scrollRect.viewport.rect.height;


            while (poolSize < maxActiveElementsCount || currentPoolCoverage < requriedCoverage)
            {
                //Instantiate and add to Pool
                if (poolSize == 0  && node != null ) item = node.Value.cell;
                else if (node.Next != null)
                {
                    node = node.Next;
                    item = node.Value.cell;
                }
                else
                {
                    item = Instantiate(prototypeCell.gameObject, scrollRect.content).GetComponent<ICell>();
                    node = activeCellList.AddLast(new CellInList() { cell = item, index = poolSize });
                }

                var itemRectTransform = item.RectTransform;
                itemRectTransform.name = "Cell_" + poolSize;

                currentPoolCoverage += Mathf.Max(itemRectTransform.rect.height, minCellSize);
                poolSize++;
            }

            //var cellsLeft = cellPool.Count - poolSize;
            //for (int i = 0; i < cellsLeft; i++)
            //{
            //    Destroy(cellPool.Last.Value.RectTransform.gameObject);
            //    cellPool.RemoveLast();
            //}
        }

        private void SetRecyclingBounds()
        {
            scrollRect.viewport.GetWorldCorners(corners);
            float threshHold = recyclingThreshold * (corners[UICornersExtension.TOP_RIGHT].y - corners[UICornersExtension.BOTTOM_LEFT].y);
            recyclableViewBounds.SetMinMax(
                new Vector3(corners[UICornersExtension.BOTTOM_LEFT].x, corners[UICornersExtension.BOTTOM_LEFT].y - threshHold),
                new Vector3(corners[UICornersExtension.TOP_RIGHT].x, corners[UICornersExtension.TOP_RIGHT].y + threshHold));
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawLine(recyclableViewBounds.min, new Vector2(recyclableViewBounds.max.x, recyclableViewBounds.min.y));
        //    Gizmos.DrawLine(new Vector2(recyclableViewBounds.min.x, recyclableViewBounds.max.y), recyclableViewBounds.max);
        //}

    }
}
