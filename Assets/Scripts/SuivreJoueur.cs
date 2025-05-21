using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SuivreJoueur : MonoBehaviour
{
    private GameObject _joueur;
    private NavMeshAgent _agent;

    [SerializeField] private int _zonesActives = 0;

    void Start()
    {
        _joueur = GameObject.FindGameObjectWithTag("Player");
        _agent = GetComponent<NavMeshAgent>();
    }

    public void NotifierEntreeZone()
    {
        _zonesActives++;
        if (_zonesActives == 1) 
        {
            CommencerSuivre();
        }
    }

    public void NotifierSortieZone()
    {
        _zonesActives--;
        if (_zonesActives <= 0)
        {
            _zonesActives = 0;
            ArreterSuivre();
        }
    }
    public void CommencerSuivre()
    {
        InvokeRepeating("DeplacerEnnemi", 0f, 0.1f);
    }

    public void ArreterSuivre()
    {
        CancelInvoke("DeplacerEnnemi");
    }

    void DeplacerEnnemi()
    {

        _agent.SetDestination(_joueur.transform.position);
 
    }
}
