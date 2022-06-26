//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using TMPro;
using UnityEngine;

namespace UI
{
	/// <summary>
	///DamagePopup full description
	/// </summary>
	public class XPPopup : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI text;
		[SerializeField] private float lifeTime = 0.5f;
		
		private Vector3 initialPosition;
		private Vector3 targetPosition;
		private float timer;

		private void Start()
		{
			var trans = transform;
			trans.position += new Vector3(0, 1.5f, 0);
			trans.LookAt(2 * transform.position - Camera.main.transform.position);
			initialPosition = trans.position;
			targetPosition = initialPosition + new Vector3(0, 1.5f, 0f);
			transform.localScale = Vector3.zero;
		}

		public void SetXpText(int damage) => text.text = "XP + " + damage;

		private void Update()
		{
			timer += Time.deltaTime;
			if (timer > lifeTime) Destroy(gameObject);
			transform.position = Vector3.Lerp(initialPosition, targetPosition, Mathf.Sin(timer / lifeTime));
			transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.Sin(timer / lifeTime));
		}
	}
}