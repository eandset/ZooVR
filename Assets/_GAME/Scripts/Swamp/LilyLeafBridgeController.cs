using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LilyLeafBridgeController : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable interactable;
    [SerializeField] private PlayerDetector detector;
    [SerializeField] private LilyLeafBridgeView view;
    [SerializeField] private XRSocketInteractor socket;

    private InteractableSocketDropDetector _dropDetector;
    private PlayerPresenceGrabBlocker _presenceGrabBlocker;
    private SocketGrabLocker _socketGrabLocker;
    private ReturnBlocker _returnBlocker;

    private void Awake()
    {
        var failHandler = GetComponent<IFailHandler>();

        _dropDetector = new InteractableSocketDropDetector(interactable, detector);
        _presenceGrabBlocker = new PlayerPresenceGrabBlocker(detector, interactable, failHandler);
        _socketGrabLocker = new SocketGrabLocker(interactable, socket);
        _returnBlocker = new ReturnBlocker(interactable, socket, view);
    }

    private void OnEnable()
    {
        _dropDetector.Initialize();
        _presenceGrabBlocker.Initialize();
        _socketGrabLocker.Initialize();
        _returnBlocker.Initialize();

        _dropDetector.OnDroppedOutside += view.ReturnPosition;
        _dropDetector.OnDroppedInside += _socketGrabLocker.EnableSocket;
    }

    private void OnDisable()
    {
        _dropDetector.Dispose();
        _presenceGrabBlocker.Dispose();
        _socketGrabLocker.Dispose();
        _returnBlocker.Dispose();

        _dropDetector.OnDroppedOutside -= view.ReturnPosition;
        _dropDetector.OnDroppedInside -= _socketGrabLocker.EnableSocket;
    }
}