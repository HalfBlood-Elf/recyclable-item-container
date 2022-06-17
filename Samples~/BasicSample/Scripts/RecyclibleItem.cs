using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RecyclableItemContainer
{
    public class RecyclibleItem : MonoBehaviour, ICell
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI messageTest;
        [SerializeField] private TextMeshProUGUI indexText;
        [SerializeField] private float lineHight = 100f;

        public RectTransform RectTransform => transform as RectTransform;

        public Vector2 GetSize()
        {
            return RectTransform.sizeDelta;
        }

        public void Setup(Data data, int index)
        {
            image.color = data.color;
            messageTest.text = data.message;
            indexText.text = index.ToString();
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, lineHight * data.linesCount);
        }
    }

}
