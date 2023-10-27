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
    [HideInInspector][SerializeField] List<Door> doorList = new List<Door>();
    [HideInInspector][SerializeField] List<Door> doorListWithoutCorrect = new List<Door>();
    [SerializeField] Transform _doorManagerContainer;
    public int doorCount = 3;
    int correctDoorIndex;
    [SerializeField][Range(1, 2)] float offsetX = 1;
    //[SerializeField][Range(1, 2)] float offsetY = 1;

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

            doorList.Add(_doorObject);
        }
    }

    private void DoorsReset()
    {
        if (doorList != null)
        {
            ResetAllDoorScale();
            doorList.Clear();
        }
        if (doorListWithoutCorrect != null) doorListWithoutCorrect.Clear();
        correctDoorIndex = 0;
        if (_doorManagerContainer.childCount == 0) return;

        foreach (Transform child in _doorManagerContainer)
            Destroy(child.gameObject);
    }

    public void ResetAllDoorScale()
    {
        foreach (Door component in doorList)
            component.transform.localScale = Vector3.one;
    }

    // If player's selected door is correct door, 

    void CopyDoorList()
    {
        foreach (Door component in doorList)
        {
            if (MontyhallManager.Instance.selected.GetComponent<Door>() != component && component != doorList[correctDoorIndex])
            {
                doorListWithoutCorrect.Add(component);
            }
        }

        if (MontyhallManager.Instance.selected.GetComponent<Door>() == doorList[correctDoorIndex])
        {
            int rnd = Random.Range(0, doorListWithoutCorrect.Count);
            doorListWithoutCorrect.Remove(doorListWithoutCorrect[rnd]);
        }
    }

    public void OpenDoors()
    {
        CopyDoorList();
        
        foreach (Door door in doorListWithoutCorrect)
        {
            if (door.isCorrect) Debug.LogError("correct door open error");

            door.OpenDoor(true);
        }
    }

    public bool TestCorrectness(Door door) => door == doorList[correctDoorIndex];
}