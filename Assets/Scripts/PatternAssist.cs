using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PatternAssist : MonoBehaviour
{
	[Serializable]
	struct SpriteSettings
	{
		public Color color;
		public Sprite sprite;
	}

	struct RowObject
	{
		public GameObject gameObject;
		public TMP_Text textRef;
		public List<Image> imageComponentsRef;
	}

	[SerializeField] private GameObject assistTablet;
	[SerializeField] private GameObject assistRowRef;
	[SerializeField] private GameObject imageObjRef;
	[SerializeField] private List<SpriteSettings> _spriteSettingsList;
	[SerializeField] private List<RowObject> assistRows;

	public void ClearRows() {
		assistRows?.ForEach(obj => Destroy(obj.gameObject));
		assistRows = new List<RowObject>();
	}

	public void AddAssistRow(MusicCommand rhythmPattern) {
		if (assistRows == null) {
			assistRows = new List<RowObject>();
		}
		
		GameObject clone = Instantiate(assistRowRef, assistTablet.transform);
		clone.name = rhythmPattern.GetName();
		RowObject rowObject = new() {
						gameObject = clone,
						textRef = clone.transform.GetComponentInChildren<TMP_Text>(),
						imageComponentsRef = new List<Image>()
		};

		rowObject.textRef.text = rhythmPattern.GetName();
		
		// Get Grid Panel Object
		GameObject gridLayout = clone.transform.Find("Panel").gameObject;

		GameObject curImageObj = null;
		Image curImageRef;
		List<Image> imageList = new List<Image>();

		int index = 0;
		foreach (var drumID in rhythmPattern.GetDrumList()) {
			curImageObj = Instantiate(imageObjRef, gridLayout.transform);
			curImageRef = curImageObj.GetComponent<Image>();
			curImageRef.sprite = _spriteSettingsList[drumID].sprite;
			curImageRef.color = _spriteSettingsList[drumID].color;
			imageList.Add(curImageRef);
			index++;
		}

		Debug.Log("WHY");
		rowObject.imageComponentsRef = imageList;
		assistRows.Add(rowObject);
	}
	
	public void AnimateSymbol(int index) {
		foreach (var row in assistRows) {
			row.imageComponentsRef.ForEach(image => 
							image.color = image.color != Color.black ? Color.white : Color.black);
			Image image = row.imageComponentsRef[index];
			Color col = _spriteSettingsList[index].color;
		}
	}

	public void ClearColor() {
		foreach (var row in assistRows) {
			row.imageComponentsRef.ForEach(image => 
							image.color = image.color != Color.black ? Color.white : Color.black);
			
		}
	}
}