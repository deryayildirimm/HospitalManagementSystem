using System;

namespace Pusula.Training.HealthCare.MedicalServices
{
    public static class MedicalServiceConsts
    {
        private const string DefaultSorting = "{0}Name asc";
        private const string DefaultDoctorSorting = "{0}FirstName asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "MedicalService." : string.Empty);
        }
        public static string GetDefaultDoctorSorting(bool withEntityName)
        {
            return string.Format(DefaultDoctorSorting, withEntityName ? "MedicalService." : string.Empty);
        }
        
        public const int NameMaxLength = 128;


        public const int ServiceTypeMinValue = 1;
        public const int ServiceTypeMaxValue = 3;
        
        public const int ServiceTypeMinLength = 1;
        public const int ServiceTypeMaxLength = 128;

        public const double CostMinValue = 0;
        public const double CostMaxValue = double.MaxValue;
        
        public const int DurationMinValue = 10;
        public const int DurationMaxValue = 200;
    }
}