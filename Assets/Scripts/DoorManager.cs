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
    [HideInInspector][SerializeField] List<bool> doorPrizeArray = new List<bool>();
    [SerializeField] Transform _doorManagerContainer;
    public int doorCount = 3;
    int correctDoorIndex;

    public GameObject selected;

    void Start() => DoorsSetup();

    public void DoorsSetup()
    {
        DoorsReset();

        correctDoorIndex = Random.Range(0, doorCount);
        for (int i = 0; i < doorCount; i++)
        {
            GameObject _newDoor = Instantiate(_doorPref, transform.position + new Vector3(i * 2 - 2, 0, 0), Quaternion.identity);
            if (i == correctDoorIndex) doorPrizeArray.Add(true);
            else doorPrizeArray.Add(false);
            _newDoor.name = i.ToString();
            _newDoor.transform.SetParent(_doorManagerContainer);
        }
    }

    private void DoorsReset()
    {
        if (doorPrizeArray != null) doorPrizeArray.Clear();
        if (_doorManagerContainer.childCount == 0) return;

        foreach (Transform child in _doorManagerContainer)
        {
            Destroy(child);
        }
    }
}

class Door
{
    protected GameObject doorObject;
    protected bool isCorrect;
    protected int index;
}