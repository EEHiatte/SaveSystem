using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Logging class that can be used to log messages to the console.
    /// </summary>
    /// <remarks>
    /// Can be repurposed if necessary to log to a file or other implementation not using Unity.
    /// </remarks>
    public static class Logging
    {
        /// <summary>
        /// Whether basic log messages should be logged.
        /// </summary>
        public static bool DebugLogMessages = true;
        
        /// <summary>
        /// Whether warning messages should be logged.
        /// </summary>
        public static bool DebugWarningMessages = true;
        
        /// <summary>
        /// Whether error messages should be logged.
        /// </summary>
        public static bool DebugErrorMessages = true;
        
        /// <summary>
        /// Log a message to the console.
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void Log(string message)
        {
            if (DebugLogMessages)
            {
                Debug.Log(message);
            }
        }
        
        /// <summary>
        /// Log a warning message to the console.
        /// </summary>
        /// <param name="message">Warning message to log.</param>
        public static void LogWarning(string message)
        {
            if (DebugWarningMessages)
            {
                Debug.LogWarning(message);
            }
        }
        
        /// <summary>
        /// Log an error message to the console.
        /// </summary>
        /// <param name="message">Error message to log.</param>
        public static void LogError(string message)
        {
            if (DebugErrorMessages)
            {
                Debug.LogError(message);
            }
        }
    }
}