using UnityEngine;
using static MontyhallManager;

public class Door : MonoBehaviour
{
    [SerializeField] Animator anim;

    public bool isCorrect;
    public int index;

    public void Open(bool open)
    {
        if (isCorrect) anim.SetBool("prize", open);
        else anim.SetBool("door", open);
    }
        
    public bool IsOpen() => anim.GetBool("door") || anim.GetBool("prize");

    public void ResetScale() => transform.localScale = Vector3.one;

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
            ResetScale();
        }
    }
}
