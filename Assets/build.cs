using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class build : MonoBehaviour
{

    [SerializeField]
    public int sizex;
    [SerializeField]
    public int sizez;

    [SerializeField]
    private GameObject building1;
    [SerializeField]
    private GameObject building2;
    [SerializeField]
    private GameObject building3;
    [SerializeField]
    private GameObject building4;
    [SerializeField]
    private GameObject building5;
    [SerializeField]
    private GameObject building6;
    [SerializeField]
    private GameObject building7;

    [SerializeField]
    private GameObject tower;


    [SerializeField]
    private GameObject person;

    [SerializeField]
    private GameObject tree;


    // Start is called before the first frame update
    void Start()
    {
        sizez = sizez + sizez % 3;

        int[,] field = new int[sizex, sizez];

        for (var j = 0; j < sizez; j++)
        {

            if (j % 3 == 0) {  // prazne ulice

                for (var i = 0; i < sizex; i++)
                {
                    field[i, j] = -1; // dodaj ljudi
                    if (0.2 > Random.Range(0.0f, 1.0f))
                    {
                        field[i, j] = 200;
                    }
                }


                continue;
            }

            for (var i = 0; i < sizex; i++)
            {

                if (i % 6 == 0) {
                    continue;
                }

                field[i, j] = Random.Range(1, 8);

                if (0.01 > Random.Range(0.0f, 1.0f)) {
                    field[i, j] = 100;
                }
            }
        }


        field[sizex / 2, sizez / 2] = 0;
        field[sizex / 2 + 1, sizez / 2] = 0;


        for (var i = 0; i < sizex; i++)
        {
            for (var j = 0; j < sizez; j++) {

                int rand = field[i, j];
                Quaternion q = Quaternion.identity;

                if (j % 3 == 1) {
                    q *= Quaternion.Euler(0, 180f, 0);
                }
               
                Vector3 position = new Vector3(i * 4.5f - sizex * 4.5f / 2, 0, j * 4.5f - sizez * 4.5f / 2);

                if (rand == -1) {
                    if (0.1 > Random.Range(0.0f, 1.0f))
                    {
                        q = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
                        Instantiate(person, position + new Vector3(Random.Range(0.0f, 2.0f), 0, Random.Range(-2.0f, 2.0f)), q); // instanciraj èloveka
                    }
                }

                Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));

                if (rand == 1) {
                    Instantiate(building1, position + offset, q);
                }

                if (rand == 2) {
                    Instantiate(building2, position + offset, q);
                }

                if (rand == 3)
                {
                    Instantiate(building3, position + offset, q);
                }

                if (rand == 4)
                {
                    Instantiate(building4, position + offset, q);
                }

                if (rand == 5)
                {
                    Instantiate(building5, position + offset, q);
                }

                if (rand == 6)
                {
                    Instantiate(building6, position + offset, q);
                }

                if (rand == 7)
                {
                    Instantiate(building7, position + offset, q);
                }

                if (rand == 100) {
                    Instantiate(tower, position, q);
                }

                if (rand == 200)
                {
                    Instantiate(tree, position + offset + new Vector3(2.0f, 0, 2.0f), q);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
       
    }
}