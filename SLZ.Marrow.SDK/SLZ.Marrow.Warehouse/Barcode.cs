using SLZ.Marrow.Utilities;
using System;
using System.Text;
using UnityEngine;

namespace SLZ.Marrow.Warehouse
{
    [Serializable]
    public class Barcode : IEquatable<Barcode>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private string _id = EMPTY;
        public string ID
        {
            get => _id;
            private set => _id = value;
        }

        private string _group;
        private string _name;





        [SerializeField]
        [ReadOnly]
        private bool generated = false;

        public static string separator = ".";

        public static readonly string EMPTY = BuildBarcode("null", "empty", "barcode");
        private static readonly string EMPTY_OLD = "00000000-0000-0000-0000-000000000000";

        public static readonly int MAX_SIZE = 120;

        public static Barcode EmptyBarcode()
        {
            return new Barcode(EMPTY);
        }

        public Barcode()
        {
        }

        public Barcode(Barcode other)
        {
            if (other != null)
            {
                ID = other.ID;
                generated = other.generated;
            }
        }

        public Barcode(string newId)
        {
            ID = newId;
            generated = true;
        }

        public static string BuildBarcode(params string[] parts)
        {
            StringBuilder sb = new StringBuilder();

            bool first = true;
            foreach (var part in parts)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(separator);
                }
                sb.Append(MarrowSDK.SanitizeID(part));
            }

            return sb.ToString();
        }

        public static bool IsValidSize(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                return true;
            }
            return barcode.Length <= MAX_SIZE;
        }

        public static bool IsValid(string barcode)
        {
            bool valid = true;

            if (string.IsNullOrEmpty(barcode))
            {
                valid = false;
            }
            else if (barcode == EMPTY || barcode == EMPTY_OLD)
            {
                valid = false;
            }

            return valid;
        }


        public void GenerateID(params string[] parts)
        {
            GenerateID(false, parts);
        }

        public void GenerateID(bool forceGeneration = false, params string[] parts)
        {
            if (!generated || forceGeneration)
            {
                ID = "";

                ID = BuildBarcode(parts);

                generated = true;
            }
        }

        public static implicit operator string(Barcode b) => (b == null ? EMPTY : b.ToString());
        public static explicit operator Barcode(string s) => new Barcode(s);

        public override string ToString() => ID;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Barcode barcode)
                return Equals(barcode);
            if (obj is string objString)
                return Equals(new Barcode(objString));
            return false;
        }

        public bool Equals(Barcode other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return this.ID.Equals(other.ID);
        }

        public static bool operator ==(Barcode barcode, Barcode otherBarcode)
        {
            if (barcode is null)
            {
                if (otherBarcode is null)
                {
                    return true;
                }
                return false;
            }
            return barcode.Equals(otherBarcode);
        }

        public static bool operator !=(Barcode barcode, Barcode otherBarcode)
        {
            return !(barcode == otherBarcode);
        }

        public override int GetHashCode() => (ID).GetHashCode();

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            if (!this.IsValid())
            {
                this.ID = EMPTY;
            }
#endif
        }
    }

    public static class BarcodeExtensions
    {
        public static bool IsValid(this Barcode barcode)
        {
            return Barcode.IsValid(barcode.ID);
        }

        public static bool IsValidSize(this Barcode barcode)
        {
            return Barcode.IsValidSize(barcode);
        }
    }

}