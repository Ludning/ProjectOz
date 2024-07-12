using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    private Rigidbody rigid;
    [SerializeField]
    private float moveForce = 100.0f;
    private float x_Axis;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        x_Axis = Input.GetAxis("Horizontal");

        Vector3 velocity = new Vector3(x_Axis, 0, 0);
        velocity *= moveForce;
        rigid.velocity = velocity;
    }
}
