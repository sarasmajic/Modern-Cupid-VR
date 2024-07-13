using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Person : MonoBehaviour
{
   
    private int HP = 0; //te se ne spreminja, zato ni SerializeFielda
    [SerializeField]
    public Slider HealthBar;
    
    [SerializeField]
    Animator A;

    [SerializeField]
    private GameObject hearts;

    [SerializeField]
    TextMeshProUGUI Counter_TMP;


 

    // Start is called before the first frame update
    void Start()
    {
        
        
   }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDmg(int DmgAmount)
    {
        
        HP += DmgAmount;
        HealthBar.value = HP; //da se pokaže črta v healthbaru
        if (HP >= 100)
        {
            A.SetBool("IsShot", true);
            hearts.SetActive(true);

            GameObject counter = GameObject.Find("Counter_TMP");
            TextMeshProUGUI t = counter.GetComponent<TextMeshProUGUI>();

            int kills = int.Parse(t.text) + 1;

            t.text = kills.ToString();
            


        }
    }

    private void OnCollisionEnter(Collision other)
    {
        print("Hit");
        if (other.transform.CompareTag("Arrow"))
        {
            TakeDmg(50);
        }
    }
}
