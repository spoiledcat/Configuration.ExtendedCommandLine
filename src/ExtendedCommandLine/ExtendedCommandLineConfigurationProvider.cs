namespace SpoiledCat.Extensions.Configuration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// A command line based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public class ExtendedCommandLineConfigurationProvider : ConfigurationProvider
    {
        private readonly Dictionary<string, string> _switchMappings;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="args">The command line args.</param>
        /// <param name="switchMappings">The switch mappings.</param>
        public ExtendedCommandLineConfigurationProvider(IEnumerable<string> args, IDictionary<string, string> switchMappings = null)
        {
            Args = args ?? throw new ArgumentNullException(nameof(args));

            if (switchMappings != null)
            {
                _switchMappings = GetValidatedSwitchMappingsCopy(switchMappings);
            }
        }

        /// <summary>
        /// The command line arguments.
        /// </summary>
        protected IEnumerable<string> Args { get; private set; }

        /// <summary>
        /// Loads the configuration data from the command line args.
        /// </summary>
        public override void Load()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string key, value;

            using (var enumerator = Args.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var currentArg = enumerator.Current;
                    var keyStartIndex = 0;

                    if (currentArg.StartsWith("--"))
                    {
                        keyStartIndex = 2;
                    }
                    else if (currentArg.StartsWith("-"))
                    {
                        keyStartIndex = 1;
                    }
                    else if (currentArg.StartsWith("/"))
                    {
                        // "/SomeSwitch" is equivalent to "--SomeSwitch" when interpreting switch mappings
                        // So we do a conversion to simplify later processing
                        currentArg = string.Format("--{0}", currentArg.Substring(1));
                        keyStartIndex = 2;
                    }

                    var separator = currentArg.IndexOf('=');

                    if (separator < 0)
                    {
                        // If there is neither equal sign nor prefix in current arugment, it is an invalid format
                        if (keyStartIndex == 0)
                        {
                            // Ignore invalid formats
                            continue;
                        }

                        // If the switch is a key in given switch mappings, interpret it
                        if (_switchMappings != null && _switchMappings.TryGetValue(currentArg, out var mappedKey))
                        {
                            key = mappedKey;
                        }
                        // Otherwise, use the switch name directly as a key
                        else
                        {
                            key = currentArg.Substring(keyStartIndex);
                        }

                        var previousKey = enumerator.Current;
                        if (!enumerator.MoveNext())
                        {
                            // ignore missing values
                            continue;
                        }

                        value = enumerator.Current;
                    }
                    else
                    {
                        var keySegment = currentArg.Substring(0, separator);

                        // If the switch is a key in given switch mappings, interpret it
                        if (_switchMappings != null && _switchMappings.TryGetValue(keySegment, out var mappedKeySegment))
                        {
                            key = mappedKeySegment;
                        }
                        // Otherwise, use the switch name directly as a key
                        else
                        {
                            key = currentArg.Substring(keyStartIndex, separator - keyStartIndex);
                        }

                        value = currentArg.Substring(separator + 1);
                    }

                    // Override value when key is duplicated. So we always have the last argument win.
                    data[key] = value;
                }
            }

            Data = data;
        }

        private Dictionary<string, string> GetValidatedSwitchMappingsCopy(IDictionary<string, string> switchMappings)
        {
            // The dictionary passed in might be constructed with a case-sensitive comparer
            // However, the keys in configuration providers are all case-insensitive
            // So we check whether the given switch mappings contain duplicated keys with case-insensitive comparer
            var switchMappingsCopy = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var mapping in switchMappings)
            {
                // Only keys start with "--" or "-" are acceptable
                if (!mapping.Key.StartsWith("-") && !mapping.Key.StartsWith("--"))
                {
                    throw new ArgumentException(
                        $"InvalidSwitchMapping {mapping.Key}",
                        nameof(switchMappings));
                }

                if (switchMappingsCopy.ContainsKey(mapping.Key))
                {
                    throw new ArgumentException(
                        $"DuplicatedKeyInSwitchMappings {mapping.Key}",
                        nameof(switchMappings));
                }

                switchMappingsCopy.Add(mapping.Key, mapping.Value);
            }

            return switchMappingsCopy;
        }
    }
}
