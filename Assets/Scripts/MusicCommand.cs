using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "command", menuName = "Music Command", order = 1)]
[Serializable]
public class MusicCommand : ScriptableObject
{
	private static readonly double BuiltInMargin = 0.25F;

	[SerializeField] private string name;
	[SerializeField] private List<Beat> Beats;
	[SerializeField] public UnityEvent _commandEvent;
	
	public bool CheckPattern(Beat[] testBeats) {
		bool correct = true;
		
		// Jump out if too little beats
		if (testBeats.Length < GetCount())
			return false;
		
		// We want to check only the last n beats in the provided list
		var start = testBeats.Length - Beats.Count;
		
		// Check first to get offset
		double offset = testBeats[start].Timing - Beats[0].Timing;

		string a = "";
		foreach (Beat testBeat in testBeats) {
			a += ",";
			a += $"({testBeat.DrumID}|{testBeat.Timing - offset})";
		}
		
		Debug.Log(a);
		Debug.Log(this.ToString());

		for (int i = 0; i < Beats.Count; i++) {
			var correctedTime = testBeats[i+start].Timing - offset;
			correct &= correct &&
			           (Beats[i].Timing - BuiltInMargin) <= correctedTime &&
			           correctedTime <= (Beats[i].Timing + BuiltInMargin) &&
			           (testBeats[i+start].DrumID == Beats[i].DrumID);
		}
		
		return correct;
	}
	
	public string GetName() {
		return name;
	}

	public List<int> GetDrumList() {
		List<int> drumList = new List<int>();
		foreach (Beat beat in Beats) {
			drumList.Add(beat.DrumID);
		}
		return drumList;
	}

	public int GetCount() {
		return Beats.Count;
	}

	public void PerformCommand() {
		Debug.Log("Performed "+GetName());
		_commandEvent.Invoke();
	}

	public override string ToString() {
		return String.Join(",", Beats.Select(x => x.ToString()).ToArray());
	}
}

[Serializable]
public struct Beat
{
	public int DrumID;
	public double Timing;

	public Beat(int x, double a) {
		DrumID = x;
		Timing = a;
	}

	public override string ToString() {
		return $"({DrumID}|{Timing})";
	}
}