using UnityEngine;

namespace Multiplayer
{
	public class MoveTest : MonoBehaviour
	{
		private float startX;
		private void Start()
		{
			startX = transform.position.x;
			GetComponent<SpriteRenderer>().color = new Color(UnityEngine.Random.value, UnityEngine.Random.value,
				UnityEngine.Random.value, 1);
		}

		private void Update()
		{
			transform.position = new Vector3(startX + Mathf.PingPong(Time.time+ UnityEngine.Random.Range(0,0.05f), 8f), transform.position.y);
		}
	}
}