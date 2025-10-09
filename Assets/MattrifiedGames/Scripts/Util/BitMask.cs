using UnityEngine;
using System.Collections;

namespace MattrifiedGames.Utility
{
    public static class BitMaskHelper
    {
        public const long L_ONE = (long)1;

        public const byte BYTE_00 = 1 << 0;
        public const byte BYTE_01 = 1 << 1;
        public const byte BYTE_02 = 1 << 2;
        public const byte BYTE_03 = 1 << 3;
        public const byte BYTE_04 = 1 << 4;
        public const byte BYTE_05 = 1 << 5;
        public const byte BYTE_06 = 1 << 6;
        public const byte BYTE_07 = 1 << 7;

        public const ushort USHRT_00 = 1 << 0;
        public const ushort USHRT_01 = 1 << 1;
        public const ushort USHRT_02 = 1 << 2;
        public const ushort USHRT_03 = 1 << 3;
        public const ushort USHRT_04 = 1 << 4;
        public const ushort USHRT_05 = 1 << 5;
        public const ushort USHRT_06 = 1 << 6;
        public const ushort USHRT_07 = 1 << 7;
        public const ushort USHRT_08 = 1 << 8;
        public const ushort USHRT_09 = 1 << 9;
        public const ushort USHRT_10 = 1 << 10;
        public const ushort USHRT_11 = 1 << 11;
        public const ushort USHRT_12 = 1 << 12;
        public const ushort USHRT_13 = 1 << 13;
        public const ushort USHRT_14 = 1 << 14;
        public const ushort USHRT_15 = 1 << 15;

        public const short BIT_08 = 1 << 8;
        public const short BIT_09 = 1 << 9;
        public const short BIT_10 = 1 << 10;
        public const short BIT_11 = 1 << 11;
        public const short BIT_12 = 1 << 12;
        public const short BIT_13 = 1 << 13;
        public const short BIT_14 = 1 << 14;
        public const int BIT_15 = 1 << 15;
        public const int BIT_16 = 1 << 16;

        public const int BIT_17 = 1 << 17;
        public const int BIT_18 = 1 << 18;
        public const int BIT_19 = 1 << 19;
        public const int BIT_20 = 1 << 20;
        public const int BIT_21 = 1 << 21;
        public const int BIT_22 = 1 << 22;
        public const int BIT_23 = 1 << 23;
        public const int BIT_24 = 1 << 24;
        public const int BIT_25 = 1 << 25;
        public const int BIT_26 = 1 << 26;
        public const int BIT_27 = 1 << 27;

        public const int BIT_28 = 1 << 28;
        public const int BIT_29 = 1 << 29;

        public const int BIT_30 = 1 << 30;
        public const int BIT_31 = 1 << 31;

        public const long LONG_31 = (long)1 << 31;

        public const long BIT_32 = (long)1 << 32;
        public const long BIT_33 = (long)1 << 33;
        public const long BIT_34 = (long)1 << 34;
        public const long BIT_35 = (long)1 << 35;
        public const long BIT_36 = (long)1 << 36;
        public const long BIT_37 = (long)1 << 37;
        public const long BIT_38 = (long)1 << 38;
        public const long BIT_39 = (long)1 << 39;

        public const long BIT_40 = (long)1 << 40;
        public const long BIT_41 = (long)1 << 41;
        public const long BIT_42 = (long)1 << 42;
        public const long BIT_43 = (long)1 << 43;
        public const long BIT_44 = (long)1 << 44;
        public const long BIT_45 = (long)1 << 45;
        public const long BIT_46 = (long)1 << 46;
        public const long BIT_47 = (long)1 << 47;
        public const long BIT_48 = (long)1 << 48;
        public const long BIT_49 = (long)1 << 49;

        public const long BIT_50 = (long)1 << 50;
        public const long BIT_51 = (long)1 << 51;
        public const long BIT_52 = (long)1 << 52;
        public const long BIT_53 = (long)1 << 53;
        public const long BIT_54 = (long)1 << 54;
        public const long BIT_55 = (long)1 << 55;
        public const long BIT_56 = (long)1 << 56;
        public const long BIT_57 = (long)1 << 57;
        public const long BIT_58 = (long)1 << 58;
        public const long BIT_59 = (long)1 << 59;

        public const long BIT_60 = (long)1 << 60;
        public const long BIT_61 = (long)1 << 61;
        public const long BIT_62 = (long)1 << 62;
        public const long BIT_63 = (long)1 << 63;

        public static int GetBit32(int bit)
        {
            return 1 << bit;
        }

        public static long GetBit64(int bit)
        {
            return (long)1 << bit;
        }

        public static void DebugTest()
        {
            Debug.Log(BIT_31);
            Debug.Log(LONG_31);
        }

        /* Adding and removing values
         *
         * Adding a bit index add a bit
         * so AddBitIndex(ref myValue, 0) would go from 
         * 000...
         * to
         * 100...
         * 
         * then
         * 
         * RemoveBitIndex(ref muValue, 0) would go from
         * 100...
         * to
         * 000...
         * 
         * Meanwhile, AddBit can be used as
         * AddBit(ref value, BIT_0);
         * or maybe
         * AddBit(ref value, BIT_0 | BIT_1)
         * to go from
         * 000...
         * 110...
         * 
         */

        public static void AddBitIndex(ref int mask, int indexToAdd)
        {
            mask |= 1 << indexToAdd;
        }

        public static void RemoveBitIndex(ref int mask, int indexToRemove)
        {
            mask &= ~(1 << indexToRemove);
        }

        public static void AddBit(ref int mask, int valueToAdd)
        {
            mask |= valueToAdd;
        }

        public static void RemoveBit(ref int mask, int valueToRemove)
        {
            mask &= ~(valueToRemove);
        }
    }

    [System.Serializable()]
    public struct ByteMask
    {
        [SerializeField()]
        public byte mask;

        public bool CheckPosition(byte index)
        {
            if (index >= 8)
                return false;

            return StaticHelpers.ByteMaskCheck(mask, 1 << index);
        }

        public bool CheckMask(byte mask)
        {
            return StaticHelpers.ByteMaskCheck(mask, 1 << mask);
        }
    }

    [System.Serializable()]
    public struct ShortMask
    {
        [SerializeField()]
        public short mask;

        public bool CheckPosition(short index)
        {
            if (index >= 16)
                return false;

            return StaticHelpers.ByteMaskCheck(mask, 1 << index);
        }

        public bool CheckMask(short index)
        {
            return StaticHelpers.ByteMaskCheck(mask, 1 << index);
        }
    }

    [System.Serializable()]
    public struct IntMask
    {
        [SerializeField()]
        public int mask;

        public bool CheckPosition(int index)
        {
            if (index >= 32)
                return false;

            return StaticHelpers.ByteMaskCheck(mask, 1 << index);
        }

        public bool CheckMask(int index)
        {
            return StaticHelpers.ByteMaskCheck(mask, 1 << index);
        }
    }
}