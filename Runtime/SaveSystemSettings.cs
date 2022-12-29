namespace SaveSystem
{
    /// <summary>
    ///     The settings used by the save system.
    /// </summary>
    public class SaveSystemSettings
    {
        /// <summary>
        ///     Whether to use compression.
        /// </summary>
        public bool compress;

        /// <summary>
        ///     The format to save in.
        /// </summary>
        public SaveSystem.Format format;

        /// <summary>
        ///     The location to save to.
        /// </summary>
        public SaveSystem.Location location;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public SaveSystemSettings()
        {
        }

        /// <summary>
        ///     Parameterized constructor.
        /// </summary>
        /// <param name="location">The location to save at.</param>
        /// <param name="format">The format to save into.</param>
        /// <param name="compress">Whether to compress the data.</param>
        public SaveSystemSettings(SaveSystem.Location location, SaveSystem.Format format, bool compress)
        {
            this.location = location;
            this.format = format;
            this.compress = compress;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="origin"></param>
        public SaveSystemSettings(SaveSystemSettings origin)
        {
            location = origin.location;
            format = origin.format;
            compress = origin.compress;
        }
    }
}