using UnityEngine;

namespace Es.InkPainter.Sample
{
	[RequireComponent(typeof(Collider))]
	public class CollisionPainter : MonoBehaviour
	{
		[SerializeField]
		private Brush brush = null;

		[SerializeField]
		private int wait = 10;
		
		private InkCanvas _inkCanvas;
		private int _waitCount;

		public void Awake()
		{
			_inkCanvas = FindFirstObjectByType<InkCanvas>();
		}

		public void FixedUpdate()
		{
			++_waitCount;
		}

		public void OnCollisionStay(Collision collision)
		{
			if(_waitCount < wait)
				return;
			_waitCount = 0;

			foreach(var p in collision.contacts)
			{
				if(_inkCanvas != null)
					_inkCanvas.Paint(brush, p.point);
			}
		}
	}
}