using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "Status Effect Data", menuName = "Scriptable Objects/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    [SerializeField]
    private string statusEffectName;
    public string StatusEffectName { get { return statusEffectName; }}

    [SerializeField]
    private StatusTypes statusType;
    public StatusTypes StatusType { get { return statusType; }}

    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite { get { return sprite; }}

    [SerializeField]
    private Material material;
    public Material Material { get { return material; }}

    public enum StatusTypes
    {
        Buff,
        Debuff,
        Passive,
    }
}
