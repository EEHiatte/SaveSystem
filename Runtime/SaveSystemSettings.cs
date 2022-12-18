namespace SaveSystem
{
    /// <summary>
    /// The settings used by the save system.
    /// </summary>
    public class SaveSystemSettings
    {
        /// <summary>
        /// The location to save to.
        /// </summary>
        public SaveSystem.Location location;
        
        /// <summary>
        /// The format to save in.
        /// </summary>
        public SaveSystem.Format format;
        
        /// <summary>
        /// Whether to use compression.
        /// </summary>
        public bool compress;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SaveSystemSettings()
        {
            
        }

        /// <summary>
        /// Copy constructor
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