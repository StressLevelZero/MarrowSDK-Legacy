using UnityEngine;

namespace SLZ.Marrow.Utilities
{
    [System.Serializable]
    public struct SimpleTransform
    {
        public static SimpleTransform Identity = new() { position = Vector3.zero, rotation = Quaternion.identity };

        public Vector3 position;
        public Quaternion rotation;

        public Vector3 forward
        {
            get
            {
                return rotation * Vector3.forward;
            }
        }

        public Vector3 up
        {
            get
            {
                return rotation * Vector3.up;
            }
        }

        public Vector3 right
        {
            get
            {
                return rotation * Vector3.right;
            }
        }

        public SimpleTransform inverse
        {
            get
            {
                return Inverse(position, rotation);
            }
        }

        public static SimpleTransform Create(Vector3 p, Quaternion r)
        {
            return new SimpleTransform(p, r);
        }

        public static SimpleTransform Create(SimpleTransform st)
        {
            return new SimpleTransform(st);
        }

        public static SimpleTransform Inverse(Transform t)
        {
            return Inverse(t.position, t.rotation);
        }

        public static SimpleTransform Create(Transform t)
        {
            return new SimpleTransform(t);
        }

        public static SimpleTransform Inverse(SimpleTransform st)
        {
            return Inverse(st.position, st.rotation);
        }

        public static SimpleTransform Inverse(Vector3 p, Quaternion r)
        {

            Quaternion inverse = Quaternion.Inverse(r);
            return new SimpleTransform(
                inverse * -p,
                inverse
            );
        }

        public static SimpleTransform Transform(Transform transformA, SimpleTransform transformB)
        {
            return new SimpleTransform(transformA).Transform(transformB);
        }

        public static SimpleTransform Transform(Transform transformA, Transform transformB)
        {
            var st = new SimpleTransform(transformA);
            return st.InverseTransform(transformB);
        }

        public static SimpleTransform InverseTransform(Transform transformA, SimpleTransform transformB)
        {
            var st = new SimpleTransform(transformA);
            return st.InverseTransform(transformB);
        }

        public static SimpleTransform InverseTransform(Transform transformA, Transform transformB)
        {
            return new SimpleTransform(transformA).InverseTransform(transformB);
        }

        public SimpleTransform(Vector3 p, Quaternion r)
        {
            position = p;
            rotation = r;
        }

        public SimpleTransform(Transform t)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            Copy(t);
        }

        public SimpleTransform(SimpleTransform st)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            Copy(st);
        }

        public void Copy(SimpleTransform st)
        {
            position = st.position;
            rotation = st.rotation;
        }

        public void Copy(Transform t)
        {
            position = t.position;
            rotation = t.rotation;
        }

        public void CopyLocal(Transform t)
        {
            position = t.localPosition;
            rotation = t.localRotation;
        }

        public void CopyToLocal(Transform t)
        {
            t.localPosition = position;
            t.localRotation = rotation;
        }

        public void CopyTo(Transform t)
        {
            t.SetPositionAndRotation(position, rotation);
        }

        public SimpleTransform InverseTransform(Transform t)
        {
            return InverseTransform(t.position, t.rotation);
        }

        public SimpleTransform InverseTransform(SimpleTransform st)
        {
            return InverseTransform(st.position, st.rotation);
        }

        public SimpleTransform InverseTransform(Vector3 p, Quaternion r)
        {
            Quaternion inverse = Quaternion.Inverse(rotation);
            return new SimpleTransform(
                inverse * (p - position),
                inverse * r
            );
        }
        public Vector3 InverseTransformPoint(Vector3 p)
        {
            return Quaternion.Inverse(rotation) * (p - position);
        }
        public Vector3 InverseTransformDirection(Vector3 d)
        {
            return Quaternion.Inverse(rotation) * d;
        }
        public Quaternion InverseTransformRotation(Quaternion r)
        {
            return Quaternion.Inverse(rotation) * r;
        }


        public SimpleTransform RotateFrom(Quaternion r)
        {
            return new SimpleTransform(r * position, r * rotation);
        }

        public SimpleTransform Transform(Transform t)
        {
            return Transform(t.position, t.rotation);
        }

        public SimpleTransform Transform(SimpleTransform st)
        {
            return Transform(st.position, st.rotation);
        }

        public SimpleTransform Transform(Vector3 p, Quaternion r)
        {
            return new SimpleTransform(
                TransformPoint(p),
                TransformRotation(r)
            );
        }

        public Vector3 TransformPoint(Vector3 p)
        {
            return position + rotation * p;
        }
        public Vector3 TransformDirection(Vector3 d)
        {
            return rotation * d;
        }
        public Quaternion TransformRotation(Quaternion r)
        {
            return rotation * r;
        }


        public Vector3 DirectionTo(Vector3 p)
        {
            return p - position;
        }

        public Vector3 DirectionTo(SimpleTransform st)
        {
            return st.position - position;
        }

        public Vector3 DirectionTo(Transform t)
        {
            return t.position - position;
        }

        public Vector3 DirectionFrom(Vector3 p)
        {
            return position - p;
        }

        public Vector3 DirectionFrom(SimpleTransform st)
        {
            return position - st.position;
        }

        public Vector3 DirectionFrom(Transform t)
        {
            return position - t.position;
        }

        public static SimpleTransform Lerp(SimpleTransform fromST, SimpleTransform toST, float perc)
        {
            return new SimpleTransform
            {
                position = Vector3.Lerp(fromST.position, toST.position, perc),
                rotation = Quaternion.Lerp(fromST.rotation, toST.rotation, perc)
            };
        }
    }
}
