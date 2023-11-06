using TMPro;
using UnityEngine;

public enum DataType
{
    STAY_WIN,
    STAY_LOSE,
    SWITCH_WIN,
    SWITCH_LOSE
}

public class Chart : MonoBehaviour
{
    [SerializeField] RectTransform[] dataTypeRectTransform;
    [SerializeField] TextMeshProUGUI[] dataTypePercentage;
    [SerializeField] int[] count;

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            dataTypeRectTransform[i].sizeDelta = new Vector2(dataTypeRectTransform[i].sizeDelta.x, 0);
            dataTypePercentage[i].gameObject.SetActive(false);
        }
    }

    public void AddNewData(DataType _type, int _value)
    {
        //SetPercentage(_type, _value);
        _value *= 1;
        RectTransform _rt = dataTypeRectTransform[(int)_type];
        _rt.sizeDelta += new Vector2(0, _value);
    }

    public void SetPercentage(DataType _type, int _value)
    {
        dataTypePercentage[(int)_type].text = string.Format("%{0}", _value);
    }

    public void ClearChart()
    {
        dataTypeRectTransform[(int)DataType.STAY_LOSE].sizeDelta = Vector2.zero;
        dataTypeRectTransform[(int)DataType.STAY_WIN].sizeDelta = Vector2.zero;
        dataTypeRectTransform[(int)DataType.SWITCH_LOSE].sizeDelta = Vector2.zero;
        dataTypeRectTransform[(int)DataType.SWITCH_WIN].sizeDelta = Vector2.zero;
    }
}
