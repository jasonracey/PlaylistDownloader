using System;

namespace PlaylistDownloaderLib
{
    public class ProcessingStatus
    {
        public ProcessingStatus(
            int countCompleted,
            int countTotal,
            string? message = null)
        {
            if (countCompleted < 0)
                throw new ArgumentException("Must be >= 0");
            if (countTotal < 0)
                throw new ArgumentException("Must be >= 0");
			
            CountCompleted = countCompleted;
            CountTotal = countTotal;
            Message = message ?? string.Empty;
        }
		
        public int CountCompleted { get; }
        
        public int CountTotal { get; }
        
        public string Message { get; }
    }
}