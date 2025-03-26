using UnityEngine;

namespace Es.InkPainter.Sample
{
	[RequireComponent(typeof(Collider))]
	public class CollisionPainter : MonoBehaviour
	{
		[SerializeField]
		private Brush brush = null;

		[SerializeField]
		private int wait = 3;

		private int waitCount;

		public void FixedUpdate()
		{
			++waitCount;
		}

		public void OnCollisionStay(Collision collision)
		{
			if(waitCount < wait)
				return;
			waitCount = 0;

			foreach(var p in collision.contacts)
			{
				if(p.otherCollider.TryGetComponent<InkCanvas>(out var canvas))
					canvas.Paint(brush, p.point);
			}
		}
	}
}