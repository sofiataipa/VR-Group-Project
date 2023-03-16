using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCPMoveObject : MonoBehaviour
{
    public float moveDuration = 2;
    public Vector3 targetLocalPosition;
    private Vector3 originalLocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        originalLocalPosition = transform.localPosition;
    }


    public void MoveTo()
    {
        Move(originalLocalPosition, targetLocalPosition);
    }

    public void MoveBack()
    {
        Move(targetLocalPosition, originalLocalPosition);
    }

    public void Move(Vector3 In, Vector3 Out)
    {
        StartCoroutine(MoveRoutine(In, Out));
    }

    IEnumerator MoveRoutine(Vector3 In, Vector3 Out)
    {
        float timer = 0;
        while (timer <= moveDuration)
        {

            Vector3 newPos = Vector3.Lerp(In, Out, timer / moveDuration);

            transform.localPosition = newPos;

            timer += Time.deltaTime;
            yield return null;
        }

    }
}
