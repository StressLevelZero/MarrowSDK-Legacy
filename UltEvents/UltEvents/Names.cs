// UltEvents // Copyright 2021 Kybernetik //

namespace UltEvents
{
    /// <summary>[Editor-Only] The names of various types and members in <see cref="UltEvents"/>.</summary>
    internal static class Names
    {
        public const string
            Namespace = "UltEvents",
            PersistentArgumentType = "PersistentArgumentType";

        /// <summary>[Editor-Only] The names of various members in <see cref="UltEvents.PersistentArgument"/>.</summary>
        internal static class PersistentArgument
        {
            public const string
                Class = "PersistentArgument",

                Type = "_Type",
                Int = "_Int",
                String = "_String",
                X = "_X",
                Y = "_Y",
                Z = "_Z",
                W = "_W",
                Object = "_Object";

            /// <summary>[Editor-Only] The full names of various members in <see cref="UltEvents.PersistentArgument"/>.</summary>
            internal static class Full
            {
                public const string
                    Type = "PersistentArgument.Type";
            }
        }

        /// <summary>[Editor-Only] The names of various members in <see cref="UltEvents.PersistentCall"/>.</summary>
        internal static class PersistentCall
        {
            public const string
                Target = "_Target",
                MethodName = "_MethodName",
                PersistentArguments = "_PersistentArguments";
        }

        /// <summary>[Editor-Only] The names of various members in <see cref="UltEvents.UltEvent"/>.</summary>
        internal static class UltEvent
        {
            public const string
                Class = "UltEvent",
                PersistentCalls = "_PersistentCalls";
        }
    }
}