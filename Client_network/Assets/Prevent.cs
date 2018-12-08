using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prevent : MonoBehaviour
{
    GameObject Car;
    private int car_number;
    private bool canControl;
    public double x, brake_Speed;
    Vector3 initScale, initpos;
    public float speed,brakingDistance, initScaleY;
    // Use this for initialization
    void Start() //initial
    {
        Car = GameObject.Find("car_main");
        speed = 0f;
        brakingDistance = 0f;
        x = 0;
        initScale = transform.localScale;
        initScaleY = initScale.y;
        initpos = transform.localPosition;
        canControl = true;
    }
    void Update()
    {
        Car.GetComponent<Car>().canControl = canControl;
        brakingDistance = Calculate_BrakingDistance(); //현재 브레이크 거리 세팅
        if (speed < 0) //스피드가 0보다 아래일 때는, 뒤로 가고 있으므로,
        {
            initScale.Set(initScale.x, initScaleY + Math.Abs(speed) / 20, initScale.z); //속도를 늘릴 수록 크기를 크게 해준다.
            initpos.Set(0, -(brakingDistance / 45), 0); //제동거리를 뒷부분에 위치.
        }
        else
        {
            //그반대.
            initScale.Set(initScale.x, initScaleY + Math.Abs(speed) / 20, initScale.z);
            initpos.Set(0, brakingDistance / 45, 0);
        }

        //위치와 크기모두 바꿔줌.
        transform.localRotation = Quaternion.Euler(0, 0, 0); //rotation이 변하지 않도록 고정해주는 것.
        transform.localPosition = initpos;
        transform.localScale = initScale;
    }
    public void SetSpeed(float Carspeed)
    {
        this.speed = Carspeed;
    }
    private float Calculate_BrakingDistance() //제동거리 구하기 (적분을 통해서 구함)
    {
        if  (Math.Abs(speed) < 0.01) return 0;
        x = Math.Log(1 / Math.Abs(speed), brake_Speed);
        return (float)(Math.Abs(speed) * ((Math.Pow(brake_Speed, x) / Math.Log(brake_Speed)) - (1 / Math.Log(brake_Speed))));
    }
    void OrnTriggeEnter2D(Collider2D others) //만약 게임 오브젝트가 부딪히면
    {
        if (others.gameObject.tag == "Car") //차랑 부딪히면 speed를 줄임
        {

            if (Math.Abs(speed) > 0.1f) //만약 거의 0이 아니면
            {
                speed *= (float)brake_Speed; //속도를 줄임
            }
            else speed = 0;
            canControl = false;
        }
        if (others.gameObject.tag == "Line") //선이랑 부딪히면 
        {

            if (Math.Abs(speed) > 0.1f) //만약 거의 0이 아니면
            {
                speed *= (float)brake_Speed; //속도를 줄임
            }
            else speed = 0;
            canControl = false;
        }
        Car.GetComponent<Car>().SetSpeed(speed);//차의 속도를 바꿔줌
    }
    void OnTriggerStay2D(Collider2D others) //enter이랑 똑같
    {
        if (others.gameObject.tag == "Car")
        {

            if (Math.Abs(speed) > 1f) //만약 거의 0이 아니면
            {
                speed *= (float)brake_Speed; //속도를 줄임
            }
            else speed = 0;
            canControl = false;
        }
        if (others.gameObject.tag == "Line")
        {
            if (Math.Abs(speed) > 1f)
            {
                speed *= (float)brake_Speed;
            }
            else speed = 0;
            canControl = false;
        }
        Car.GetComponent<Car>().SetSpeed(speed);
    }

    void OnTriggerExit2D(Collider2D others) //만약 게임 오브젝트가 부딪히면
    {
        if (others.gameObject.tag == "Car") //차랑 부딪히면 speed를 줄임
        {
            canControl = true;
        }
        if (others.gameObject.tag == "Line") //선이랑 부딪히면 
        {
            canControl = true;
        }
    }
}