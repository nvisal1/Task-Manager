namespace TaskManager.ExtensionMethods
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts a pascal case string to a camel case string
        /// 
        /// example
        /// 
        /// input -> CoolString
        /// output -> coolString
        /// 
        /// </summary>
        /// <param name="pascalCaseString"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string pascalCaseString)
        {
            if (pascalCaseString?.Length > 0)
            {
                string camelCaseString = pascalCaseString.Substring(0, 1).ToLowerInvariant() + pascalCaseString.Substring(1);

                return camelCaseString;
            }

            return pascalCaseString;
        }

        /// <summary>
        /// Returns the name of the property that caused the
        /// ModelState error
        /// 
        /// $. prefix is removed from the name if present
        /// </summary>
        /// <param name="modelStateKey"></param>
        /// <returns></returns>
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
