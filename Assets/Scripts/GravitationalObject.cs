using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalObject : MonoBehaviour
{
    public static GravitationalObject[] gravitationalObjectList = null;
    public float mass = 1f;
    public Vector3 acceleration = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Vector3 angularVelocity = Vector3.zero;
    private Vector3 gravitationalForce = Vector3.zero;
    static float gravitationalConstant = 6.673f;
    static float gravitationMultiplier = 1;
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;
    private int nVertices;
    private float radius;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        nVertices = mesh.vertices.Length;
        colors = new Color[nVertices];
        radius = mesh.vertices[0].magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        SetGravitationalForce();
        SetAcceleration();
        SetVelocity();
        SetForceColor();
        // SetRotation();
        Debug.LogFormat("{0} | Gravitational Force: {1}", this.name, gravitationalForce);
    }

    void LateUpdate()
    {
        SetPosition();
    }

    private void SetAcceleration()
    {
        acceleration = gravitationalForce / mass;
    }

    private void SetVelocity()
    {
        velocity += acceleration * Time.deltaTime;
    }

    private void SetPosition()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private void SetForceColor()
    {
        vertices = mesh.vertices;
        Debug.LogFormat("{0}: Vert 0: {1}", transform.name, vertices[0]);
        for (int i=0; i<nVertices; i++)
        {
            Vector3 diff = gravitationalForce.normalized - vertices[i].normalized;
            colors[i] = Color.LerpUnclamped(Color.red, Color.blue, diff.magnitude/2);
            
        }
        mesh.colors = colors;
    }

    private void SetRotation()
    {
        Vector3 rot = transform.localEulerAngles;
        rot += angularVelocity * Time.deltaTime;

        if (rot.x >= 90) rot.x -= 180;
        if (rot.y >= 90) rot.y -= 180;
        if (rot.z >= 90) rot.z -= 180;
        
        transform.localRotation = Quaternion.Euler(rot);
    }

    //TODO remove redundant force calculations by cumulating force in series -- possibly move to compute shader and just let the GPU figure it out
    //http://www.scholarpedia.org/article/N-body_simulations_(gravitational)#Direct_methods (1) F⃗ i=−∑j≠iGmimj(r⃗ i−r⃗ j) / |ri→−rj→|3−∇⃗ ⋅ϕext(r⃗ i) -- omit external potential because yeet
    private void SetGravitationalForce()
    {
        Vector3 distanceVector = Vector3.zero;
        gravitationalForce = Vector3.zero;
        foreach (GravitationalObject gravitationalObject in gravitationalObjectList)
        { 
            if (gravitationalObject != this)
            {
                distanceVector = gravitationalObject.GetDistanceVector(transform.position);
                gravitationalForce -= mass * gravitationalObject.GetMass() * distanceVector / CubeValue(distanceVector.magnitude);
            }
        }
        gravitationalForce *= gravitationalConstant * gravitationMultiplier;
    }
    
    private float CubeValue(float value)
    {
        return value * value * value;
    }

    public float GetMass()
    {
        return mass;
    }

    public Vector3 GetDistanceVector(Vector3 objectPosition)
    {
        return objectPosition - transform.position;
    } 
}
