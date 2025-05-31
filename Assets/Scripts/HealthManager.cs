using UnityEngine;

public class HealthManager : PersistentManager<HealthManager>
{
    public float VieJoueur = 100f;
    public float damageInterval = 1f;
    public float baseDamage = 20f;
    public float reducedDamage = 5f;

    private bool isTakingDamage = false;
    private Collider currentEnemyZone = null;


    private const string TAG_ENEMY = "Enemy";

    public override void Initialize()
    {
        VieJoueur = 100f;

        UpdateUI();
        UpdateData();
    }

    void Update()
    {
        if (VieJoueur <= 0f)
        {
            VieJoueur = 0f;
            CancelInvoke(nameof(ApplyDamage));
            Debug.Log("Player is dead.");
        }
    }

    public void DamageHandler(Collider enemyZone, bool entered)
    {
        if (!enemyZone.CompareTag(TAG_ENEMY)) return;

        if (entered)
        {
            if (!isTakingDamage)
            {
                isTakingDamage = true;
                currentEnemyZone = enemyZone;
                InvokeRepeating(nameof(ApplyDamage), 0f, damageInterval);
            }
        }
        else
        {
            if (currentEnemyZone == enemyZone)
            {
                isTakingDamage = false;
                CancelInvoke(nameof(ApplyDamage));
                currentEnemyZone = null;
            }
        }
    }

    private void ApplyDamage()
    {
        float damage = GameUIManager.Instance.HasMask ? reducedDamage : baseDamage;
        TakeDamage(damage);
    }


    public void TakeDamage(float amount)
    {
        VieJoueur -= amount;
        VieJoueur = Mathf.Clamp(VieJoueur, 0f, 100f);
        UpdateUI();
        UpdateData();
    }

    private void UpdateUI()
    {
        GameUIManager.Instance?.UpdateHealthDisplay(VieJoueur);
    }

    private void UpdateData()
    {
        GameDataManager.Instance?.SetHealth(VieJoueur);
    }
}
