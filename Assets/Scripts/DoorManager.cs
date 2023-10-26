using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DoorManager : MonoBehaviour
{
    #region Singleton
    public static DoorManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] GameObject _doorPref;
    [HideInInspector][SerializeField] List<Door> doorArray = new List<Door>();
    [SerializeField] Transform _doorManagerContainer;
    public int doorCount = 3;
    int correctDoorIndex;
    [SerializeField][Range(1, 2)] float offsetX = 1;
    [SerializeField][Range(1, 2)] float offsetY = 1;

    void Start() => DoorsSetup();

    public void DoorsSetup()
    {
        DoorsReset();

        correctDoorIndex = Random.Range(0, doorCount);
        for (int i = 0; i < doorCount; i++)
        {
            GameObject _newDoorPref = Instantiate(_doorPref, transform.position + new Vector3(i * 2 * offsetX, 0, 0), Quaternion.identity);
            _newDoorPref.transform.SetParent(_doorManagerContainer);
            _newDoorPref.name = i.ToString();
            Door _doorObject = _newDoorPref.GetComponent<Door>();
            _doorObject.index = i;
            if (i == correctDoorIndex) _doorObject.isCorrect = true;
            else _doorObject.isCorrect = false;

            doorArray.Append(_doorObject);
        }
    }

    private void DoorsReset()
    {
        if (doorArray != null) doorArray.Clear();
        if (_doorManagerContainer.childCount == 0) return;

        foreach (Transform child in _doorManagerContainer)
        {
            Destroy(child);
        }
    }

    public void ResetAllDoorScale()
    {
        foreach (Door component in doorArray)
        {
            component.transform.localScale = Vector3.one;
        }
    }
}