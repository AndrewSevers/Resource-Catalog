namespace Extensions.Utils {

    public enum BezierControlPointMode {
		Free,
		Aligned,
		Mirrored
	}

    public enum BezierPointType {
        Origin,
        StartTangent,
        EndTangent
    }

    public enum BezierEdgeType {
        Start,
        End
    }

    /// <summary>
    /// Mode of traversal: 
    /// Once = Traverse once and finish, 
    /// Loop = Traverse and once finished move to the beggining and start again
    /// QuickLoop = Traverse and once finished jump to beginning and start again
    /// PingPong = Traverse from start to finish and finish to start forever
    /// Extend = Traverse once and upon finishing move spline to current origin and repeat traversal forever
    /// </summary>
    public enum TraversalMode {
        Once = 0,
        Loop = 1,
        QuickLoop = 2,
        PingPong = 3,
        Extend = 4
    }

    public enum TravelDirection {
        None = 0,
        Forwards = 1,
        Backwards = 2
    }

    public enum FacingDirection {
        Right = 0,
        Down = 90,
        Left = 180,
        Up = 270
    }

}
