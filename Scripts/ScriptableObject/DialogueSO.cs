using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "ScriptableObject/Dialogue")]
public class DialogueSO : ScriptableObject
{
    [field : SerializeField] public List<Dialogue> dialogue;
}