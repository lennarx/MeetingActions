# Meeting Actions Web App

A minimal web application to consume the Meeting Actions API. Submit meeting transcripts and get actionable insights including decisions, actions, dates, risks, and open questions.

## Features

- Submit meeting transcripts for analysis
- Real-time processing status with polling
- View and copy results to clipboard
- Simple, clean UI built with React + Tailwind CSS

## Prerequisites

- Node.js 18 or higher
- npm or yarn
- Backend API running on `http://localhost:5000`

## Installation

1. Install dependencies:

```bash
npm install
```

## Running Locally

1. Make sure the backend API is running on `http://localhost:5000`

2. Start the development server:

```bash
npm run dev
```

3. Open your browser to `http://localhost:5173`

## Usage

1. **Create Job**: Paste your meeting transcript, select meeting type (daily/client/planning), and click "Generate Actions"
2. **Processing**: Wait while the API analyzes your transcript (polls status every 3 seconds)
3. **Result**: View the extracted insights and copy them to clipboard

## Building for Production

```bash
npm run build
```

The built files will be in the `dist/` directory.

## Preview Production Build

```bash
npm run preview
```

## Project Structure

```
src/
├── pages/           # Page components (CreateJob, Processing, Result)
├── services/        # API service layer
├── types/           # TypeScript type definitions
├── utils/           # Helper functions
├── App.tsx          # Router setup
└── main.tsx         # Entry point
```

## API Endpoints Used

- `POST /v1/jobs` - Create new job
- `GET /v1/jobs/{jobId}` - Get job status
- `GET /v1/jobs/{jobId}/result` - Get job result

## Tech Stack

- React 18
- TypeScript
- Vite
- React Router
- Tailwind CSS
- Fetch API (no Axios)

## Notes

- No authentication required
- Simple state management with React hooks
- Minimal UI design focused on functionality
- Backend must be running for the app to work
