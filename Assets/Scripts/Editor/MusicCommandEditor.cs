using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicCommand))]
public class MusicCommandEditor : Editor
{
	private MusicCommand command;

	private void OnEnable() {
		command = target as MusicCommand;
	}

	private void OnInspectorGUI() {
		// Draw the baseline script 
		base.OnInspectorGUI();
		
		
	}
}