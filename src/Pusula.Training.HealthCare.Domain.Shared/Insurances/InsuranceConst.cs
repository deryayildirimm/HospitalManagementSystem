
namespace Pusula.Training.HealthCare.Insurances
{
    public static class InsuranceConst
    {
        private const string DefaultSorting = "{0}InsuranceCompanyName asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Insurance." : string.Empty);
        }

        public const int MinPolicyNumberLength = 5;
        public const int MaxPolicyNumberLength = 20;

        public const decimal MinPremiumAmount = 0;
        public const decimal MaxPremiumAmount = 1000000;

        public const decimal MinCoverageAmount = 1000;
        public const decimal MaxCoverageAmount = 500000;

        public const int MinInsuranceCompanyName = 1;
        public const int MaxInsuranceCompanyName = 9;

        public const int MaxDescriptionLength = 500;

    }
}
