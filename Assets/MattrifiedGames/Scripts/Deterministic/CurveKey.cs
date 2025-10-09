
#region License

/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Authors:
Olivier Dufour (Duff)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion License

using System;
namespace TrueSync
{
    [System.Serializable()]
    public class FPCurveKey : IEquatable<FPCurveKey>, IComparable<FPCurveKey>
    {
#region Private Fields

        [UnityEngine.SerializeField()]
        private CurveContinuity continuity;

        [UnityEngine.SerializeField()]
        private FP position;

        [UnityEngine.SerializeField()]
        private FP value;

        [UnityEngine.SerializeField()]
        private FP tangentIn;

        [UnityEngine.SerializeField()]
        private FP tangentOut;

#endregion Private Fields

#region Properties

        public CurveContinuity Continuity
        {
            get { return continuity; }
            set { continuity = value; }
        }

        public FP Position
        {
            get { return position; }
        }

        public FP TangentIn
        {
            get { return tangentIn; }
            set { tangentIn = value; }
        }

        public FP TangentOut
        {
            get { return tangentOut; }
            set { tangentOut = value; }
        }

        public FP Value
        {
            get { return value; }
            set { this.value = value; }
        }

#endregion

#region Constructors

        public FPCurveKey(FP position, FP value)
            : this(position, value, 0, 0, CurveContinuity.Smooth)
        {
        }

        public FPCurveKey(FP position, FP value, FP tangentIn, FP tangentOut)
            : this(position, value, tangentIn, tangentOut, CurveContinuity.Smooth)
        {
        }

        public FPCurveKey(FP position, FP value, FP tangentIn, FP tangentOut, CurveContinuity continuity)
        {
            this.position = position;
            this.value = value;
            this.tangentIn = tangentIn;
            this.tangentOut = tangentOut;
            this.continuity = continuity;
        }

#endregion Constructors

#region Public Methods

#region IComparable<CurveKey> Members

        public int CompareTo(FPCurveKey other)
        {
            return position.CompareTo(other.position);
        }

#endregion

#region IEquatable<CurveKey> Members

        public bool Equals(FPCurveKey other)
        {
            return (this == other);
        }

#endregion

        public static bool operator !=(FPCurveKey a, FPCurveKey b)
        {
            return !(a == b);
        }

        public static bool operator ==(FPCurveKey a, FPCurveKey b)
        {
            if (Equals(a, null))
                return Equals(b, null);

            if (Equals(b, null))
                return Equals(a, null);

            return (a.position == b.position)
                   && (a.value == b.value)
                   && (a.tangentIn == b.tangentIn)
                   && (a.tangentOut == b.tangentOut)
                   && (a.continuity == b.continuity);
        }

        public FPCurveKey Clone()
        {
            return new FPCurveKey(position, value, tangentIn, tangentOut, continuity);
        }

        public override bool Equals(object obj)
        {
            return (obj is FPCurveKey) ? ((FPCurveKey) obj) == this : false;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() ^ value.GetHashCode() ^ tangentIn.GetHashCode() ^
                   tangentOut.GetHashCode() ^ continuity.GetHashCode();
        }

#endregion
    }
}
