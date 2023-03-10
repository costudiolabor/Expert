using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private Transform _transform;
    void Start()
    {
        _transform = transform;
    }

    void Update()
    {
        _transform.Rotate(new Vector3(1,1.5f,2));
    }
}
