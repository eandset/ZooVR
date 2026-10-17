using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LilyLeafBridgeController : MonoBehaviour
{
    [SerializeField] private PlayerDetector detector;
    [SerializeField] private LilyLeafBridgeView view;
    [SerializeField] private XRSocketInteractor socket;
    
    private SocketGrabLocker _grabLocker;
    private PlayerPresenceGrabBlocker _grabBlocker;
    private ReturnBlocker _returnBlocker;
    
    private void Awake()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        var failHandler = GetComponent<IFailHandler>();
        
        _grabLocker = new SocketGrabLocker(interactable, socket);
        _grabBlocker = new PlayerPresenceGrabBlocker(detector, interactable, failHandler);
        _returnBlocker = new ReturnBlocker(interactable, socket, view);
    }

    private void OnEnable()
    {
        _grabLocker.Initialize();
        _grabBlocker.Initialize();
        _returnBlocker.Initialize();
    }

    private void OnDisable()
    {
        _grabLocker.Dispose();
        _grabBlocker.Dispose();
        _returnBlocker.Dispose();
    }
}