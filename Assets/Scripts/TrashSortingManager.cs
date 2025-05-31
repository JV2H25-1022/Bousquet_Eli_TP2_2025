using System.Collections.Generic;
using UnityEngine;

public class TrashSortingManager : MonoBehaviour
{
    [System.Serializable]
    public class BinRule
    {
        public Collider binCollider;
        public List<string> acceptedTags;  // Accept multiple tags
    }

    public List<BinRule> binRules = new List<BinRule>();

    public bool hasScored = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only check tags you're interested in
        if (!(other.CompareTag("poubelle")
           || other.CompareTag("plastique")
           || other.CompareTag("papier")
           || other.CompareTag("compost")))
        {
            return;
        }

        foreach (BinRule rule in binRules)
        {
            if (rule.binCollider.bounds.Intersects(other.bounds))
            {
                string enteredTag = other.tag;

                if (rule.acceptedTags.Contains(enteredTag))
                {
                    ScoreManager.Instance.AddPoint();
                }
                else
                {
                    ScoreManager.Instance.SubtractPoint();
                }

                hasScored = true;
                Destroy(other.gameObject); // or disable, up to you
                break;
            }
        }
    }
}


