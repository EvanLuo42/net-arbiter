using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerashake : MonoBehaviour
{
    public static bool startShake = false;  //camera�Ƿ�ʼ��
    public static float seconds = 0f;    //�𶯳�������
    public static bool started = false;    //�Ƿ��Ѿ���ʼ��
    public static float quake = 0.2f;       //��ϵ��

    private Vector3 camPOS;  //camera����ʼλ��

    // Use this for initialization
    void Start()
    {
        camPOS = transform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (startShake)
        {
            transform.localPosition = camPOS + Random.insideUnitSphere * quake;
        }

        if (started)
        {
            StartCoroutine(WaitForSecond(seconds));
            started = false;
        }
    }
    public static void ShakeFor(float a, float b)
    {
        //		if (startShake)
        //			return;
        seconds = a;
        started = true;
        startShake = true;
        quake = b;
    }
    IEnumerator WaitForSecond(float a)
    {
        yield return new WaitForSeconds(a);
        startShake = false;
        transform.localPosition = camPOS;
    }
}
