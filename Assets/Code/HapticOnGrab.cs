using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticOnGrab : MonoBehaviour
{
    [Range(0f, 1f)] public float kekuatanGetar = 0.4f;
    public float durasiGetar = 0.1f;

    public void PulsaSaatDigenggam(SelectEnterEventArgs args)
    {
        var interactorComponent = args.interactorObject as Component;
        if (interactorComponent == null) return;

        XRBaseController controller = interactorComponent.GetComponentInParent<XRBaseController>();
        if (controller != null)
        {
            controller.SendHapticImpulse(kekuatanGetar, durasiGetar);
        }
    }
}