using System.Collections.Generic;

namespace check_in_out.Common.Utils
{
    public class TypeFactory
    {
        public static TypeFactory Instance { get; } = new TypeFactory();
        private readonly Dictionary<int, string> MapTypeFactors = new Dictionary<int, string>();

        private TypeFactory()
        {
            if (MapTypeFactors != null || MapTypeFactors.Count == 0)
            {
                MapTypeFactors.Add(0, "check in");
                MapTypeFactors.Add(1, "check out");
            }
        }

        public string GetTypeDescription(int type)
        {
            return MapTypeFactors[type];
        }
    }
}
