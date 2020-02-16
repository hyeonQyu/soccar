using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _walkSpeed;

    // Use this for initialization
    void Start()
    {
 
    }




    // Update is called once per frame
    void Update()
    {

        Move();

    }

    private void Move()
    {

        if (Input.GetKey(KeyCode.A))
        {
            switch (Camera._playerIndex)
            {
                case 0:
                    Camera._gameObject.transform.Translate(Vector3.left * _walkSpeed * Time.deltaTime);
                    break;
                case 1:
                    Camera._gameObject.transform.Translate(Vector3.forward * _walkSpeed * Time.deltaTime);
                    break;
                case 2:
                    Camera._gameObject.transform.Translate(Vector3.right * _walkSpeed * Time.deltaTime);
                    break;
                case 3:
                    Camera._gameObject.transform.Translate(Vector3.back * _walkSpeed * Time.deltaTime);
                    break;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            switch (Camera._playerIndex)
            {
                case 0:
                    Camera._gameObject.transform.Translate(Vector3.right * _walkSpeed * Time.deltaTime);
                    break;
                case 1:
                    Camera._gameObject.transform.Translate(Vector3.back * _walkSpeed * Time.deltaTime);
                    break;
                case 2:
                    Camera._gameObject.transform.Translate(Vector3.left * _walkSpeed * Time.deltaTime);
                    break;
                case 3:
                    Camera._gameObject.transform.Translate(Vector3.forward * _walkSpeed * Time.deltaTime);
                    break;
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            switch (Camera._playerIndex)
            {
                case 0:
                    Camera._gameObject.transform.Translate(Vector3.forward * _walkSpeed * Time.deltaTime);
                    break;
                case 1:
                    Camera._gameObject.transform.Translate(Vector3.right * _walkSpeed * Time.deltaTime);
                    break;
                case 2:
                    Camera._gameObject.transform.Translate(Vector3.back * _walkSpeed * Time.deltaTime);
                    break;
                case 3:
                    Camera._gameObject.transform.Translate(Vector3.left * _walkSpeed * Time.deltaTime);
                    break;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            switch (Camera._playerIndex)
            {
                case 0:
                    Camera._gameObject.transform.Translate(Vector3.back * _walkSpeed * Time.deltaTime);
                    break;
                case 1:
                    Camera._gameObject.transform.Translate(Vector3.left * _walkSpeed * Time.deltaTime);
                    break;
                case 2:
                    Camera._gameObject.transform.Translate(Vector3.forward * _walkSpeed * Time.deltaTime);
                    break;
                case 3:
                    Camera._gameObject.transform.Translate(Vector3.right * _walkSpeed * Time.deltaTime);
                    break;
            }
        }
    }

}
