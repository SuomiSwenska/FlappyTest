using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [SerializeField] private Sprite startSkinSprite;
    [SerializeField] private RuntimeAnimatorController animatorController;

    public Sprite StartSkinSprite { get => startSkinSprite; }
    public RuntimeAnimatorController AnimatorController { get => animatorController; }
}
