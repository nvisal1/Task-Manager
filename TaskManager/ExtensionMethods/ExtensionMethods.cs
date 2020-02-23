namespace TaskManager.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static string ToCamelCase(this string pascalCaseString)
        {
            if (pascalCaseString?.Length > 0)
            {
                string camelCaseString = pascalCaseString.Substring(0, 1).ToLowerInvariant() + pascalCaseString.Substring(1);

                return camelCaseString;
            }

            return pascalCaseString;
        }
        public static string CleanseModelStateKey(this string modelStateKey)
        {
            if (modelStateKey?.StartsWith("$.") == true)
            {
                return modelStateKey.Substring(2);
            }
            return modelStateKey;
        }
    }
}
