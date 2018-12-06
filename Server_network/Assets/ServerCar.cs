using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerCar : MonoBehaviour {

    public int Carnumber;
    public float speed, max_speed, brakeSpeed;
    public int go_direction, go_front, count, isContect; //서버차의 방향
    public float now_rotate;
    // Use this for initialization
    void Start()
    {
        now_rotate = this.transform.localEulerAngles.z;
        speed = 0f;
        max_speed = 6f;//서버가 지정한 속도
        go_direction = 0;
        go_front = 1;
        count = 0;
        isContect = 0;
        brakeSpeed = 0.9f;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponentInChildren<Prevent>().SetSpeed(speed); //매번마다 레드레인지의 스피드를 바꿔주기
        if (go_front == 0)
        {
            speed = 0;
        }
        if (isContect == 1)
        {
            go_front = 0;
            count++;
            transform.Rotate(0, 0, 1);//좌방향 회전
            if (count == 90) { isContect = 0; count = 0; go_front = 1; now_rotate = transform.localEulerAngles.z; }
        }
        if (go_front == 1)
        {
            transform.localRotation = Quaternion.Euler(0, 0, now_rotate);
            if (speed < 0.5f) speed = 0.5f;
            else if (speed < max_speed) speed *= 1.1f;
        }
        transform.Translate(0, speed, 0);

    }
    public float getBrakeSpeed()
    {
        return brakeSpeed;
    }
    public void SetSpeed(float Carspeed)
    {
        this.speed = Carspeed;
    }

}
