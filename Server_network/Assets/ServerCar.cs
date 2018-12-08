using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerCar : MonoBehaviour {

    public int Carnumber;
    public float speed, max_speed, brakeSpeed;
    public int go_direction, go_front, count, isContect; //서버차의 방향
    public float now_rotate;
    public bool canControl;
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
        brakeSpeed = 0.8f;
        canControl = true;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponentInChildren<Prevent>().SetSpeed(speed); //매번마다 레드레인지의 스피드를 바꿔주기
        if (go_front == 0)
        {
            speed = 0;
        }
        if (isContect == 1) // 서버카가 일정 구간을 돌기위해 설치해둔 오브젝트에 충돌하게 되면,
        {
            go_front = 0; //잠시 돌기위해 앞으로 가지 말게 해주고
            transform.Rotate(0, 0, 1);//좌방향 회전
            if (System.Math.Round(transform.localEulerAngles.z) % 90 == 0) { isContect = 0; go_front = 1; now_rotate = transform.localEulerAngles.z; } //90도가 되면, 다시 되돌려주기.
        }
        if (go_front == 1 && canControl)//앞으로 가며, 컨트롤을 할수 있다면,
        {
            transform.localRotation = Quaternion.Euler(0, 0, now_rotate); //클라이언트 차에 치여 rotation이 바뀌지 않도록 해준다.
            if (speed < 0.5f) speed = 0.5f; 
            else if (speed < max_speed) speed *= 1.1f;//스피드를 증가시켜 부드럽게 출발할수 있도록 해준다.
        }

        //차위치 변환
        transform.Translate(0, speed, 0);

    }
    public void SetSpeed(float Carspeed)
    {
        this.speed = Carspeed;
    }
    public void SetControl(bool acanControl) //prevent 클래스에서 컨트롤에 참여할수 있도록 해주는 것.
    {
        canControl = acanControl;
    }
}
