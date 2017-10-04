using UnityEngine;
using System.Collections;

namespace PlayPhone
{
	public enum Gravity
	{
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8,
	}

	public static class GravityConverter
	{
		private enum SdkGravity
		{
			Left = 3,
			Right = 5,
			Top = 48,
			Bottom = 80,
		}

		public static int ToInt(Gravity gravity)
		{
			int result = 0;
			if (((int)gravity & (int)Gravity.Left) != 0)
			{
				result |= (int)SdkGravity.Left;
			}
			if (((int)gravity & (int)Gravity.Right) != 0)
			{
				result |= (int)SdkGravity.Right;
			}
			if (((int)gravity & (int)Gravity.Top) != 0)
			{
				result |= (int)SdkGravity.Top;
			}
			if (((int)gravity & (int)Gravity.Bottom) != 0)
			{
				result |= (int)SdkGravity.Bottom;
			}
			return result;
		}

		public static Gravity FromInt(int value)
		{
			int result = 0;
			if ((value & (int)SdkGravity.Left) == (int)SdkGravity.Left)
			{
				result |= (int)Gravity.Left;
			}
			if ((value & (int)SdkGravity.Right) == (int)SdkGravity.Right)
			{
				result |= (int)Gravity.Right;
			}
			if ((value & (int)SdkGravity.Top) == (int)SdkGravity.Top)
			{
				result |= (int)Gravity.Top;
			}
			if ((value & (int)SdkGravity.Bottom) == (int)SdkGravity.Bottom)
			{
				result |= (int)Gravity.Bottom;
			}
			return (Gravity)result;
		}
	}
}
