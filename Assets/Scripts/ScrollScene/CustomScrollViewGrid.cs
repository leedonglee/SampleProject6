using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomScrollViewGrid : MonoBehaviour
{
    public class GridButton
    {
        public Button button;
        public TextMeshProUGUI tmpText;
        public int number;

        public GridButton(Button button, TextMeshProUGUI tmpText)
        {
            this.button = button;
            this.tmpText = tmpText;
        }

        public void SetNumber(int number)
        {
            this.number = number;
            tmpText.text = number.ToString();
        }
    }

    private List<GridButton> _gridButtonList;

    public void Init(Action<int> buttonCallback)
    {
        _gridButtonList = new List<GridButton>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Button button = transform.GetChild(i).GetComponent<Button>();
            TextMeshProUGUI tmpText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            GridButton gridButton = new GridButton(button, tmpText);

            gridButton.button.onClick.AddListener(() =>
            {
                buttonCallback?.Invoke(gridButton.number);
            });

            _gridButtonList.Add(gridButton);
        }
    }

}
