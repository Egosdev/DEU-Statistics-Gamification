using System;
using System.Collections.Generic;
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
    [SerializeField] Transform _doorManagerContainer;
    [SerializeField] [Range(1, 2)] float offsetX = 1;

    [HideInInspector]
    [SerializeField] List<Door> doorList = new List<Door>();

    public int doorCount = 3;
    private int _correctDoorIndex;

    void Start() => DoorsSetup();
    public bool CheckForPrizeDoor(Door door) => door == doorList[_correctDoorIndex];

    /// <summary>
    /// Recreate the doors at the beginning of each round.
    /// </summary>
    public void DoorsSetup()
    {
        RemoveAllDoors();

        _correctDoorIndex = Random.Range(0, doorCount);
        for (int i = 0; i < doorCount; i++)
        {
            GameObject _newDoorPref = Instantiate(_doorPref, transform.position + new Vector3(i * 2 * offsetX, 0, 0), Quaternion.identity, _doorManagerContainer);
            _newDoorPref.name = i.ToString();
            _newDoorPref.GetComponent<Door>().index = i;

            doorList.Add(_newDoorPref.GetComponent<Door>());
        }
        doorList[_correctDoorIndex].isCorrect = true;
    }

    /// <summary>
    /// Doors without prizes will open until only two doors remain, one with a prize and the other without a prize.
    /// </summary>
    public void OpenNonPrizeDoors()
    {
        List<Door> prizelessDoors = ListPrizelessDoors();
        foreach (Door door in prizelessDoors)
        {
            if (door.isCorrect) Debug.LogError("correct door open error");
            door.OpenDoor(true);
        }
    }

    // Resets the scale of all doors.
    public void ResetAllDoorsHoverAnimation()
    {
        foreach (Door component in doorList)
            component.transform.localScale = Vector3.one;
    }

    /*
     * It appends doors to the list that do not include the door you selected and have no rewards behind them.
     * If the door you select contains a reward, a random door will be removed from the list. 
     * The door that remains is the one where you will be asked if you want to change your choice.
     */
    private List<Door> ListPrizelessDoors()
    {
        List<Door> _prizelessDoors = new List<Door>();
        Door _selected = MontyhallManager.Instance.selected.GetComponent<Door>();

        foreach (Door currentDoor in doorList)
        {
            if (_selected != currentDoor && !CheckForPrizeDoor(currentDoor))
            {
                _prizelessDoors.Add(currentDoor);
            }
        }

        if (CheckForPrizeDoor(_selected))
        {
            int rnd = Random.Range(0, _prizelessDoors.Count);
            _prizelessDoors.Remove(_prizelessDoors[rnd]);
        }

        return _prizelessDoors;
    }

    // Destroys all doors on scene.
    private void RemoveAllDoors()
    {
        if (doorList != null)
            foreach (Door door in doorList)
                Destroy(door.gameObject); // Object pool optimization is required.

        doorList.Clear();
    }
}