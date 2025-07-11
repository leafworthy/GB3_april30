namespace Gaskellgames
{
    #region GaskellgamesLogs

    internal enum ShowGaskellgamesLogs
    {
        Always,
        Never
    }

    #endregion
    
    //----------------------------------------------------------------------------------------------------
    
    #region LogicGate
    
    public enum LogicGate
    {
        /// <summary>
        /// BUFFER: The Output is TRUE if the (single) input is true. Otherwise, the output is FALSE.
        /// </summary>
        BUFFER,
            
        /// <summary>
        /// NOT: The Output is TRUE if the (single) input is false. Otherwise, the output is FALSE.
        /// </summary>
        NOT,
            
        /// <summary>
        /// AND: The output is TRUE when all inputs are true. Otherwise, the output is FALSE.
        /// </summary>
        AND,
            
        /// <summary>
        /// AND: The output is TRUE when any inputs are false. Otherwise, the output is FALSE.
        /// </summary>
        NAND,
            
        /// <summary>
        /// OR: The output is TRUE if any of the inputs are true. Otherwise, FALSE.
        /// </summary>
        OR,
            
        /// <summary>
        /// NOR: The output is TRUE if none of the inputs are true. Otherwise, FALSE.
        /// </summary>
        NOR,
            
        /// <summary>
        /// XOR: The output is TRUE if only one of the inputs are true. Otherwise, FALSE.
        /// </summary>
        XOR,
            
        /// <summary>
        /// XNOR: The output is TRUE if none, or more than one, of the inputs are true. Otherwise, FALSE.
        /// </summary>
        XNOR
    }

    #endregion
    
    //----------------------------------------------------------------------------------------------------
    
    #region Units

    public enum Units
    {
        /// <summary> </summary>
        None,
        
        // --- time ---

        /// <summary>
        /// Milliseconds: ms
        /// </summary>
        Milliseconds,

        /// <summary>
        /// Seconds: s
        /// </summary>
        Seconds,

        /// <summary>
        /// Minutes: m
        /// </summary>
        Minutes,

        /// <summary>
        /// Hours: h
        /// </summary>
        Hours,

        /// <summary>
        /// Days: d
        /// </summary>
        Days,
        
        // --- storage ---

        /// <summary>
        /// Bytes: B
        /// </summary>
        Bytes,

        /// <summary>
        /// Kilobytes: kB
        /// </summary>
        Kilobytes,

        /// <summary>
        /// Megabytes: MB
        /// </summary>
        Megabytes,
        
        // --- force ---
        
        /// <summary>
        /// Nutons: N
        /// </summary>
        Nutons,
        
        // --- general ---

        /// <summary>
        /// Units; u
        /// </summary>
        Units,

        /// <summary>
        /// Multiplier; x
        /// </summary>
        Multiplier,

        /// <summary>
        /// Percentage: %
        /// </summary>
        Percentage,

        /// <summary>
        /// Degrees: \u00B0
        /// </summary>
        Degrees,

        /// <summary>
        /// Radians: rad
        /// </summary>
        Radians,

        /// <summary>
        /// Frames: frames
        /// </summary>
        Frames,
        
        // --- 'unit' per second ---

        /// <summary>
        /// PerSecond: /s
        /// </summary>
        PerSecond,

        /// <summary>
        /// UnitsPerSecond: u/s
        /// </summary>
        UnitsPerSecond,

        /// <summary>
        /// DegreesPerSecond: \u00B0/s
        /// </summary>
        DegreesPerSecond,

        /// <summary>
        /// RadiansPerSecond: rad/s
        /// </summary>
        RadiansPerSecond,

        /// <summary>
        /// FramesPerSecond: fps
        /// </summary>
        FramesPerSecond,
    }

    #endregion
    
    //----------------------------------------------------------------------------------------------------
    
    #region InfoMessageType

    public enum InfoMessageType
    {
        None,
        Info,
        Warning,
        Error
    }

    #endregion
    
    //----------------------------------------------------------------------------------------------------

    #region TaskResultType

    public enum TaskResultType
    {
        Timeout,
        Cancelled,
        Complete,
    }

    #endregion
    
    //----------------------------------------------------------------------------------------------------

    #region TrackedTaskStatus

    internal enum TrackedTaskStatus
    {
        InProgress,
        Timeout,
        Cancelled,
        Complete,
    }

    #endregion
    
    //----------------------------------------------------------------------------------------------------
    
    #region Alignment

    public enum Alignment
    {
        Left,
        Center,
        Right
    }

    #endregion
    
    //----------------------------------------------------------------------------------------------------
    
    #region GUIColorTarget

    public enum GUIColorTarget
    {
        All,
        Content,
        Background
    }

    #endregion
    
} // namespace end
