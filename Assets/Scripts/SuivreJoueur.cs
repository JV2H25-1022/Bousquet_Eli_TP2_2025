using UnityEngine;
using UnityEngine.AI;

public class SuivreJoueur : MonoBehaviour
{
    private GameObject _joueur;
    private NavMeshAgent _agent;

    [SerializeField] private int _zonesActives = 0;

    private const string TAG_PLAYER = "Player";

    void Start()
    {
        _joueur = GameObject.FindGameObjectWithTag(TAG_PLAYER);
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
        InvokeRepeating(nameof(DeplacerEnnemi), 0f, 0.1f);
    }

    public void ArreterSuivre()
    {
        CancelInvoke(nameof(DeplacerEnnemi));
    }

    void DeplacerEnnemi()
    {
        if (_joueur != null)
            _agent.SetDestination(_joueur.transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TAG_PLAYER))
        {
            HealthManager.Instance.DamageHandler(GetComponent<Collider>(), true);
            Debug.Log("Degats commencent");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TAG_PLAYER))
        {
            HealthManager.Instance.DamageHandler(GetComponent<Collider>(), false);
            Debug.Log("Degats arretent");
        }
    }
}
