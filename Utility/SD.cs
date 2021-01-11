using System;
using BasePackageModule2.Models;

namespace BasePackageModule2.Utility
{
	public static class SD
	{
		public const string SuperAdmin = "SuperAdmin";
		public const string Admin = "Admin";
		public const string Manager = "Manager";
		public const string CustomerEndUser = "Customer";

		public const string DefaultProductImage = "product-default.jpg";

		public const string ssShoppingCartCount = "ssCartCount";
		public const string ssCouponCode = "ssCouponCode";



		public const string StatusSubmitted = "Submitted";
		public const string StatusInProcess = "BeingPrepared";
		public const string StatusReady = "Ready for Pickup";
		public const string StatusCompleted = "Completed";
		public const string StatusCancel = "Canceled";


		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusRejected = "Rejected";








		public static string ConvertToRawHtml(string source)
		{
			char[] array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;

			for (int i = 0; i < source.Length; i++)
			{
				char let = source[i];
				if (let == '<')
				{
					inside = true;
					continue;
				}
				if (let == '>')
				{
					inside = false;
					continue;
				}
				if (!inside)
				{
					array[arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}

		public static Double discountedPrice(Coupon couponFromDb, double originalOrderToatal)
		{
			if(couponFromDb == null)
			{
				return originalOrderToatal;
			}
			else
			{
				if(couponFromDb.MinimumAmount > originalOrderToatal)
				{
					return originalOrderToatal;
				}

				else
				{
					if(Convert.ToInt32(couponFromDb.CouponType)==(int)Coupon.ECouponType.Inr)

					{
						return Math.Round(originalOrderToatal - couponFromDb.Discount, 2);
					}
				
						if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Percent)

						{
							return Math.Round(originalOrderToatal - (originalOrderToatal*couponFromDb.Discount/100), 2);
						}
					
				}
			}

			return originalOrderToatal;
		}

	}
}
