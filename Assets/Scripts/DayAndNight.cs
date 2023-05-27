using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{
    public enum TimeState { Day, Evening, Night, Dawn}
    public static TimeState tState = TimeState.Day;
    public float timeSpeed;

    private float elpasedTime;  //����ð�
    private float r = 1f;       //r,g,b �����
    private float g = 1f;
    private float b = 1f;

    public GameObject skyDome;  //��ī�̵�
    private Material skyDomeMaterial;   //��ī�̵��� ���׸���
    private float offsetValueX = 0;

    void Start()
    {
        skyDomeMaterial = skyDome.GetComponent<Renderer>().material;
        //��ī�̵� ���׸����� �����¿� �����ϴ� ���
        //�����¿��� �����ؼ� �� ����
        skyDomeMaterial.SetTextureOffset("_MainTex", new Vector2(offsetValueX, 0));

        StartCoroutine(Day());
    }

    //��
    IEnumerator Day()
    {
        tState = TimeState.Day;

        skyDomeMaterial.SetTextureOffset("_MainTex", new Vector2(offsetValueX, 0));

        while (true)
        {
            elpasedTime += Time.deltaTime * timeSpeed;

            if (elpasedTime >= 400)
            {
                StopAllCoroutines();
                elpasedTime = 0;
                StartCoroutine(DayToNight());
                break;
            }

            yield return null;
        }
    }

    //�� -> ��
    IEnumerator DayToNight()
    {
        tState = TimeState.Evening;
        print(tState);
        while (true)
        {
            elpasedTime += Time.deltaTime * timeSpeed;

            RenderSettings.ambientLight = new Color(r, g, b, 1);
            r -= r / 1000;
            g -= g / 1000;
            b -= b / 1000;

            if (r <= 0 || g <= 0 || b <= 0)
            {
                r = 0;
                g = 0;
                b = 0;
            }

            offsetValueX += 0.0025f * Time.deltaTime * timeSpeed;
            skyDomeMaterial.SetTextureOffset("_MainTex", new Vector2(offsetValueX, 0));

            if (elpasedTime >= 200)
            {
                offsetValueX = 0.5f;
                elpasedTime = 0;

                StartCoroutine(Night());
                break;
            }

            yield return null;
        }
    }

    //��
    IEnumerator Night()
    {
        tState = TimeState.Night;
        print(tState);

        skyDomeMaterial.SetTextureOffset("_MainTex", new Vector2(offsetValueX, 0));

        while (true)
        {
            elpasedTime += Time.deltaTime * timeSpeed;


            if (elpasedTime >= 400)
            {
                elpasedTime = 0;
                StartCoroutine(NightToDay());
                break;
            }

            yield return null;
        }
    }

    //�� -> ��
    IEnumerator NightToDay()
    {
        tState = TimeState.Dawn;
        print(tState);

        while (true)
        {
            elpasedTime += Time.deltaTime * timeSpeed;

            RenderSettings.ambientLight = new Color(r, g, b, 1);
            r += r / 1000;
            g += g / 1000;
            b += b / 1000;

            if (r >= 1 || g >= 1 || b >= 1)
            {
                r = 1;
                g = 1;
                b = 1;
            }

            offsetValueX -= 0.0025f * Time.deltaTime * timeSpeed;
            skyDomeMaterial.SetTextureOffset("_MainTex", new Vector2(offsetValueX, 0));

            if (elpasedTime >= 200)
            {
                offsetValueX = 0;
                elpasedTime = 0;

                StartCoroutine(Day());
                break;
            }

            yield return null;
        }
    }

    
}