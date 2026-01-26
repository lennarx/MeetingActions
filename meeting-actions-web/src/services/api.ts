import type {
  CreateJobRequest,
  CreateJobResponse,
  JobStatusResponse,
  JobResultResponse
} from '../types/api';

// Base API URL - Use environment variable or default to localhost:5000
const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5000/v1';

/**
 * Helper function to handle API responses
 * Throws error if response is not ok
 */
async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const errorText = await response.text();
    let errorMessage = `API Error: ${response.status} ${response.statusText}`;

    try {
      const errorJson = JSON.parse(errorText);
      errorMessage = errorJson.error || errorJson.message || errorMessage;
    } catch {
      // If not JSON, use the text as error message
      if (errorText) {
        errorMessage = errorText;
      }
    }

    throw new Error(errorMessage);
  }

  return response.json();
}

/**
 * POST /v1/jobs
 * Creates a new job with meeting transcript
 */
export async function createJob(request: CreateJobRequest): Promise<CreateJobResponse> {
  const response = await fetch(`${API_BASE}/jobs`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  });

  return handleResponse<CreateJobResponse>(response);
}

/**
 * GET /v1/jobs/{jobId}
 * Gets the current status of a job
 */
export async function getJobStatus(jobId: string): Promise<JobStatusResponse> {
  const response = await fetch(`${API_BASE}/jobs/${jobId}`);
  return handleResponse<JobStatusResponse>(response);
}

/**
 * GET /v1/jobs/{jobId}/result
 * Gets the result of a completed job
 */
export async function getJobResult(jobId: string): Promise<JobResultResponse> {
  const response = await fetch(`${API_BASE}/jobs/${jobId}/result`);
  return handleResponse<JobResultResponse>(response);
}
