using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _walkSpeed;
    [SerializeField]
    private string _id;
    public string Id 
    {
        get
        {
            return _id;
        }
    }
    private int _score = 0;
    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
        }
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
            switch (Camera.PlayerIndex)
            {
                case 0:
                    Camera.GameObject.transform.Translate(Vector3.left * _walkSpeed * Time.deltaTime);
                    break;
                case 1:
                    Camera.GameObject.transform.Translate(Vector3.forward * _walkSpeed * Time.deltaTime);
                    break;
                case 2:
                    Camera.GameObject.transform.Translate(Vector3.right * _walkSpeed * Time.deltaTime);
                    break;
                case 3:
                    Camera.GameObject.transform.Translate(Vector3.back * _walkSpeed * Time.deltaTime);
                    break;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            switch (Camera.PlayerIndex)
            {
                case 0:
                    Camera.GameObject.transform.Translate(Vector3.right * _walkSpeed * Time.deltaTime);
                    break;
                case 1:
                    Camera.GameObject.transform.Translate(Vector3.back * _walkSpeed * Time.deltaTime);
                    break;
                case 2:
                    Camera.GameObject.transform.Translate(Vector3.left * _walkSpeed * Time.deltaTime);
                    break;
                case 3:
                    Camera.GameObject.transform.Translate(Vector3.forward * _walkSpeed * Time.deltaTime);
                    break;
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            switch (Camera.PlayerIndex)
            {
                case 0:
                    Camera.GameObject.transform.Translate(Vector3.forward * _walkSpeed * Time.deltaTime);
                    break;
                case 1:
                    Camera.GameObject.transform.Translate(Vector3.right * _walkSpeed * Time.deltaTime);
                    break;
                case 2:
                    Camera.GameObject.transform.Translate(Vector3.back * _walkSpeed * Time.deltaTime);
                    break;
                case 3:
                    Camera.GameObject.transform.Translate(Vector3.left * _walkSpeed * Time.deltaTime);
                    break;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            switch (Camera.PlayerIndex)
            {
                case 0:
                    Camera.GameObject.transform.Translate(Vector3.back * _walkSpeed * Time.deltaTime);
                    break;
                case 1:
                    Camera.GameObject.transform.Translate(Vector3.left * _walkSpeed * Time.deltaTime);
                    break;
                case 2:
                    Camera.GameObject.transform.Translate(Vector3.forward * _walkSpeed * Time.deltaTime);
                    break;
                case 3:
                    Camera.GameObject.transform.Translate(Vector3.right * _walkSpeed * Time.deltaTime);
                    break;
            }
        }
    }

    // 득점
    public void Scores(ref GameObject conceder)
    {
        PlayerController concederPlayer = conceder.GetComponent<PlayerController>();

        // 득점자는 +2점, 실점자는 -1점
        if(concederPlayer.Id != _id)
        {
            _score += 2;
            /* 득점에 대한 메시지 */
            Debug.Log("득점자: " + _id + "   실점자: " + concederPlayer.Id);
        }
        // 자책골, 자책골은 득점으로 인정하지 않음
        else
        {
            /* 자책골에 대한 메시지 */
            Debug.Log(_id + "의 자책골");
        }
        concederPlayer.Score--;

        Debug.Log(_id + ": " + _score + "    " + concederPlayer.Id + ": " + concederPlayer.Score);
    }

}
