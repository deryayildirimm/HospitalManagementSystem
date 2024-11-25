using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.BloodTests
{
    public static class BloodTestConst
    {
        
        private const string TestCategoryDefaultSorting = "{0}Name asc";
        private const string BloodTestDefaultSorting = "{0}CreationTime desc";

        public static string GetTestCategoryDefaultSorting(bool withEntityName)
        {
            return string.Format(TestCategoryDefaultSorting, withEntityName ? "TestCategory." : string.Empty);
        }

        public static string GetBloodTestDefaultSorting(bool withEntityName)
        {
            return string.Format(BloodTestDefaultSorting, withEntityName ? "BloodTest." : string.Empty);
        }
        public const int BloodTestStatusMin = 1;
        public const int BloodResultStatusMin = 1;
        public const int CategoryNameMin = 3;
        public const int CategoryDescriptionMin = 3;
        public const int TestNameMin = 3;


        public const int BloodTestStatusMax = 5;
        public const int BloodResultStatusMax = 3;
        public const int CategoryNameMax = 50;
        public const int CategoryDescriptionMax = 100;
        public const int TestNameMax = 50;
        
    }
}
