import type { CsvUploadResponse } from './types'

export async function fetchHealth(): Promise<{ status: string; timestamp: string }> {
  const response = await fetch('/api/health')
  return response.json()
}

export async function uploadCsv(file: File): Promise<CsvUploadResponse> {
  const formData = new FormData()
  formData.append('file', file)

  const response = await fetch('/api/csv/upload', {
    method: 'POST',
    body: formData,
  })

  if (!response.ok) {
    const error = await response.json()
    throw new Error(error.errorSummary || 'Failed to upload CSV')
  }

  return response.json()
}
