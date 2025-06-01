using UnityEngine;

public class ObjetResponsable : MonoBehaviour, IInteractable, IHasViewSource
{

    [Header("Prompt Settings")]
    [SerializeField] private GameObject floatingPromptPrefab;
    [SerializeField] private Vector3 promptOffset = Vector3.zero;

    private GameObject floatingPromptInstance;
    private Transform viewSource;

    private bool hasBeenUsed = false;

    [SerializeField] private AudioClip usedSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void Start()
    {
        if (floatingPromptPrefab != null)
        {
            floatingPromptInstance = Instantiate(floatingPromptPrefab);
            floatingPromptInstance.SetActive(false);

            floatingPromptInstance.transform.position = transform.position + Vector3.up * promptOffset.y;
        }
    }

    private void Update()
    {
        if (floatingPromptInstance && floatingPromptInstance.activeSelf && viewSource != null)
        {
            Vector3 toPlayer = (viewSource.position - transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, toPlayer).normalized;

            Vector3 offset =
                toPlayer * promptOffset.z +
                Vector3.up * promptOffset.y +
                right * promptOffset.x;

            floatingPromptInstance.transform.position = transform.position + offset;
            floatingPromptInstance.transform.rotation = Quaternion.LookRotation(toPlayer * -1f);
        }
    }

    public void SetViewSource(Transform playerViewSource)
    {
        viewSource = playerViewSource;
    }

    public void ShowPrompt(bool show)
    {
        if (floatingPromptInstance != null)
        {
            floatingPromptInstance.SetActive(show);
        }
    }
    public void Interact()
    {
        if (hasBeenUsed) return;
        hasBeenUsed = true;

        if (CompareTag("ObjetResponsable"))
        {
            Light light = GetComponent<Light>();
            if (light != null) light.enabled = false;

            ScoreManager.Instance.AddPoint(1000/3);

            if (audioSource != null && usedSound != null)
            {
                audioSource.PlayOneShot(usedSound);
            }
        }
    }

        private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(promptOffset), 0.1f);
    }
}
