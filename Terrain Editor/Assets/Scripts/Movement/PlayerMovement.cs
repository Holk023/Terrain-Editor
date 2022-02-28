using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{

    #region Variables

    [Header("Komponenty:")]
    [SerializeField] private Rigidbody myRigidbody;
    [SerializeField] private float speed;
    [SerializeField] private Camera myCamera;
    public GameObject cameraBrain;

    #endregion

    public void InputUpdate()
    {

       if (Input.GetKey(KeyCode.W))
		{
            myRigidbody.MovePosition(transform.position + (myCamera.transform.forward * speed) * Time.fixedDeltaTime);
		}
        if (Input.GetKey(KeyCode.S))
        {
            myRigidbody.MovePosition(transform.position + (-myCamera.transform.forward * speed) * Time.fixedDeltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            myRigidbody.MovePosition(transform.position + (myCamera.transform.right * speed) * Time.fixedDeltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            myRigidbody.MovePosition(transform.position + (-myCamera.transform.right * speed) * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            myRigidbody.MovePosition(transform.position + (Vector3.up * speed) * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            myRigidbody.MovePosition(transform.position + (-Vector3.up * speed) * Time.fixedDeltaTime);
        }

        myRigidbody.velocity = Vector3.zero;
    }
}
