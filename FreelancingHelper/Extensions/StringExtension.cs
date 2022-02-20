namespace FreelancingHelper.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmptyOrWhiteSpace(this string str) =>
            string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
    }
}
