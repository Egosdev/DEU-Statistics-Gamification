using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSign : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] Vector2 range;

    void Start()
    {
        Tween.LocalPosition(transform, transform.localPosition + new Vector3(0, Random.Range(range.x, range.y), 0), duration, 0, Tween.EaseInOut, Tween.LoopType.PingPong);
    }
}
