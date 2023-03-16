using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controls different brushmodes 
/// Different properties of each brush mode are affected by speed of controller movement
/// brushMode is an int value:
/// 1 is trail mode
/// 2 is game object mode
/// 3 is particle mode
/// </summary>

public class VRCPSpeedBrushTrail : MonoBehaviour
{
    public GameObject trailPrefab = null;
    public GameObject shapePrefab = null;
    public GameObject particlePrefab = null;

    public float brushSensitivity = 0.01f;

    // 1 is trail mode
    // 2 is game object mode
    // 3 is particle mode
    private int brushMode;
    private Color color = Color.white;

    private AudioSource audioSource = null;
    private GameObject currentTrail = null;
    private TrailRenderer trailRenderer = null;
    private Vector3 previousPosition;
    private float speed;
    private Queue<float> previousSpeeds = new Queue<float>();
    private bool paint;
    private GameObject paintBrushMesh;

    // Start is called before the first frame update
    void Start()
    {

        audioSource = GetComponent<AudioSource>();

        paintBrushMesh = transform.GetChild(0).gameObject;
        paintBrushMesh.SetActive(false);

        brushMode = 1;//starts out in trail mode
        paint = false;
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (paint)
        {
            Vector3 vec = transform.position - previousPosition;
            float s = vec.magnitude / Time.deltaTime;
            previousSpeeds.Enqueue(s);

            if (previousSpeeds.Count > 20)
            {
                previousSpeeds.Dequeue();
            }

            speed = 0;
            foreach (float f in previousSpeeds)
            {
                speed += f;
            }

            speed /= previousSpeeds.Count;


            if (speed > brushSensitivity)
            {
                if(brushMode == 1)
                {
                    ApplyTrailSettings(mapToRange(speed, 0.0f, 5.0f, 0.001f, 0.05f, true));
                }
                else if(brushMode == 2)
                {
                    SpawnGameObject(mapToRange(speed, 0.0f, 5.0f, 0.01f, 0.7f, true));
                }
                else if(brushMode == 3)
                {
                    //don't spawn too many particle systems otherwise things get slow
                    //using probability we say if a random value between 0 - 1 is greater than another value spawn
                    //try the particlesDie prefab compared to the Particles prefab... things might start to slow down
                    //for you if you put millions of particle systems in when painting
                    if(Random.value > 0.5f)
                    {
                        SpawnParticles(mapToRange(speed, 0.0f, 5.0f, 0.01f, 0.7f, true));
                    }
                  
                }
   
                Debug.Log("Controller: " + speed + ", " + vec.x + " " + vec.y + " " + vec.z);
            }

            previousPosition = transform.position;
        }


    }


    private void ApplyTrailSettings(float width)
    {
        
        trailRenderer.widthMultiplier = width;
        trailRenderer.startColor = color;
        trailRenderer.endColor = color;
    }

    void SpawnGameObject(float scale)
    {
        GameObject sphere = Instantiate(shapePrefab, transform.position, transform.rotation);
        Vector3 scaleVec = new Vector3(scale, scale, scale);
        MeshRenderer mR = sphere.GetComponent<MeshRenderer>();
        mR.material.SetColor("_EmissionColor", color);
        mR.material.SetColor("_BaseColor", color);
        sphere.transform.localScale = scaleVec;
    }

    void SpawnParticles(float scale)
    {
        //advanced coding challenge
        //Can you affect attributes of the particle system you instantiate?
        //https://docs.unity3d.com/ScriptReference/Component-particleSystem.html
        //https://docs.unity3d.com/ScriptReference/ParticleSystem.html
        //Be careful as you might really start to slow down the frame rate. 
        Instantiate(particlePrefab, transform.position, transform.rotation);
    }

    void StartTrail()
    {
        if (!currentTrail)
        {
            currentTrail = Instantiate(trailPrefab, transform.position, transform.rotation, transform);
            trailRenderer = currentTrail.GetComponent<TrailRenderer>();
            ApplyTrailSettings(0.01f);
        }
    }

    void StopTrail()
    {
        if (currentTrail)
        {
            trailRenderer = null;
            currentTrail.transform.parent = null;
            currentTrail = null;
        }

    }

    public void SetColor(Color value)
    {
        color = value;
    }

    public void SetBrushMode(int mode)
    {
        brushMode = mode;
    }

    public void PaintOn()
    {
        paintBrushMesh.SetActive(true);
        if (brushMode == 1)
        {
            StartTrail();
        }
        else if (brushMode == 2)
        {

        }
        else if (brushMode == 3)
        {

        }
        previousPosition = transform.position;//update previous position before painting
        paint = true;
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void PaintOff()
    {
        paintBrushMesh.SetActive(false);
        if (brushMode == 1)
        {
            StopTrail();
        }
        else if (brushMode == 2)
        {

        }
        else if (brushMode == 3)
        {

        }
        paint = false;
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    //maps a value from a existing range to a new range
    //need to give value and min and max of the rane
    //and the target range min and max (outMin & outMax)
    //clamp bool will clamp it to the out range
    float mapToRange(float value, float inMin, float inMax, float outMin, float outMax, bool clamp)
    {
        float mapped = (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        if (clamp)
        {
            mapped = Mathf.Clamp(mapped, outMin, outMax);
        }
        return mapped;
    }



}
