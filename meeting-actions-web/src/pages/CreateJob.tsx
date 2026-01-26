import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createJob } from '../services/api';
import { InputType } from '../types/api.ts';

export default function CreateJob() {
  const navigate = useNavigate();
  const [transcriptText, setTranscriptText] = useState('');
  const [meetingType, setMeetingType] = useState('daily');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Disable submit if transcript is less than 10 characters (API validation requirement)
  const isValid = transcriptText.trim().length >= 10;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!isValid) return;

    setLoading(true);
    setError(null);

    try {
      // Call API to create job
      const response = await createJob({
        meetingType,
        inputType: InputType.Text,
        transcriptText: transcriptText.trim(),
      });

      // Navigate to processing page with jobId
      navigate(`/processing?jobId=${response.jobId}`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create job');
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-50 via-purple-50 to-pink-50 py-12 px-4">
      <div className="max-w-3xl mx-auto">
        {/* Header with gradient text */}
        <div className="text-center mb-10">
          <h1 className="text-5xl font-extrabold bg-gradient-to-r from-indigo-600 via-purple-600 to-pink-600 bg-clip-text text-transparent mb-3">
            Meeting Actions
          </h1>
          <p className="text-lg text-gray-700">
            Transform your meeting transcripts into actionable insights with AI
          </p>
        </div>

        {/* Main card with glassmorphism effect */}
        <form onSubmit={handleSubmit} className="bg-white/80 backdrop-blur-sm rounded-2xl shadow-2xl p-8 space-y-6 border border-white/20">
          {/* Meeting Type Select with icon */}
          <div>
            <label htmlFor="meetingType" className="flex items-center text-sm font-semibold text-gray-800 mb-3">
              <svg className="w-5 h-5 mr-2 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
              Meeting Type
            </label>
            <select
              id="meetingType"
              value={meetingType}
              onChange={(e) => setMeetingType(e.target.value)}
              className="w-full px-4 py-3 border-2 border-indigo-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent bg-white transition-all hover:border-indigo-300"
            >
              <option value="daily">🌅 Daily Standup</option>
              <option value="client">👥 Client Meeting</option>
              <option value="planning">📋 Planning Session</option>
            </select>
          </div>

          {/* Transcript Text Area with icon */}
          <div>
            <label htmlFor="transcriptText" className="flex items-center text-sm font-semibold text-gray-800 mb-3">
              <svg className="w-5 h-5 mr-2 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              Meeting Transcript
            </label>
            <textarea
              id="transcriptText"
              value={transcriptText}
              onChange={(e) => setTranscriptText(e.target.value)}
              placeholder="Paste the meeting transcript. We’ll extract decisions, tasks, owners and dates."
              rows={12}
              className="w-full px-4 py-3 border-2 border-purple-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent font-mono text-sm bg-white transition-all hover:border-purple-300 resize-none"
            />
            <div className="flex items-center mt-2">
              <svg className="w-4 h-4 mr-1 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <p className="text-sm text-gray-600">
                Minimum 10 characters required
              </p>
            </div>
          </div>

          {/* Error Message with animation */}
          {error && (
            <div className="bg-red-50 border-l-4 border-red-500 text-red-700 px-4 py-3 rounded-lg shadow-sm animate-shake">
              <div className="flex items-center">
                <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                </svg>
                {error}
              </div>
            </div>
          )}

          {/* Submit Button with gradient */}
          <button
            type="submit"
            disabled={!isValid || loading}
            className="w-full bg-gradient-to-r from-indigo-600 via-purple-600 to-pink-600 text-white font-bold py-4 px-6 rounded-xl hover:from-indigo-700 hover:via-purple-700 hover:to-pink-700 disabled:from-gray-300 disabled:via-gray-300 disabled:to-gray-300 disabled:cursor-not-allowed transition-all transform hover:scale-105 active:scale-95 shadow-lg disabled:shadow-none disabled:transform-none"
          >
            {loading ? (
              <span className="flex items-center justify-center">
                <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Creating Job...
              </span>
            ) : (
              <span className="flex items-center justify-center">
                <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                </svg>
                Extract Actions
              </span>
            )}
          </button>
        </form>

        {/* Footer info */}
        <div className="text-center mt-8 text-sm text-gray-600">
          <p>Powered by AI • Secure & Private • Instant Results</p>
        </div>
      </div>
    </div>
  );
}
