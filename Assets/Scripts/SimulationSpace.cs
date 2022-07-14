using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSpace : MonoBehaviour
{
    public GravitationalObject sun = null;
    public GameObject dottedLinePrefab;
    public Material planetMaterial;
    public bool showTrails = true;
    // Start is called before the first frame update
    void Start()
    {
        CheckChildren();
    }

    // Update is called once per frame
    void Update()
    {
        CheckChildren();   
    }

    void CheckChildren()
    {
        GravitationalObject[] gravitationalObjects = new GravitationalObject[transform.childCount];
        int i = 0;
        Debug.LogFormat("Checking {0} children", transform.childCount);
        foreach (Transform child in transform)
        {
            Debug.LogFormat("Checking {0}, iteration {1}", child.name, i);
            if (child.gameObject.HasComponent<GravitationalObject>())
            {
                gravitationalObjects[i] = child.GetComponent<GravitationalObject>();
                Debug.LogFormat("{0} has component, setting in list", child.name);
            }
            else
            {
                Debug.LogFormat("{0} missing component, adding and setting in list", child.name);
                GameObject dottedLine = Instantiate(dottedLinePrefab);
                dottedLine.transform.SetParent(child);
                dottedLine.transform.localPosition = Vector3.zero;
                Color randomColor = Random.ColorHSV();
                dottedLine.GetComponent<ParticleSystem>().startColor = randomColor;
                // child.gameObject.GetComponent<MeshRenderer>().material.color = randomColor;
                child.localScale = Vector3.one * Random.Range(0.1f, 4f);

                child.gameObject.AddComponent<GravitationalObject>();
                GravitationalObject gravitationalObject = child.GetComponent<GravitationalObject>();
                gravitationalObject.mass = child.localScale.x;
                gravitationalObject.transform.position = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f));
                if (sun)
                {
                    Vector3 tangent = Vector3.Cross(sun.GetDistanceVector(gravitationalObject.transform.position), Vector3.up);
                    tangent.Normalize();
                    gravitationalObject.velocity = tangent * Random.Range(-10,10);
                    gravitationalObject.transform.localRotation = new Quaternion(Random.Range(0,1), Random.Range(0,1), Random.Range(0,1), Random.Range(0,1));
                    gravitationalObject.angularVelocity = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                }
                else
                {
                    gravitationalObject.velocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
                }
                gravitationalObjects[i] = gravitationalObject;
                
            }

            if (!child.gameObject.HasComponent<SphereGeometry>()){
                child.gameObject.AddComponent<SphereGeometry>();
                child.gameObject.GetComponent<MeshRenderer>().material = planetMaterial;
            }

            if (showTrails | !child.GetChild(0).gameObject.HasComponent<ParticleSystem>()) child.GetChild(0).gameObject.SetActive(true);
            else child.GetChild(0).gameObject.SetActive(false);
            i++;
        }
        GravitationalObject.gravitationalObjectList = gravitationalObjects;
    }

}
