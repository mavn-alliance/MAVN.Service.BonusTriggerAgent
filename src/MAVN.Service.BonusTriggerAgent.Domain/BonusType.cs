using MAVN.Service.PartnerManagement.Client.Models;

namespace MAVN.Service.BonusTriggerAgent.Domain
{
    public static class BonusTypes
    {
        // General
        public static readonly BonusType SignUp =
            new BonusType("Sign up", "signup",
                null, 0, false, false, false, true, false, false, false);

        public static readonly BonusType EmailVerificationTrigger =
            new BonusType("Email verified", "emailverified",
                null, 1, false, false, false, false, false, false, false);

        public static readonly BonusType FriendReferred =
            new BonusType("Refer a friend to the app", "friend-referral",
                null, 2, false, false, false, true, false, false, false);

        // Real Estate
        public static readonly BonusType PropertyLeadApprovedReferral =
            new BonusType("Real Estate: Refer a purchaser", "estate-lead-referral",
                Vertical.RealEstate, 3, true, false, false, true, true, false, false);

        public static readonly BonusType OfferToPurchaseByLead =
            new BonusType("Connector earns when lead signs OTP", "estate-otp-referral",
                Vertical.RealEstate, 4, true, false, false, true, false, true, false);

        public static readonly BonusType ReferralPropertyFirstPurchase =
            new BonusType("Connector earns when the purchaser pays", "commission-one-referral",
                Vertical.RealEstate, 5, true, true, true, true, false, true, true);

        public static readonly BonusType PropertyPurchaseFirstReward =
            new BonusType("Real Estate: Purchaser earns when purchasing a property", "property-purchase-commission-one",
                Vertical.RealEstate, 7, false, true, true, true, false, false, false);

        public static readonly BonusType PropertyPurchasedByLead =
            new BonusType("Customer: Signs SPA", "estate-purchase",
                Vertical.RealEstate, 0, false, true, true, false, false, false, false);
        // Hospitality
        public static readonly BonusType HotelCheckoutReferral =
            new BonusType("Hospitality: Referrer earns when a customer stays in a hotel", "hotel-stay-referral",
                Vertical.Hospitality, 9, false, true, true, true, true, false, false);

        public static readonly BonusType HotelCheckout =
            new BonusType("Hospitality: Customer earns by staying in a hotel", "hotel-stay",
                Vertical.Hospitality, 10, false, true, true, true, false, false, false);

        // Retail
        public static readonly BonusType PurchaseReferral =
            new BonusType("Product purchase (referral)", "purchase-referral",
                Vertical.Retail, 12, false, true, true, false, false, false, false);
    }

    public class BonusType
    {
        public BonusType(string displayName, string eventName, Vertical? vertical, int order, bool allowInfinite,
            bool allowPercentage, bool allowConversionRate, bool isEnabled, bool isStakeable, bool isHidden, bool rewardHasRatio)
        {
            DisplayName = displayName;
            EventName = eventName;
            Vertical = vertical;
            Order = order;
            AllowInfinite = allowInfinite;
            AllowPercentage = allowPercentage;
            AllowConversionRate = allowConversionRate;
            IsEnabled = isEnabled;
            IsStakeable = isStakeable;
            IsHidden = isHidden;
            RewardHasRatio = rewardHasRatio;
        }

        public string DisplayName { get; }

        public string EventName { get; }

        public Vertical? Vertical { get; }

        public bool AllowInfinite { get; }

        public bool AllowPercentage { get; }

        public bool AllowConversionRate { get; }

        public bool IsEnabled { get; set; }

        public bool IsStakeable { get; set; }

        public bool IsHidden { get; set; }

        public int Order { get; set; }

        public bool RewardHasRatio { get; set; }
    }
}

