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
	public class DamagePopup : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI text;
		[SerializeField] private float lifeTime = 0.5f;
		[SerializeField] private float minDistance = 2f;

		[SerializeField] private float maxDistance = 2f;
		private Vector3 initialPosition;
		private Vector3 targetPosition;
		private float timer;
		private float fraction;

		private void Start()
		{
			var trans = transform;
			trans.position += new Vector3(0, 1.75f, 0);
			trans.LookAt(2 * transform.position - Camera.main.transform.position);
			var direction = UnityEngine.Random.rotation.eulerAngles.x;
			initialPosition = trans.position;
			var dist = Random.Range(minDistance, maxDistance);
			targetPosition = initialPosition + (Quaternion.Euler(0, 0, direction) * new Vector3(dist, dist, 0f));
			fraction = lifeTime / 2f;
			transform.localScale = Vector3.zero;
		}

		public void SetDamageText(int damage) => text.text = damage.ToString();

		private void Update()
		{
			timer += Time.deltaTime;
			if (timer > lifeTime) Destroy(gameObject);
			else if (timer > fraction)
				text.color = Color.Lerp(text.color, Color.clear, (timer - fraction) / (lifeTime - fraction));
			transform.position = Vector3.Lerp(initialPosition, targetPosition, Mathf.Sin(timer / lifeTime));
			transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.Sin(timer / lifeTime));
		}
	}
}