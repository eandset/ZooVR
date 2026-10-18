using UnityEngine;

public class FailsContainer : MonoBehaviour, IFailHandler
{
    [SerializeReference, SubclassSelector] private IFailHandler[] handlers;

    private void Awake()
    {
        foreach (var handler in handlers)
            if (handler is IFailInitialize initialize)
                initialize.Initialize();
    }

    public void HandleFail()
    {
        foreach (var handler in handlers)
            handler.HandleFail();
    }
}