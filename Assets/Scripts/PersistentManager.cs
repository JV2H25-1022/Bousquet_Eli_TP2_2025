using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base class for singleton-like persistent managers that survive scene loads.
/// </summary>
public abstract class PersistentManager<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    protected bool isPersisted = false;


    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;

        // Ensure this object is root so DontDestroyOnLoad works properly
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        isPersisted = true;
        OnSingletonAwake();


    }




    /// <summary>
    /// Optional method for custom logic during Awake in derived classes.
    /// </summary>
    protected virtual void OnSingletonAwake() { }

    /// <summary>
    /// Optional manual initialization method.
    /// </summary>
    public virtual void Initialize() { }

}
