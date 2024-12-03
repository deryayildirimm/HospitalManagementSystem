namespace Pusula.Training.HealthCare.Protocols
{
    public static class ProtocolConsts
    {
        private const string DefaultSorting = "{0}StartTime asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Protocol." : string.Empty);
        }

        public const int MinNotesLength = 1;
        public const int MaxNotesLength = 100;

    }
}