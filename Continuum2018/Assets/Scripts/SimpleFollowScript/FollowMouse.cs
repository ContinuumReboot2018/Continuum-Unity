using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Vector3 _mousePosition;
    public float MoveSpeed = 2f;

    void Update()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mousePosition.z = 0;
        transform.position = Vector3.MoveTowards(transform.position, _mousePosition, MoveSpeed * Time.deltaTime);
    }
}
