using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCtrl : MonoBehaviour
{
    //����ũ ��ƼŬ ������ ������ ����
    public GameObject sparkEffect;


    [HideInInspector] public bool m_IsColl = false;
    Material m_WMaterial = null;

    // Start is called before the first frame update
    void Start()
    {
        m_WMaterial = gameObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //�浹�� ������ �� �߻��ϴ� �̺�Ʈ
    void OnCollisionEnter(Collision coll)
    {
        //�浹�� ���ӿ�����Ʈ�� �±װ� ��
        if (coll.collider.tag == "BULLET")
        {
            //����ũ ��ƼŬ�� �������� ����
            GameObject spark = Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);

            //ParticleSystem ������Ʈ�� ����ð�(duration)�� ���� �� ���� ó��
            Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

            //�浹�� ���ӿ�����Ʈ ����
            Destroy(coll.gameObject);
        }
    }


    public void AlphaOnOff(bool IsOn = true)
    {

        if (m_WMaterial == null)
            return;

        if (IsOn == true )
        {
            m_WMaterial.SetFloat("_Mode", 3); //opaque
            m_WMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            m_WMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            m_WMaterial.SetInt("_ZWrite", 0);
            m_WMaterial.DisableKeyword("_ALPHATEST_ON");
            m_WMaterial.DisableKeyword("_ALPHABLEND_ON");
            m_WMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            m_WMaterial.renderQueue = 3000;
            m_WMaterial.color = new Color(1,1,1,0.3f);

        }
        else
        {
            m_WMaterial.SetFloat("_Mode", 0);//transparent
            m_WMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            m_WMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);

            m_WMaterial.SetInt("_ZWrite", 1);
            m_WMaterial.DisableKeyword("_ALPHATEST_ON");
            m_WMaterial.DisableKeyword("_ALPHABLEND_ON");

            m_WMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON"); 
            m_WMaterial.renderQueue = -1;
            m_WMaterial.color = new Color(1,1,1,1);

        }

    }



}
