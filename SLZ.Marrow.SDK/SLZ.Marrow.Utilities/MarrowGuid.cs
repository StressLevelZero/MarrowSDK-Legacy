using System;
using System.Collections;
using UnityEngine;

namespace SLZ.Marrow.Utilities
{
    [Serializable]
    public struct MarrowGuid : IEquatable<MarrowGuid>
    {
        [SerializeField]
        private byte[] _byteArray;
        public byte[] ByteArray => _byteArray;

        public MarrowGuid(byte[] byteArray)
        {
            _byteArray = new byte[16];
            byteArray.CopyTo(_byteArray, 0);
        }

        public MarrowGuid(string hexString)
        {
            _byteArray = new byte[16];
            FromHexString(hexString);
        }

        public void GenerateGuid(bool firstBit)
        {
            var guid = Guid.NewGuid();
            var guidBytes = guid.ToByteArray();

            var bitArray = new BitArray(guidBytes);
            bitArray[(8 * 4) - 1] = firstBit;
            bitArray.CopyTo(guidBytes, 0);

            _byteArray = guidBytes;
        }

        public bool Equals(MarrowGuid other)
        {
            return Equals(_byteArray, other._byteArray);
        }

        public override bool Equals(object obj)
        {
            return obj is MarrowGuid other && Equals(other);
        }

        public static bool operator ==(MarrowGuid guid, MarrowGuid otherGuid)
        {
            return guid.Equals(otherGuid);
        }

        public static bool operator !=(MarrowGuid guid, MarrowGuid otherGuid)
        {
            return !guid.Equals(otherGuid);
        }

        public override int GetHashCode()
        {
            return (_byteArray != null ? _byteArray.GetHashCode() : 0);
        }

        public bool IsValid()
        {
            return IsValid(this);
        }

        public static bool IsValid(MarrowGuid guid)
        {
            return guid._byteArray != null && guid._byteArray.Length == 16;
        }

        public string ToHexString()
        {
            Guid guid = new Guid(ByteArray);
            return guid.ToString();
        }

        public void FromHexString(string hexString)
        {
            Guid guid = new Guid(hexString);
            if (_byteArray == null)
                _byteArray = new byte[16];
            guid.ToByteArray().CopyTo(_byteArray, 0);
        }
    }
}