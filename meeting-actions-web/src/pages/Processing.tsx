import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { getJobStatus } from '../services/api';
import { JobStatus } from '../types/api.ts';

export default function Processing() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const jobId = searchParams.get('jobId');

  const [status, setStatus] = useState<JobStatus | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // If no jobId in URL, redirect to home
    if (!jobId) {
      navigate('/');
      return;
    }

    let intervalId: number;

    // Function to poll job status
    const pollStatus = async () => {
      try {
        const response = await getJobStatus(jobId);
        setStatus(response.status);

        // If job is done, navigate to result page
        if (response.status === JobStatus.Done) {
          clearInterval(intervalId);
          navigate(`/result?jobId=${jobId}`);
        }

        // If job failed, show error
        if (response.status === JobStatus.Failed) {
          clearInterval(intervalId);
          setError(response.errorMessage || 'Job processing failed');
        }
      } catch (err) {
        clearInterval(intervalId);
        setError(err instanceof Error ? err.message : 'Failed to check job status');
      }
    };

    // Poll immediately
    pollStatus();

    // Then poll every 3 seconds
    intervalId = setInterval(pollStatus, 3000);

    // Cleanup interval on unmount
    return () => {
      if (intervalId) {
        clearInterval(intervalId);
      }
    };
  }, [jobId, navigate]);

  const getStatusText = () => {
    if (status === JobStatus.Pending) return 'Pending...';
    if (status === JobStatus.Processing) return 'Processing...';
    return 'Processing meeting...';
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-50 via-purple-50 to-pink-50 flex items-center justify-center px-4">
      <div className="max-w-lg w-full">
        {error ? (
          <>
            {/* Error State */}
            <div className="bg-white/80 backdrop-blur-sm rounded-2xl shadow-2xl p-10 text-center border border-white/20">
              <div className="mb-6 relative">
                <div className="absolute inset-0 bg-red-500/20 rounded-full blur-3xl"></div>
                <svg
                  className="relative mx-auto h-20 w-20 text-red-500"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
              </div>
              <h2 className="text-3xl font-bold text-gray-900 mb-4">Processing Failed</h2>
              <p className="text-gray-700 mb-8 text-lg">{error}</p>
              <button
                onClick={() => navigate('/')}
                className="bg-gradient-to-r from-indigo-600 to-purple-600 text-white font-bold py-3 px-8 rounded-xl hover:from-indigo-700 hover:to-purple-700 transition-all transform hover:scale-105 active:scale-95 shadow-lg"
              >
                Start Over
              </button>
            </div>
          </>
        ) : (
          <>
            {/* Loading State */}
            <div className="bg-white/80 backdrop-blur-sm rounded-2xl shadow-2xl p-10 text-center border border-white/20">
              <div className="mb-8 relative">
                {/* Animated circles */}
                <div className="relative mx-auto w-24 h-24">
                  <div className="absolute inset-0 bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500 rounded-full blur-2xl opacity-50 animate-pulse"></div>
                  <div className="relative inline-block animate-spin rounded-full h-24 w-24 border-8 border-transparent border-t-indigo-600 border-r-purple-600 border-b-pink-600"></div>
                </div>
              </div>

              <h2 className="text-3xl font-bold bg-gradient-to-r from-indigo-600 via-purple-600 to-pink-600 bg-clip-text text-transparent mb-4">
                {getStatusText()}
              </h2>

              <p className="text-gray-700 text-lg mb-6">
                Analyzing your meeting transcript and generating insights
              </p>

              {/* Progress indicators */}
              <div className="space-y-3 text-sm text-gray-600">
                <div className="flex items-center justify-center animate-pulse">
                  <div className="w-2 h-2 bg-indigo-600 rounded-full mr-2"></div>
                  <span>Extracting decisions...</span>
                </div>
                <div className="flex items-center justify-center animate-pulse" style={{animationDelay: '0.2s'}}>
                  <div className="w-2 h-2 bg-purple-600 rounded-full mr-2"></div>
                  <span>Identifying action items...</span>
                </div>
                <div className="flex items-center justify-center animate-pulse" style={{animationDelay: '0.4s'}}>
                  <div className="w-2 h-2 bg-pink-600 rounded-full mr-2"></div>
                  <span>Detecting dates and risks...</span>
                </div>
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
