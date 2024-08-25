using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public List<int> SpeakerType { get; set; }
    [field: SerializeField] public List<int> SpeakerId { get; set; }
    [field: SerializeField] public List<string> Texts { get; set; }
    [field: SerializeField] public int DialogueEvent { get; set; }
    [field: SerializeField] public List<int> SFXIndex { get; set; }
    [field: SerializeField] public int NextId { get; set; }
    [field: SerializeField] public List<int> IconIndex { get; set; }
}
