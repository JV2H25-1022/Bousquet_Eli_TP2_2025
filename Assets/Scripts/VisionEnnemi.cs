using System.Collections;
using UnityEngine;

public class VisionEnnemi : MonoBehaviour
{
    [SerializeField] private SuivreJoueur _suivreJoueur;
    [SerializeField] private float tempsAvantSuivre = 1.5f;
    [SerializeField] private float tempsAvantArret = 2.0f;
    [SerializeField] private LayerMask obstacleMask;

    private Coroutine _coroutineSuivre;
    private Coroutine _coroutineArret;
    private bool _joueurDansZone = false;
    private bool _zoneActive = false;

    private GameObject _joueur;

    private void Start()
    {
        _joueur = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // press T to test
        {
            _suivreJoueur.CommencerSuivre();
        }

        if (Input.GetKeyDown(KeyCode.L)) // press L to test
        {
            Debug.Log("Joueur dans zone:" + _joueurDansZone);
            Debug.Log("Zone active:" + _zoneActive);
            Debug.Log("Peut voir joueur:" + PeutVoirJoueur());
        }
        
        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _joueurDansZone = true;

            Debug.Log("Joueur entre");

            if (_coroutineArret != null)
            {
                StopCoroutine(_coroutineArret);
                _coroutineArret = null;
                Debug.Log("stop delai1");                

            }

            if (_coroutineSuivre == null && !_zoneActive && PeutVoirJoueur())
            {
                _coroutineSuivre = StartCoroutine(DelaiAvantSuivre());
                Debug.Log("delai suivre");

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _joueurDansZone = false;

            
            Debug.Log("Joueur sort");

            if (_coroutineSuivre != null)
            {
                StopCoroutine(_coroutineSuivre);
                _coroutineSuivre = null;
                Debug.Log("stop delai2");                  
            }

            if (_coroutineArret == null && _zoneActive)
            {
                _coroutineArret = StartCoroutine(DelaiAvantArret());
                Debug.Log("delai arret");                
            }  
        }
    }

    private IEnumerator DelaiAvantSuivre()
    {
        float timer = 0f;
        while (timer < tempsAvantSuivre)
        {
            if (!_joueurDansZone || !PeutVoirJoueur())
            {
                _coroutineSuivre = null;
                Debug.Log("stop delai3");                  
                yield break; 
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (_joueurDansZone && !_zoneActive && PeutVoirJoueur())
        {
            _suivreJoueur.NotifierEntreeZone();
            _zoneActive = true;
            
            Debug.Log("VOIT JOUEUR");

        }

        _coroutineSuivre = null;
    }

    private IEnumerator DelaiAvantArret()
    {
        yield return new WaitForSeconds(tempsAvantArret);
        if (!_joueurDansZone && _zoneActive)
        {
            _suivreJoueur.NotifierSortieZone();
            _zoneActive = false;

        }

        _coroutineArret = null;
    }
    private bool PeutVoirJoueur()
    {
        if (_joueur == null) return false;

        Vector3 origin = transform.position;


        Vector3 target = _joueur.transform.position + Vector3.up * 0.9f;

        Vector3 direction = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        Debug.DrawRay(origin, direction * distance, Color.red, 1f);
        Debug.Log("1");

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ~obstacleMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("2: Hit = " + hit.collider.name);
            return hit.collider.CompareTag("Player");
        }

        return false;
    }



}

