//Car
//Client가 운영하는 차를 부드럽게 조작하기 위한 클래스.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Car : MonoBehaviour
{
    GameObject RedRange; //레드레인지 오브젝트
    public float speed, max_speed, brakeSpeed, sumSpeed;
    //brakeSpeed는 브레이크를 밟을 때 줄어들도록 만든 임의의 float값
    //sumSpeed는 앞으로 나가거나 뒤로 가기 시작할때 부드럽게 시작하기 위한 임의의 float값
    public bool canControl; //컨트롤을 할수 있냐 없냐를 따지는 변수
    // Use this for initialization
    void Start()//initial
    {
        RedRange = GameObject.Find("MainRedRange");
        brakeSpeed = 0.8f;
        this.GetComponentInChildren<Prevent>().brake_Speed = brakeSpeed;
        speed = 0f;
        max_speed = 15f;//사용자가 지정한 속도
        sumSpeed = 1.1f;
        canControl = true;
    }

    // Update is called once per frame
    void Update()
    {
        RedRange.GetComponent<Prevent>().SetSpeed(speed); //매번마다 레드레인지의 스피드를 정해주기
        if (Input.GetKey(KeyCode.UpArrow))//속도 올리기
        {
            if (max_speed < 25f)
            {
                max_speed += 1f;
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))//속도 올리기
        {
            if (max_speed > 1f)
            {
                max_speed -= 1f;
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (speed < 0) //차가 뒤로 가고 잇으면 속도를 줄여줌
            {
                speed *= brakeSpeed;
                if (speed > -1) //전체적으로 멈추면 -1을 곱해서 앞으로 나가게 함
                {
                    speed = 0;
                }
            }
            else
            {
                if (canControl) //앞으로 나갈떄에는 속도를 올려줌
                {
                    if (speed < 1) speed = 1;
                    else if (speed < max_speed) speed *= sumSpeed;
                }
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, 0, 1f);
            //회전
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 0, -1f);//회전
        }
        if (Input.GetKey(KeyCode.S))
        {//브레이크 밎 뒤로 가기(앞으로 갈떄랑 똑같아요)
            if (speed > 0)
            {
                speed *= brakeSpeed;
                if (speed < 1)
                {
                    speed = 0;
                }
            }
            else
            {
                if (canControl) //앞으로 나갈떄에는 속도를 올려줌
                {
                    if (speed > -1) speed = -1;
                    else if (speed > -1 * max_speed) speed *= sumSpeed;
                }
            }

        }
        else if (Input.GetKey(KeyCode.Space))
        {//브레이크
            speed *= brakeSpeed;
            if (speed < 1 && speed > -1)
            {
                speed = 0;
            }
        }
        transform.Translate(0, speed, 0);
    }
    public void SetSpeed(float Carspeed)
    {
        this.speed = Carspeed;
    }
    public void SetControl(bool acanControl)
    {
        canControl = acanControl;
    }
}