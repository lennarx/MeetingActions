// Enums matching backend contracts
export const InputType = {
  Text: 0,
  Audio: 1,
  Video: 2
} as const;

export type InputType = (typeof InputType)[keyof typeof InputType];

export const JobStatus = {
  Pending: 0,
  Processing: 1,
  Done: 2,
  Failed: 3
} as const;

export type JobStatus = (typeof JobStatus)[keyof typeof JobStatus];

// Request types
export interface CreateJobRequest {
  meetingType: string;
  inputType: InputType;
  transcriptText: string;
}

// Response types
export interface CreateJobResponse {
  jobId: string;
}

export interface JobStatusResponse {
  jobId: string;
  status: JobStatus;
  createdAtUtc: string;
  updatedAtUtc: string;
  errorMessage?: string;
}

export interface JobResultResponse {
  jobId: string;
  resultJson: string; // JSON stored as string
}

// Parsed result structure (expected format from API)
export interface ParsedResult {
  decisions?: string[];
  actions?: string[];
  implicitDates?: string[];
  risks?: string[];
  openQuestions?: string[];
}
