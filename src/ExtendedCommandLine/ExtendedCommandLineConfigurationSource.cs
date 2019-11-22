namespace SpoiledCat.Extensions.Configuration
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    public class ExtendedCommandLineConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Gets or sets the switch mappings.
        /// </summary>
        public IDictionary<string, string> SwitchMappings { get; set; }

        /// <summary>
        /// Gets or sets the command line args.
        /// </summary>
        public IEnumerable<string> Args { get; set; }

        /// <summary>
        /// Builds the <see cref="ExtendedCommandLineConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="ExtendedCommandLineConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ExtendedCommandLineConfigurationProvider(Args, SwitchMappings);
        }
    }
}
