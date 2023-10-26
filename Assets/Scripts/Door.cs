using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MontyhallManager;

public class Door : MonoBehaviour
{
    public bool isCorrect;
    public int index;
    [SerializeField] Animator anim;

    public void OpenAnimation() => anim.SetTrigger("open");
    public void CloseAnimation() => anim.SetTrigger("close");

    private void OnMouseEnter()
    {
        if (Instance.CurrentGameState == GameState.SELECT_DOOR)
        {
            DoorManager.Instance.ResetAllDoorScale();
            transform.localScale = new Vector3(1.1f, 1.1f, 1);
        }
    }

    private void OnMouseExit()
    {
        transform.localScale = Vector3.one;
    }
}
