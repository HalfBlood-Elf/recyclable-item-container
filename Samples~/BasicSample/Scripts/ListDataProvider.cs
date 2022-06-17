using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecyclableItemContainer
{
    public class ListDataProvider : MonoBehaviour, IRecycleSystemDataSource
    {
        public event System.Action ItemCountChanged;

        [SerializeField] private RecyclibleItem itemPrefab;
        [SerializeField] private Transform container;
        [SerializeField] private int dataStartCount = 10;
        [SerializeField] private Color[] colors;
        [SerializeField] private VerticalRecycleSystem verticalRecycleSystem;

        private List<RecyclibleItem> recyclibleItems = new();
        private List<Data> datas = new();

        public int ItemCount => datas.Count;

        private void Start()
        {

            foreach (Transform item in container)
            {
                var itemScript = item.GetComponent<RecyclibleItem>();
                if (itemScript) recyclibleItems.Add(itemScript);
            }

            datas = GenerateData(dataStartCount);


            verticalRecycleSystem.SetNewDataSource(this);
            //if (recyclibleItems.Count < datas.Count)
            //{
            //    var diff = datas.Count - recyclibleItems.Count;
            //    for (int i = 0; i < diff; i++)
            //    {
            //        var newItem = Instantiate(itemPrefab, container).GetComponent<RecyclibleItem>();
            //        recyclibleItems.Add(newItem);
            //    }
            //}

            //for (int i = 0; i < recyclibleItems.Count; i++)
            //{
            //    var isActive = i < datas.Count;
            //    if (isActive)
            //        recyclibleItems[i].Setup(datas[i], i);
            //    recyclibleItems[i].gameObject.SetActive(isActive);
            //}
        }

        private List<Data> GenerateData(int count)
        {
            List<Data> newDatas = new();

            for (int i = 0; i < count; i++)
            {
                newDatas.Add(new Data()
                {
                    message = $"message_{i}",
                    color = colors[Random.Range(0, colors.Length)],
                    linesCount = Random.Range(1, 12)
                });
            }

            return newDatas;
        }

        public void SetCell(ICell cell, int index)
        {
            var item = cell as RecyclibleItem;
            item.Setup(datas[index], index);
        }

        public void AddData()
        {
            datas.Add(new Data()
            {
                message = $"message_{datas.Count}",
                color = colors[Random.Range(0, colors.Length)],
                linesCount = Random.Range(1, 12)
            });
            ItemCountChanged?.Invoke();
        }
    }
}
