using UnityEngine;

public class HealthManager : PersistentManager<HealthManager>
{
    public float VieJoueur = 100f;
    public float damageInterval = 1f;
    public float baseDamage = 20f;
    public float reducedDamage = 5f;

    private bool isTakingDamage = false;

    public override void Initialize()
    {
        VieJoueur = 100f;
        UpdateUI();
    }

    void Update()
    {
        // Optional: Handle death
        if (VieJoueur <= 0)
        {
            VieJoueur = 0;
            CancelInvoke("ApplyDamage");
            Debug.Log("Player is dead.");
            // Trigger death sequence, restart, etc.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !isTakingDamage)
        {
            isTakingDamage = true;
            InvokeRepeating("ApplyDamage", 0f, damageInterval);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            isTakingDamage = false;
            CancelInvoke("ApplyDamage");
        }
    }

    private void ApplyDamage()
    {
        float damage = baseDamage;

        GameObject maskIcon = GameObject.FindGameObjectWithTag("MaskIcon");
        if (maskIcon != null && maskIcon.activeInHierarchy)
        {
            damage = reducedDamage;
        }

        VieJoueur -= damage;
        VieJoueur = Mathf.Clamp(VieJoueur, 0, 100);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (GameUIManager.Instance != null && GameUIManager.Instance.healthBar != null)
        {
            GameUIManager.Instance.healthBar.value = VieJoueur;
        }
    }
}
