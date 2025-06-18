using System;

namespace GangstaBean.Console
{
    /// <summary>
    /// Interface for all console commands
    /// </summary>
    public interface IConsoleCommand
    {
        /// <summary>
        /// Primary command name (e.g., "respawn")
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Command description for help system
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Alternative names for the command (e.g., ["r", "res"])
        /// </summary>
        string[] Aliases { get; }
        
        /// <summary>
        /// Usage example (e.g., "respawn player1" or "heal player1 100")
        /// </summary>
        string Usage { get; }
        
        /// <summary>
        /// Execute the command with given parameters
        /// </summary>
        /// <param name="parameters">Command parameters (excluding the command name itself)</param>
        /// <returns>Result message to display in console</returns>
        string Execute(string[] parameters);
        
        /// <summary>
        /// Validate if the parameters are correct before execution
        /// </summary>
        /// <param name="parameters">Parameters to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        bool ValidateParameters(string[] parameters);
    }
    
    /// <summary>
    /// Attribute to mark classes as console commands for automatic registration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConsoleCommandAttribute : Attribute
    {
        public bool AutoRegister { get; set; } = true;
    }
}