using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using DG.Tweening;
using System;
using UnityEditor;

namespace AntoineFoucault.Utilities
{
    public static class MathExtentions
    {
        /// <summary>
        /// Normalize an angle between -180 and 180 degrees
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static float NormalizeAngle(this float a)
        {
            return a - 180f * Mathf.Floor((a + 180f) / 180f);
        }

        /// <summary>
        /// Correctly clamps an angle between a minimum and a maximum value
        /// </summary>
        /// <param name="current"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float ClampAngle(float current, float min, float max)
        {
            float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
            float hdtAngle = dtAngle * 0.5f;
            float midAngle = min + hdtAngle;

            float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
            if (offset > 0)
                current = Mathf.MoveTowardsAngle(current, midAngle, offset);
            return current;
        }

        /// <summary>
        /// Snaps a Vector2 to the closest vector between a maximum number of directions
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="maxDirections"></param>
        /// <returns></returns>
        public static Vector2 SnapVector(this Vector2 vector, int maxDirections = 4)
        {
            float minAngle = 2 * Mathf.PI / maxDirections;
            float angle = Mathf.Atan2(vector.y, vector.x);
            float nearestMultiple = (float)Math.Round(angle / minAngle, MidpointRounding.AwayFromZero) * minAngle;
            vector.x = Mathf.Cos(nearestMultiple);
            vector.y = Mathf.Sin(nearestMultiple);
            return vector;
        }

        /// <summary>
        /// Returns the modulo of a positive or a negative integer
        /// </summary>
        /// <param name="x"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int Modulo(this int x, int m)
        {
            return (x % m + m) % m;
        }

        public static float SqrtDistance(Vector3 a, Vector3 b)
        {
            float num = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return (float)(num * num + num2 * num2 + num3 * num3);
        }
    }

    public static class VectorExtensions
    {
        #region Set One Param

        #region Vector3
        public static void SetX(this ref Vector3 v, float x)
        {
            v = new Vector3(x, v.y, v.z);
        }
        public static void SetY(this ref Vector3 v, float y)
        {
            v = new Vector3(v.x, y, v.z);
        }
        public static void SetZ(this ref Vector3 v, float z)
        {
            v = new Vector3(v.x, v.y, z);
        }
        #endregion

        #region Vector2
        public static void SetX(this ref Vector2 v, float x)
        {
            v = new Vector2(x, v.y);
        }
        public static void SetY(this ref Vector2 v, float y)
        {
            v = new Vector2(v.x, y);
        }
        #endregion

        #region Vector3Int
        public static void SetX(this ref Vector3Int v, int x)
        {
            v = new Vector3Int(x, v.y, v.z);
        }
        public static void SetY(this ref Vector3Int v, int y)
        {
            v = new Vector3Int(v.x, y, v.z);
        }
        public static void SetZ(this ref Vector3Int v, int z)
        {
            v = new Vector3Int(v.x, v.y, z);
        }
        #endregion

        #region Vector2Int
        public static void SetX(this ref Vector2Int v, int x)
        {
            v = new Vector2Int(x, v.y);
        }
        public static void SetY(this ref Vector2Int v, int y)
        {
            v = new Vector2Int(v.x, y);
        }
        #endregion

        #endregion

        #region Add One Param

        #region Vector3
        public static void AddX(this ref Vector3 v, float x)
        {
            v = new Vector3(v.x + x, v.y, v.z);
        }
        public static void AddY(this ref Vector3 v, float y)
        {
            v = new Vector3(v.x, v.y + y, v.z);
        }
        public static void AddZ(this ref Vector3 v, float z)
        {
            v = new Vector3(v.x, v.y, v.z + z);
        }
        #endregion

        #region Vector2
        public static void AddX(this ref Vector2 v, float x)
        {
            v = new Vector2(v.x + x, v.y);
        }
        public static void AddY(this ref Vector2 v, float y)
        {
            v = new Vector2(v.x, v.y + y);
        }
        #endregion

        #region Vector3Int
        public static void AddX(this ref Vector3Int v, int x)
        {
            v = new Vector3Int(v.x + x, v.y, v.z);
        }
        public static void AddY(this ref Vector3Int v, int y)
        {
            v = new Vector3Int(v.x, v.y + y, v.z);
        }
        public static void AddZ(this ref Vector3Int v, int z)
        {
            v = new Vector3Int(v.x, v.y, v.z + z);
        }
        #endregion

        #region Vector2Int
        public static void AddX(this ref Vector2Int v, int x)
        {
            v = new Vector2Int(v.x + x, v.y);
        }
        public static void AddY(this ref Vector2Int v, int y)
        {
            v = new Vector2Int(v.x, v.y + y);
        }
        #endregion

        #endregion

        #region Set Two Params

        #region Vector2
        public static void SetXX(this ref Vector2 v, Vector2 t)
        {
            v = new Vector2(t.x, t.x);
        }
        public static void SetYY(this ref Vector2 v, Vector2 t)
        {
            v = new Vector2(t.y, t.y);
        }
        public static void SetZZ(this ref Vector2 v, Vector3 t)
        {
            v = new Vector2(t.z, t.z);
        }
        public static void SetXY(this ref Vector2 v, Vector2 t)
        {
            v = new Vector2(t.x, t.y);
        }
        public static void SetYX(this ref Vector2 v, Vector2 t)
        {
            v = new Vector2(t.y, t.x);
        }
        public static void SetXZ(this ref Vector2 v, Vector3 t)
        {
            v = new Vector2(t.x, t.z);
        }
        public static void SetZX(this ref Vector2 v, Vector3 t)
        {
            v = new Vector2(t.z, t.x);
        }
        public static void SetYZ(this ref Vector2 v, Vector3 t)
        {
            v = new Vector2(t.y, t.z);
        }
        public static void SetZY(this ref Vector2 v, Vector3 t)
        {
            v = new Vector2(t.z, t.y);
        }
        #endregion

        #region Vector3
        public static void SetXX(this ref Vector3 v, Vector2 t)
        {
            v = new Vector2(t.x, t.x);
        }
        public static void SetYY(this ref Vector3 v, Vector2 t)
        {
            v = new Vector2(t.y, t.y);
        }
        public static void SetZZ(this ref Vector3 v, Vector3 t)
        {
            v = new Vector2(t.z, t.z);
        }
        public static void SetXY(this ref Vector3 v, Vector2 t)
        {
            v = new Vector2(t.x, t.y);
        }
        public static void SetYX(this ref Vector3 v, Vector2 t)
        {
            v = new Vector2(t.y, t.x);
        }
        public static void SetXZ(this ref Vector3 v, Vector3 t)
        {
            v = new Vector2(t.x, t.z);
        }
        public static void SetZX(this ref Vector3 v, Vector3 t)
        {
            v = new Vector2(t.z, t.x);
        }
        public static void SetYZ(this ref Vector3 v, Vector3 t)
        {
            v = new Vector2(t.y, t.z);
        }
        public static void SetZY(this ref Vector3 v, Vector3 t)
        {
            v = new Vector2(t.z, t.y);
        }
        #endregion

        #region Vector2Int
        public static void SetXX(this ref Vector2Int v, Vector2Int t)
        {
            v = new Vector2Int(t.x, t.x);
        }
        public static void SetYY(this ref Vector2Int v, Vector2Int t)
        {
            v = new Vector2Int(t.y, t.y);
        }
        public static void SetZZ(this ref Vector2Int v, Vector3Int t)
        {
            v = new Vector2Int(t.z, t.z);
        }
        public static void SetXY(this ref Vector2Int v, Vector2Int t)
        {
            v = new Vector2Int(t.x, t.y);
        }
        public static void SetYX(this ref Vector2Int v, Vector2Int t)
        {
            v = new Vector2Int(t.y, t.x);
        }
        public static void SetXZ(this ref Vector2Int v, Vector3Int t)
        {
            v = new Vector2Int(t.x, t.z);
        }
        public static void SetZX(this ref Vector2Int v, Vector3Int t)
        {
            v = new Vector2Int(t.z, t.x);
        }
        public static void SetYZ(this ref Vector2Int v, Vector3Int t)
        {
            v = new Vector2Int(t.y, t.z);
        }
        public static void SetZY(this ref Vector2Int v, Vector3Int t)
        {
            v = new Vector2Int(t.z, t.y);
        }
        #endregion

        #region Vector3Int
        public static void SetXX(this ref Vector3Int v, Vector2Int t)
        {
            v = new Vector3Int(t.x, t.x);
        }
        public static void SetYY(this ref Vector3Int v, Vector2Int t)
        {
            v = new Vector3Int(t.y, t.y);
        }
        public static void SetZZ(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.z, t.z);
        }
        public static void SetXY(this ref Vector3Int v, Vector2Int t)
        {
            v = new Vector3Int(t.x, t.y);
        }
        public static void SetYX(this ref Vector3Int v, Vector2Int t)
        {
            v = new Vector3Int(t.y, t.x);
        }
        public static void SetXZ(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.x, t.z);
        }
        public static void SetZX(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.z, t.x);
        }
        public static void SetYZ(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.y, t.z);
        }
        public static void SetZY(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.z, t.y);
        }
        #endregion


        #endregion

        #region Set Three Params

        #region Vector3
        public static void SetXXX(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.x, t.x, t.x);
        }
        public static void SetYYY(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.y, t.y, t.y);
        }
        public static void SetZZZ(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.z, t.z, t.z);
        }
        public static void SetYXX(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.y, t.x, t.x);
        }
        public static void SetXYX(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.x, t.y, t.x);
        }
        public static void SetXXY(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.x, t.x, t.y);
        }
        public static void SetZXX(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.z, t.x, t.x);
        }
        public static void SetXZX(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.x, t.z, t.x);
        }
        public static void SetXXZ(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.x, t.x, t.z);
        }
        public static void SetXYY(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.x, t.y, t.y);
        }
        public static void SetYXY(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.y, t.x, t.y);
        }
        public static void SetYYX(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.y, t.y, t.x);
        }
        public static void SetZYY(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.z, t.y, t.y);
        }
        public static void SetYZY(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.y, t.z, t.y);
        }
        public static void SetYYZ(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.y, t.y, t.z);
        }
        public static void SetXZZ(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.x, t.z, t.z);
        }
        public static void SetZXZ(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.z, t.x, t.z);
        }
        public static void SetZZX(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.z, t.z, t.x);
        }
        public static void SetYZZ(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.y, t.z, t.z);
        }
        public static void SetZYZ(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.z, t.y, t.z);
        }
        public static void SetZZY(this ref Vector3 v, Vector3 t)
        {
            v = new Vector3(t.z, t.z, t.y);
        }
        #endregion

        #region Vector3Int
        public static void SetXXX(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.x, t.x, t.x);
        }
        public static void SetYYY(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.y, t.y, t.y);
        }
        public static void SetZZZ(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.z, t.z, t.z);
        }
        public static void SetYXX(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.y, t.x, t.x);
        }
        public static void SetXYX(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.x, t.y, t.x);
        }
        public static void SetXXY(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.x, t.x, t.y);
        }
        public static void SetZXX(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.z, t.x, t.x);
        }
        public static void SetXZX(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.x, t.z, t.x);
        }
        public static void SetXXZ(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.x, t.x, t.z);
        }
        public static void SetXYY(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.x, t.y, t.y);
        }
        public static void SetYXY(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.y, t.x, t.y);
        }
        public static void SetYYX(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.y, t.y, t.x);
        }
        public static void SetZYY(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.z, t.y, t.y);
        }
        public static void SetYZY(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.y, t.z, t.y);
        }
        public static void SetYYZ(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.y, t.y, t.z);
        }
        public static void SetXZZ(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.x, t.z, t.z);
        }
        public static void SetZXZ(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.z, t.x, t.z);
        }
        public static void SetZZX(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.z, t.z, t.x);
        }
        public static void SetYZZ(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.y, t.z, t.z);
        }
        public static void SetZYZ(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.z, t.y, t.z);
        }
        public static void SetZZY(this ref Vector3Int v, Vector3Int t)
        {
            v = new Vector3Int(t.z, t.z, t.y);
        }
        #endregion

        #endregion

        public static Vector3 ProjectAndScale(Vector3 vector, Vector3 normal)
        {
            float magnitude = vector.magnitude;
            vector = Vector3.ProjectOnPlane(vector, normal).normalized;
            vector *= magnitude;
            return vector;
        }
        
        [Serializable]
        public struct Vector3MinMaxRange
        {
            public Vector3 Min;
            public Vector3 Max;

            public Vector3 GetRandomRange()
            {
                return new Vector3(UnityEngine.Random.Range(Min.x, Max.x), UnityEngine.Random.Range(Min.y, Max.y), UnityEngine.Random.Range(Min.z, Max.z));
            }
        }
        
        [Serializable]
        public struct Vector3AnimationCurve
        {
            public AnimationCurve x;
            public AnimationCurve y;
            public AnimationCurve z;
        }
    }

    public static class CollectionsExtensions
    {
        public static T GetRandomItem<T>(this IList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
	public static T GetRandomItem<T>(this IList<T> list, int size)
	{
	    return list[UnityEngine.Random.Range(0, Mathf.Min(list.Count, size))];
	}
	public static T GetRandomItem<T>(this IList<T> list, int start, int end)
	{
	    return list[UnityEngine.Random.Range(Mathf.Max(0, start), Mathf.Min(list.Count, end))];
	}

        public static T GetRandomItemExcluding<T>(this IList<T> list, int excludedIndex)
        {
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            if (randomIndex == excludedIndex)
                return GetRandomItemExcluding(list, excludedIndex);
            else
                return list[randomIndex];
        }

        public static T GetRandomBetween<T>(T arr1, T arr2)
        {
            return UnityEngine.Random.Range(0, 2) == 1 ? arr1 : arr2;
        }

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int index = UnityEngine.Random.Range(0, list.Count);
                list.ExchangeAt(index, i);
            }
            return list;
        }

        public static void ExchangeAt<T>(this IList<T> list, int i1, int i2)
        {
            T value = list[i1];
            list[i1] = list[i2];
            list[i2] = value;
        }

        public static int[] FindAllIndexof<T>(this IEnumerable<T> values, T val)
        {
            return values.Select((b, i) => object.Equals(b, val) ? i : -1).Where(i => i != -1).ToArray();
        }

        public static GameObject GetRandomPonderatedGameObject(this IList<PonderableGameObject> list)
        {
            int total = 0;
            Dictionary<int, GameObject> gameObjectToSpawn = new Dictionary<int, GameObject>();
            foreach (var go in list)
            {
                gameObjectToSpawn.Add(total, go.GameObject);
                total += go.Probability;
            }

            int randomIndex = UnityEngine.Random.Range(0, total);
            int max = 0;

            foreach (var go in gameObjectToSpawn)
            {
                if (go.Key > randomIndex) continue;
                if (max < go.Key) max = go.Key;
            }

            return gameObjectToSpawn[max];
        }


        [Serializable]
        public struct PonderableGameObject
        {
            public int Probability;
            public GameObject GameObject;
        }
    }

    public static class TransformExtensions
    {
        public static void ResetTransform(this Transform transform)
        {
            ResetPosition(transform);
            ResetRotation(transform);
            ResetScale(transform);
        }

        public static void ResetPosition(this Transform transform)
        {
            transform.position = Vector3.zero;
        }

        public static void ResetRotation(this Transform transform)
        {
            transform.rotation = Quaternion.identity;
        }

        public static void ResetScale(this Transform transform)
        {
            transform.localScale = Vector3.one;
        }

        public static void SetTransform(this Transform transform, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = scale;
        }

        public static void SetTransform(this Transform transform, Transform targetTransform)
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
            transform.localScale = targetTransform.localScale;
        }

        public static GameObject GetClosestItem(this IList<GameObject> list, Vector3 position)
        {
            float maxDistance = float.MaxValue;
            GameObject gameObjectToReturn = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                var item = list[i];
                float distance = Mathf.Pow(item.transform.position.x - position.x, 2) + 
                                 Mathf.Pow(item.transform.position.y - position.y, 2) +
                                 Mathf.Pow(item.transform.position.z - position.z, 2);

                if (distance < maxDistance)
                {
                    maxDistance = distance;
                    gameObjectToReturn = item;
                }
            }
            return gameObjectToReturn;
        }

        public static RaycastHit GetClosestItem(this IList<RaycastHit> list, Vector3 position)
        {
            float maxDistance = float.MaxValue;
            RaycastHit raycastToReturn = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                var item = list[i];
                if (item.collider == null) continue;

                if (item.distance < maxDistance)
                {
                    maxDistance = item.distance;
                    raycastToReturn = item;
                }
            }
            return raycastToReturn;
        }
    }

    public static class GameObjectExtensions
    {
        /// <summary>
        /// ActivatesDeactivates all GameObjects from a IEnumerable, depending on the given true or false/ value.
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="value"></param>
        public static void SetAllActive(this IEnumerable<GameObject> gameObjects, bool value)
        {
            foreach (GameObject go in gameObjects)
            {
                go.SetActive(value);
            }
        }
        public static Transform Clear(this Transform transform)
        {
            while (transform.childCount > 0)
            {
                GameObject.Destroy(transform.GetChild(0).gameObject);
            }
            return transform;
        }

        public static Transform ClearImmediate(this Transform transform)
        {
            while (transform.childCount > 0)
            {
                GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
            }

            return transform;
        }

        public static void ToggleActivation(GameObject gameObject)
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }
    }

    public static class LayerExtensions
    {
        public static bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }
    }

    public static class ColliderExtensions
    {
        public static Vector3 right(this Bounds bounds) => new Vector3(bounds.max.x, bounds.center.y, bounds.center.z);
        public static Vector3 left(this Bounds bounds) => new Vector3(bounds.max.x, bounds.center.y, bounds.center.z);
        public static Vector3 top(this Bounds bounds) => new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
        public static Vector3 bottom(this Bounds bounds) => new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        public static Vector3 forward(this Bounds bounds) => new Vector3(bounds.center.x, bounds.center.y, bounds.max.z);
        public static Vector3 backward(this Bounds bounds) => new Vector3(bounds.center.x, bounds.center.y, bounds.min.z);
        public static Vector2 topLeft(this Bounds bounds) => new Vector2(bounds.min.x, bounds.max.y);
        public static Vector2 topRight(this Bounds bounds) => new Vector2(bounds.max.x, bounds.max.y);
        public static Vector2 bottomLeft(this Bounds bounds) => new Vector2(bounds.min.x, bounds.min.y);
        public static Vector2 bottomRight(this Bounds bounds) => new Vector2(bounds.max.x, bounds.min.y);

        public static void GetCapsulePoints(this CapsuleCollider collider, out Vector3 p1, out Vector3 p2)
        {
            Vector3 direction = collider.direction == 0 ? Vector3.right : collider.direction == 1 ? Vector3.up : Vector3.forward;
            p1 = collider.transform.position + collider.center + direction * -collider.height * 0.5F;
            p2 = p1 + direction * collider.height;
        }
    }
    
    public static class VisualExtensions
    {
        public static void SetAlpha(this SpriteRenderer renderer, float alpha)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
        }
        
        [Serializable]
        public struct ColorSquare
        {
            public Color TopLeft;
            public Color TopRight;
            public Color BottomLeft;
            public Color BottomRight;

            public Color this[int i]
            {
                get {
                    if (i == 1) return TopLeft;
                    else if (i == 2) return TopRight;
                    else if (i == 3) return BottomRight;
                    else return BottomLeft;
                }
            }
        }
    }

    public static class GizmoExtensions
    {
        public static void DrawSphereCast(Vector3 origin, float radius, Vector3 direction, float maxDistance, Color? color = null)
        {
            color ??= Color.white;
            Gizmos.color = (Color)color;
            Gizmos.DrawWireSphere(origin, radius);
            Gizmos.DrawLine(origin, origin + direction * maxDistance);
            Gizmos.DrawWireSphere(origin + direction * maxDistance, radius);
        }

        public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;
            DrawArrow(pos, direction, arrowHeadLength, arrowHeadAngle);
        }
        public static void DrawSphereCast(Vector3 origin, float radius, Vector3 direction, float maxDistance, Color color = default)
        {
            if (color == default) color = Color.white;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(origin, radius);
            Gizmos.DrawLine(origin, origin + direction * maxDistance);
            Gizmos.DrawWireSphere(origin + direction * maxDistance, radius);
        }

#if UNITY_EDITOR
        public static void DrawWireCircle(Vector3 center, Vector3 rotation, float radius, float thickness = 1f, Color color = default)
        {
            if (color == default) color = Color.white;
            Handles.color = color;
            Handles.DrawWireDisc(center, rotation, radius, thickness);
        }

        public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
        {
            if (_color != default(Color))
                Handles.color = _color;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (_height - (_radius * 2)) / 2;

                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
                Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
                Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);

            }
        }
#endif
    }

    public static class StringExtensions
    {
        public static string OrdinalSuffixOf(int index)
        {
            int dizains = index % 100;
            int units = index % 10;
            if (units == 1 && dizains != 11) return "st";
            else if (units == 2 && dizains != 12) return "nd";
            else if (units == 3 && dizains != 13) return "rd";
            else return "th";
        }
        
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }


    }

   /* public static class Tween
    {
        /// <summary>
        /// Tweens a float to a given value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="time"></param>
        /// <param name="ease"></param>
        public static void DOFloat(this float value, float target, float time, Ease ease = Ease.Unset)
        {
            DOTween.To(() => value, x => value = x, target, time).SetEase(ease);
        }

        /// <summary>
        /// Tweens an int to a given value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="time"></param>
        /// <param name="ease"></param>
        public static void DOInt(this int value, int target, float time, Ease ease = Ease.Unset)
        {
            DOTween.To(() => value, x => value = x, target, time).SetEase(ease);
        }

        [Serializable]
        public struct DoTweenPunchFeedback
        {
            [SerializeField] public float PunchTime;
            [SerializeField] public Vector3 PunchDirection;
            [SerializeField] public int PunchVibrato;
            [SerializeField] public float PunchElasticity;
        }

    }*/
   
   



   

}