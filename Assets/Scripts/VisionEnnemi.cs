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
}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _joueurDansZone = true;

            if (_coroutineArret != null)
            {
                StopCoroutine(_coroutineArret);
                _coroutineArret = null;
            }

            if (_coroutineSuivre == null && !_zoneActive && PeutVoirJoueur())
            {
                _coroutineSuivre = StartCoroutine(DelaiAvantSuivre());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _joueurDansZone = false;

            if (_coroutineSuivre != null)
            {
                StopCoroutine(_coroutineSuivre);
                _coroutineSuivre = null;
            }

            if (_coroutineArret == null && _zoneActive)
            {
                _coroutineArret = StartCoroutine(DelaiAvantArret());
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

        Debug.Log("1");

        Vector3 direction = (_joueur.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, _joueur.transform.position);

        Debug.DrawRay(transform.position, direction * distance, Color.red, 1f);


        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, ~obstacleMask))
        {
            Debug.Log("2: Hit = " + hit.collider.name);
            return hit.collider.CompareTag("Player");
        }

        return false;
}

}

