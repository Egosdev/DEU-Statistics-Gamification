using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MontyhallManager;

public class Door : MonoBehaviour
{
    public bool isCorrect;
    public int index;
    [SerializeField] Animator anim;

    public void OpenDoor(bool open) => anim.SetBool("door", open);
    public void OpenPrizeDoor(bool open) => anim.SetBool("prize", open);

    public bool IsOpen() => anim.GetBool("door");


    private void OnMouseEnter()
    {
        if (IsOpen()) return;

        if (Instance.CurrentGameState == GameState.SELECT_DOOR || Instance.CurrentGameState == GameState.KEEP_OR_CHANGE)
        {
            DoorManager.Instance.ResetAllDoorsHoverAnimation();
            transform.localScale = new Vector3(1.1f, 1.1f, 1);
        }
    }

    private void OnMouseExit()
    {
        if (Instance.CurrentGameState == GameState.SELECT_DOOR || Instance.CurrentGameState == GameState.KEEP_OR_CHANGE)
        {
            transform.localScale = Vector3.one;
        }
    }
}
