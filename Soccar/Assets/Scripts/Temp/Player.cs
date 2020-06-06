using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{


    //플레이어 움직임속도
    public float moveSpeed = 5f;
    //플레이어 방향회전속도
    public float rotationSpeed = 90f;

    private bool isRotate = false;
    //건들지 마시오
    float animSpeed = 1.5f;

    Animator animator;

    // Use this for initialization
    void Start()
    {
        //"캐릭터 애니메이터 컴포넌트를 사용하겠다"
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Sensitivity = "+Input.GetAxis("Sensitivity"));

        //움직이자!
        float horizontal = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        //transform.Rotate(0, horizontal, 0);
        if(!isRotate){transform.Rotate(0, horizontal, 0);}
        //움직이자!(2)
        float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(0, 0, vertical); ;

            //      축(방향)
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            //애니메이터 파라미터
            animator.SetFloat("Speed", v);

        }
    }
