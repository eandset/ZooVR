using System.Collections;
using TMPro;
using UnityEngine;

public abstract class InputTextAnimationConfig : ScriptableObject
{
    public abstract IEnumerator Animate(TMP_Text text, string message);
}