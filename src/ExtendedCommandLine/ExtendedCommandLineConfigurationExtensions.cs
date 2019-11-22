namespace SpoiledCat.Extensions.Configuration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    public static class ExtendedCommandLineConfigurationExtensions
    {
        /// <summary>
        ///   Adds a <see cref="ExtendedCommandLineConfigurationProvider"/> <see cref="IConfigurationProvider"/>
        ///   that reads configuration values from the command line.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="args">The command line args.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        /// <remarks>
        ///   <para>
        ///     The values passed on the command line, in the <c>args</c> string array, should be a set
        ///     of keys prefixed with one ("-") or two dashes ("--") and then values, separate by either the
        ///     equals sign ("=") or a space (" ").
        ///   </para>
        ///   <para>
        ///     A forward slash ("/") can be used as an alternative prefix, with either equals or space, and when using
        ///     an equals sign the prefix can be left out altogether.
        ///   </para>
        ///   <para>
        ///     There are seven basic alternative formats for arguments:
        ///     <c>key1=value1 -key2=value2 --key3=value3 /key4=value4 -key5 --key6 value6 /key7 value7</c>.
        ///   </para>
        /// </remarks>
        /// <example>
        ///   A simple console application that has seven values.
        ///   <code>
        ///     // dotnet run key1=value1 -key2=value2 --key3=value3 /key4=value4 -key5 --key6 value6 /key7 value7
        ///
        ///     using Microsoft.Extensions.Configuration;
        ///     using System;
        ///
        ///     namespace CommandLineSample
        ///     {
        ///        public class Program
        ///        {
        ///            public static void Main(string[] args)
        ///            {
        ///                var builder = new ConfigurationBuilder();
        ///                builder.AddExtendedCommandLine(args);
        ///
        ///                var config = builder.Build();
        ///
        ///                Console.WriteLine($"Key1: '{config["Key1"]}'");
        ///                Console.WriteLine($"Key2: '{config["Key2"]}'");
        ///                Console.WriteLine($"Key3: '{config["Key3"]}'");
        ///                Console.WriteLine($"Key4: '{config["Key4"]}'");
        ///                Console.WriteLine($"Key5: '{config["Key5"]}'");
        ///                Console.WriteLine($"Key6: '{config["Key6"]}'");
        ///                Console.WriteLine($"Key7: '{config["Key7"]}'");
        ///            }
        ///        }
        ///     }
        ///   </code>
        /// </example>
        public static IConfigurationBuilder AddExtendedCommandLine(this IConfigurationBuilder configurationBuilder, string[] args)
        {
            return configurationBuilder.AddExtendedCommandLine(args, switchMappings: null);
        }

        /// <summary>
        ///   Adds a <see cref="ExtendedCommandLineConfigurationProvider"/> <see cref="IConfigurationProvider"/> that reads
        ///   configuration values from the command line using the specified switch mappings.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="args">The command line args.</param>
        /// <param name="switchMappings">
        ///   The switch mappings. A dictionary of short (with prefix "-") and
        ///   alias keys (with prefix "--"), mapped to the configuration key (no prefix).
        /// </param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        /// <remarks>
        ///   <para>
        ///     The <c>switchMappings</c> allows additional formats for alternative short and alias keys
        ///     to be used from the command line. Also see the basic version of <c>AddCommandLine</c> for
        ///     the standard formats supported.
        ///   </para>
        ///   <para>
        ///     Short keys start with a single dash ("-") and are mapped to the main key name (without
        ///     prefix), and can be used with either equals or space.
        ///   </para>
        ///   <para>
        ///     There are two formats for short arguments:
        ///     <c>-k1=value1 -k2 value2</c>.
        ///   </para>
        ///   <para>
        ///     Alias key definitions start with two dashes ("--") and are mapped to the main key name (without
        ///     prefix), and can be used in place of the normal key. They also work when a forward slash prefix
        ///     is used in the command line (but not with the no prefix equals format).
        ///   </para>
        ///   <para>
        ///     There are only four formats for aliased arguments:
        ///     <c>--alt3=value3 /alt4=value4 --alt5 value5 /alt6 value6</c>.
        ///   </para>
        /// </remarks>
        /// <example>
        ///   A simple console application that has two short and four alias switch mappings defined.
        ///   <code>
        ///     // dotnet run -k1=value1 -k2 value2 --alt3=value2 /alt4=value3 --alt5 value5 /alt6 value6
        ///
        ///     using Microsoft.Extensions.Configuration;
        ///     using System;
        ///     using System.Collections.Generic;
        ///
        ///     namespace CommandLineSample
        ///     {
        ///        public class Program
        ///        {
        ///            public static void Main(string[] args)
        ///            {
        ///                var switchMappings = new Dictionary&lt;string, string&gt;()
        ///                {
        ///                    { "-k1", "key1" },
        ///                    { "-k2", "key2" },
        ///                    { "--alt3", "key3" },
        ///                    { "--alt4", "key4" },
        ///                    { "--alt5", "key5" },
        ///                    { "--alt6", "key6" },
        ///                };
        ///                var builder = new ConfigurationBuilder();
        ///                builder.AddExtendedCommandLine(args, switchMappings);
        ///
        ///                var config = builder.Build();
        ///
        ///                Console.WriteLine($"Key1: '{config["Key1"]}'");
        ///                Console.WriteLine($"Key2: '{config["Key2"]}'");
        ///                Console.WriteLine($"Key3: '{config["Key3"]}'");
        ///                Console.WriteLine($"Key4: '{config["Key4"]}'");
        ///                Console.WriteLine($"Key5: '{config["Key5"]}'");
        ///                Console.WriteLine($"Key6: '{config["Key6"]}'");
        ///                Console.WriteLine($"Key7: '{config["Key7"]}'");
        ///            }
        ///        }
        ///     }
        ///   </code>
        /// </example>
        public static IConfigurationBuilder AddExtendedCommandLine(
            this IConfigurationBuilder configurationBuilder,
            string[] args,
            IDictionary<string, string> switchMappings)
        {
            configurationBuilder.Add(new ExtendedCommandLineConfigurationSource { Args = args, SwitchMappings = switchMappings });
            return configurationBuilder;
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from the command line.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddExtendedCommandLine(this IConfigurationBuilder builder, Action<ExtendedCommandLineConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}
