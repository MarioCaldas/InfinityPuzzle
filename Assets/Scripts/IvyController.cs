using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvyController : MonoBehaviour
{
    MeshRenderer rend;

    public float growValue;

    public bool playAnim = false;

    bool aux = false;


    private ParticleSystem leafsParticles;

    public Material growMat;

    public Material windMat;

    [SerializeField] float windBendStrengt;

    private float windBendStrengtMax;


    public bool increaseWind = false;

    public bool leafsBool;


    void Start()
    {
        windBendStrengtMax = 0.6f;

        rend = GetComponent<MeshRenderer>();

        growValue = rend.material.GetFloat("Vector1_386C6BEE");

        leafsParticles = Resources.Load<ParticleSystem>("LeafsParticles");

        windBendStrengt = 0;

    }

    public void PlayIvyAnim(bool play)
    {
        playAnim = play;
    }

    public void ResetVars()
    {
        growValue = 0;
        GetComponent<Renderer>().material = growMat;
        rend.material.SetFloat("Vector1_386C6BEE", -0.086f);
        //growValue = rend.material.GetFloat("Vector1_386C6BEE");
        aux = false;
        playAnim = false;

        windBendStrengt = 0;
        increaseWind = false;
    }

    public void PlayParticles()
    {
        Vector3 spawnPos = new Vector3(transform.parent.parent.transform.position.x, transform.parent.parent.transform.position.y, transform.parent.parent.transform.position.z - 0.2f);

        if (growValue > .4)
            Instantiate(leafsParticles, spawnPos, transform.parent.parent.rotation * Quaternion.Euler(169f, 0, 0f));

    }

    void Update()
    {
        if (playAnim)
        {
            if (growValue <= 1)
            {
                growValue += 1.3f * Time.deltaTime;
                rend.material.SetFloat("Vector1_386C6BEE", growValue);

                if (growValue >= 0.75 && aux == false)
                {
                    aux = true;
                }
            }
            else
            {
                if (leafsBool)
                {
                    increaseWind = true;
                    GetComponent<Renderer>().material = windMat;
                    windBendStrengt = 0.6f;
                    rend.material.SetFloat("Vector1_A3ACDC04", windBendStrengt);
                }

                playAnim = false;
            }

        }
    }
}
