using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    struct carInfo
    {
        TcpClient clientSocket;
        public bool isalive;
        public float x, y;
        public float speed;
        public float angle;

        public void setInfo(bool aisalive, float ax, float ay, float aspeed, float aangle)
        {
            this.isalive = aisalive;
            this.x = ax;
            this.y = ay;
            this.speed = aspeed;
            this.angle = aangle;
        }

        public void setSocket(TcpClient temp)
        {
            clientSocket = temp;
            isalive = true;
        }

        public void delInfo()
        {
            clientSocket = null;
            isalive = false;
        }

        public String toString()
        {
            return isalive + " " + x + " " + y + " " + speed + " " + angle;
        }
    }
    TcpClient client;
    carInfo carStat;
    NetworkStream stream;
    GameObject[] CarObject;
    GameObject[] ServerCarObject;
    int Carnumber, start_first;
    public static GameObject car;
    Thread clientthr;
    StreamWriter sw;
    StreamReader sr;
    carInfo[] ServerCarArr;
    carInfo[] carArr;
    public int Grade;
    public bool isEnd, ChangeEndScene, ChangeNoScene;
    // Use this for initialization
    void Start()
    {
        ChangeNoScene = false;
        ChangeEndScene = false;
        isEnd = false;
        ServerCarArr = new carInfo[2];
        carArr = new carInfo[8];
        carStat.setSocket(null);
        for (int index = 0; index < 8; index++)//initalize
        {
            carArr[index].delInfo();
        }
        CarObject = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            CarObject[i] = GameObject.Find("OtherCar" + i);
        }
        ServerCarObject = new GameObject[2];
        for (int i = 0; i < 2; i++)
        {
            ServerCarObject[i] = GameObject.Find("ServerCar" + i);
        }
        car = GameObject.Find("car_main");
        client = new TcpClient("127.0.0.1", 5004);//일단 시험용으로 자신에게 접속(소켓새성)
        clientthr = new Thread(new ParameterizedThreadStart(clientThread));
        clientthr.Start(client);
        start_first = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnd && ChangeEndScene)
        {
            GameObject.Find("Grade").GetComponent<Grade>().grade = Grade;
            SceneManager.LoadScene("End");
        }
        if (ChangeNoScene)
        {
            SceneManager.LoadScene("endScence");
        }
        //처음 시작 판단 후 넘버에 따른 첫 위치 지정해주기.
        if (start_first == 1)
        {
            start_first = 0;
            SetFirstLocation();
        }
        for (int i = 0; i < 8; i++)
        {
            if (carArr[i].isalive && i != Carnumber) CarObject[i].SetActive(true);//지금사용하고 있으면 나타내주고
            else CarObject[i].SetActive(false);
        }
        //현재차 정보 넣기
        carStat.setInfo(true, car.transform.position.x, car.transform.position.y, car.GetComponent<Carspeed>().speed, car.transform.localEulerAngles.z);
        //서버카 위치 수정
        for (int i = 0; i < 2; i++)
        {
            ServerCarObject[i].transform.position = new Vector3(ServerCarArr[i].x, ServerCarArr[i].y, 0);
            ServerCarObject[i].transform.localRotation = Quaternion.Euler(0, 0, ServerCarArr[i].angle);
            ServerCarObject[i].GetComponentInChildren<OtherCarPrevent>().SetSpeed(ServerCarArr[i].speed);
        }
        //다른카 위치 수정
        for (int i = 0; i < 8; i++)
        {
            if (i == Carnumber) break;

            CarObject[i].transform.position = new Vector3(carArr[i].x, carArr[i].y, 0);
            CarObject[i].transform.localRotation = Quaternion.Euler(0, 0, carArr[i].angle);
            CarObject[i].GetComponentInChildren<OtherCarPrevent>().SetSpeed(carArr[i].speed);
        }
    }

    void clientThread(object temp)
    {
        client = temp as TcpClient;
        stream = client.GetStream();
        sw = new StreamWriter(stream);
        sr = new StreamReader(stream);
        try
        {
            String CanGoString = sr.ReadLine();
            if (String.Equals(CanGoString, "Sorry")) { ChangeNoScene = true; }
            else if(String.Equals(CanGoString, "Start"))
            {
                Carnumber = int.Parse(sr.ReadLine());
                Debug.Log(Carnumber);
                start_first = 1;

                while (true)
                {
                    if (isEnd)
                    {
                        sw.WriteLine("End");
                        sw.Flush();
                        Grade = int.Parse(sr.ReadLine());
                        ChangeEndScene = true;
                    }
                    else
                    {
                        sw.WriteLine(carStat.toString());
                        sw.Flush();

                        for (int i = 0; i < 2; i++)
                        {
                            String msg = sr.ReadLine();//입력받기
                            int num = int.Parse(msg.Substring(0, 1));
                            StringtoStructor(num, msg.Substring(1), 0);
                        }
                        for (int index = 0; index < 8; index++)
                        {
                            String msg = sr.ReadLine();//입력받기
                            int num = int.Parse(msg.Substring(0, 1));
                            StringtoStructor(num, msg.Substring(1), 1);
                        }
                        Thread.Sleep(20);
                    }
                }
            }
        }
        catch (Exception e) { Debug.Log(e.Message); }
            stream.Close();
            client.Close();
            sw.Close();
            sr.Close();
    }
    void SetFirstLocation()
    {
        if (Carnumber == 0) { car.transform.localPosition = new Vector3(-8269, 4590, 0); car.transform.localRotation = Quaternion.Euler(0, 0, -90); }//new Quaternion(0, 0, -90, 0); }
        else if (Carnumber == 1) { car.transform.localPosition = new Vector3(-3160, 6369, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 180); }
        else if (Carnumber == 2) { car.transform.localPosition = new Vector3(-3160, 6369, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 180); }
        else if (Carnumber == 3) { car.transform.localPosition = new Vector3(8269, 4590, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 90); }
        else if (Carnumber == 4) { car.transform.localPosition = new Vector3(-8269, -4590, 0); car.transform.localRotation = Quaternion.Euler(0, 0, -90); }
        else if (Carnumber == 5) { car.transform.localPosition = new Vector3(-3160, -6369, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 180); }
        else if (Carnumber == 6) { car.transform.localPosition = new Vector3(3160, -6369, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 180); }
        else if (Carnumber == 7) { car.transform.localPosition = new Vector3(8269, -4590, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 90); }
    }

    void StringtoStructor(int index, String a, int inputArr)
    {
        String[] arr = a.Split(' ');
        if (inputArr == 0) //0은 서버카 정리
            ServerCarArr[index].setInfo(Boolean.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]), float.Parse(arr[4]));
        if (inputArr == 1) //1은 다른 차들 위치 정리
            carArr[index].setInfo(Boolean.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]), float.Parse(arr[4]));
    }


    void Application_ApplicationExit(object sender, EventArgs e)
    {
        try
        {
            clientthr.Abort();
        }
        catch { }
    }

    void OnApplicationQuit()//어플리케이션이 꺼지면 소켓 ,스트림, 스레드 모두종료
    {
        carStat.delInfo();
        stream.Close();
        client.Close();
        sw.Close();
        sr.Close();
        clientthr.Abort();
    }
    public String getCarinfo(int carNumber)
    {
        return carArr[carNumber].toString();
    }
}