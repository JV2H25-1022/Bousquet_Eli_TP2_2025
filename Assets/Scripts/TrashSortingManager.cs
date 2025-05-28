using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSortingManager : MonoBehaviour
{
    [System.Serializable]
    public class BinRule
    {
        public Collider binCollider;
        public string acceptedTag;
    }

    public List<BinRule> binRules = new List<BinRule>();

    public bool hasScored = false;

    private void OnTriggerEnter(Collider other)
    {
        // Loop through all bin rules to check if the entered collider matches
        foreach (BinRule rule in binRules)
        {
            if (!(other.CompareTag("poubelle")
            || other.CompareTag("recyclage")
            || other.CompareTag("compost")))
            {
                return;
            }

            // Check if the object entered one of the defined bins
            if (rule.binCollider.bounds.Intersects(other.bounds))
            {
                string enteredTag = other.tag;

                if (enteredTag == rule.acceptedTag)
                {
                    ScoreManager.Instance.AddPoint();
                }
                else
                {
                    ScoreManager.Instance.SubtractPoint();
                }

                hasScored = true;

                // Optionally destroy or disable the trash after scoring
                Destroy(other.gameObject);

                break; // Only process one rule
            }
        }
    }
}

