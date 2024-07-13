using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{

    public float speed = 0.5f;
    Vector3 direction;
    public float timer = 0.0f;
    public float wait = 0.0f;
    
    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wait)
        {
            direction = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
            wait = Random.Range(0, 10);
            timer = 0;
        }

        transform.position += Time.deltaTime * direction;



    }
}
