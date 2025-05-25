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
            Debug.Log("ZONE ACTIVE");
            if (_zonesActives == 1)
            {
                CommencerSuivre();
                Debug.Log("commencer suivre");
            }
        }

        public void NotifierSortieZone()
        {
            _zonesActives--;

            if (_zonesActives <= 0)
            {
                _zonesActives = 0;
                ArreterSuivre();
                Debug.Log("arreter suivre");

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
            Debug.Log("Trying to move to player");
            _agent.SetDestination(_joueur.transform.position);
        }

    }
