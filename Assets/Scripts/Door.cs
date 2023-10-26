using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MontyhallManager;

public class Door : MonoBehaviour
{
    public bool isCorrect;
    public int index;
    [SerializeField] Animator anim;

    public void SetAnimation(bool open) => anim.SetBool("door", open);

    private void OnMouseEnter()
    {
        if (anim.GetBool("door")) return;

        if (Instance.CurrentGameState == GameState.SELECT_DOOR || Instance.CurrentGameState == GameState.KEEP_OR_CHANGE)
        {
            DoorManager.Instance.ResetAllDoorScale();
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