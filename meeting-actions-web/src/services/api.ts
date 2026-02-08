import type {
  CreateJobRequest,
  CreateJobResponse,
  JobStatusResponse,
  JobResultResponse
} from '../types/api';

const DEV_API_BASE = 'http://localhost:5000/v1';
const PROD_API_BASE = 'https://meeting-actions-api-a0cngbg0g7crcbav.centralus-01.azurewebsites.net/v1';

function normalizeApiBase(rawBase: string): string {
  const trimmed = rawBase.trim().replace(/\/+$/, '');
  return /\/v1$/i.test(trimmed) ? trimmed : `${trimmed}/v1`;
}

function resolveApiBase(): string {
  const configured = import.meta.env.VITE_API_URL;

  if (typeof configured === 'string' && configured.trim().length > 0) {
    return normalizeApiBase(configured);
  }

  return import.meta.env.DEV ? DEV_API_BASE : PROD_API_BASE;
}

const API_BASE = resolveApiBase();

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
