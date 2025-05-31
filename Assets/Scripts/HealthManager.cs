using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{

    private GameObject enemy;
    private GameObject player;

    void Start()
    {

        enemy = GameObject.FindGameObjectWithTag("Ennemi");
        player = GameObject.FindGameObjectWithTag("Joueur");


    }

    void Update()
    {
        
    }
}
